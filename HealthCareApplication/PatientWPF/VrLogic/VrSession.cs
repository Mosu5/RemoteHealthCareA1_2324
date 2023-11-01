using System.Linq;
using System;
using System.Numerics;
using System.Threading.Tasks;
using PatientApp.VrLogic.Graphics;
using Newtonsoft.Json.Linq;
using Utilities.Communication;

namespace PatientApp.VrLogic
{
    public class Data
    {
        public string DataType { get; set; }
        public object Value { get; set; }
    }

    public class VrSession
    {
        private static string _sessionId;
        private static string _tunnelId;

        #region Connectivity
        /// <summary>
        ///     Initialize the connection by receiving the session ID of the NetworkEngine running on the same
        ///     computer that is running the application, and the tunnel ID
        /// </summary>
        /// <returns>Wether a successful connection was established to the server.</returns>
        public static async Task<bool> Initialize(string serverIpAddress, int serverPort)
        {
            // If the application could not connect to the server
            if (!await VrCommunication.ConnectToServer(serverIpAddress, serverPort))
            {
                return false;
            }

            // Request session list
            await VrCommunication.SendAsJson(Formatting.SessionListGet());

            // Receive and process the session ID of the running NetworkEngine instance
            JObject sessionListResponse = await VrCommunication.ReceiveJsonObject();
            _sessionId = Formatting.ValidateAndGetSessionId(sessionListResponse);

            if (_sessionId == null) return false;

            // Request adding a new tunnel
            await VrCommunication.SendAsJson(Formatting.TunnelCreate(_sessionId));

            // Receive and process the tunnel ID of the running NetworkEngine instance
            JObject tunnelCreateResponse = await VrCommunication.ReceiveJsonObject();
            _tunnelId = Formatting.ValidateAndGetTunnelId(tunnelCreateResponse);
            return true;
        }

        /// <summary>
        /// Close the connection with the VR server
        /// </summary>
        public static void Close() => VrCommunication.CloseConnection();
        #endregion

        #region Scene

        public static async Task SaveScene()
        {
            object sceneSave = Formatting.SceneSave();
            object tunnelSend = Formatting.TunnelSend(_tunnelId, sceneSave);
            await VrCommunication.SendAsJson(tunnelSend);
        }

        public static async Task LoadScene()
        {
            object sceneLoad = Formatting.SceneLoad();
            object tunnelSend = Formatting.TunnelSend(_tunnelId, sceneLoad);
            await VrCommunication.SendAsJson(tunnelSend);
        }

        /// <summary>
        ///     Send a request to VR server and retrieve the VR scene data
        /// </summary>
        public static async Task<JObject> GetScene()
        {
            var sceneGetCommand = Formatting.SceneGet();
            var tunnelMessage = Formatting.TunnelSend(_tunnelId, sceneGetCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        /// <summary>
        ///     Reset the VR scene to default
        /// </summary>
        public static async Task<JObject> ResetScene()
        {
            var sceneResetCommand = Formatting.SceneReset();
            var tunnelMessage = Formatting.TunnelSend(_tunnelId, sceneResetCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }
        #endregion

        #region Nodes
        /// <summary>
        ///     Get uuid from node based on name
        /// </summary>
        /// <param name="name">name of the requested node</param>
        /// <returns>uuid as string</returns>
        public static async Task<string> GetNodeId(string name)
        {
            JObject response = await RequestNodesWithName(name);
            var responseAsArray = response?["data"]?["data"]?["data"];

            // If there were multiple nodes with the same name, get the first one's id
            if (responseAsArray is JArray)
            {
                if (responseAsArray == null)
                    throw new CommunicationException($"Could not retrieve nodes with name {name} from response.");
                return GetFirstNode((JArray)responseAsArray);
            }

            // Response is just a single object, so retrieve value as string
            string responseAsString = (string)response["data"]["id"];
            if (responseAsString == null)
                throw new CommunicationException($"Could not retrieve the node with name {name} from response.");

            return responseAsString;
        }

        /// <summary>
        /// Queries the VR server to find all nodes with a specific name.
        /// </summary>
        /// <returns>The VR server's response</returns>
        private static async Task<JObject> RequestNodesWithName(string name)
        {
            object sceneFindNodeCommand = Formatting.SceneNodeFind(name);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, sceneFindNodeCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        /// <summary>
        ///     Helper method to get the first node's UUID, or else an empty string.
        ///     It is assumed that the first node in the JsonArray is always the target node.
        /// </summary>
        private static string GetFirstNode(JArray nodes)
        {
            var node = nodes.First(); // only need one node with the name needed
            string nodeId = node?["uuid"]?.ToString();

            if (nodeId == null)
                throw new CommunicationException($"Could not retrieve this node from the response.");

            return nodeId;
        }

        /// <summary>
        /// Removes a node from the VR environment.
        /// </summary>
        public static async Task<JObject> RemoveNode(string nodeName)
        {
            string uuid = await GetNodeId(nodeName);

            object removeNodeCommand = Formatting.RemoveNode(uuid);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, removeNodeCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }
        #endregion

        #region Skybox
        /// <summary>
        /// Sets the time of the skybox, to be able to simulate day/night cycle
        /// </summary>
        public static async Task<JObject> SetSkyTime(double time)
        {
            object setSkyCommand = Formatting.SetSkyboxTime(time);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, setSkyCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }
        public static async Task<JObject> UpdateSkybox(String rt, String lf, String up, String dn, String bk, String ft)
        {
            object setSkyCommand = Formatting.SkyboxUpdate(rt, lf, up, dn, bk, ft);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, setSkyCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        #endregion

        #region Terrain
        /// <summary>
        /// Adds a grassy hills type terrain to the scene,
        /// with the height map generated using Perlin noise.
        /// </summary>
        /// <returns>A string concatenation of all responses sent by the server while creating the terrain.</returns>
        public static async Task<string> AddHillTerrain(int length, int width, Vector3 position, Vector3 rotation, int height)
        {
            float[] heightMap = PerlinNoiseGenerator.GenerateHeightMap(height);

            JObject terrainData = await AddTerrainData(length, width, heightMap);
            JObject terrainNode = await AddTerrainNode(position, rotation);
            JObject terrainLayer = await AddTerrainGrassLayer();

            return terrainData.ToString() + terrainNode.ToString() + terrainLayer.ToString();
        }

        /// <summary>
        ///     Add terrain data to VR scene
        ///     Since its only positional data, no visual component is rendered in the scene
        /// </summary>
        /// <param name="heightMap">Height data of the terrain</param>
        private static async Task<JObject> AddTerrainData(int length, int width, float[] heightMap)
        {
            var terrainAddCommand = Formatting.TerrainAdd(length, width, heightMap);
            var tunnelMessage = Formatting.TunnelSend(_tunnelId, terrainAddCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        /// <summary>
        ///     Add terrain node to VR scene
        ///     Visual component (layers) can be added after this
        /// </summary>
        private static async Task<JObject> AddTerrainNode(Vector3 position, Vector3 rotation)
        {
            var addTerrainNodeCommand = Formatting.TerrainAddNode(position, rotation);
            var tunnelMessage = Formatting.TunnelSend(_tunnelId, addTerrainNodeCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        /// <summary>
        ///     Add visual component to terrain
        ///     Requires actual node to add visual component
        /// </summary>
        private static async Task<JObject> AddTerrainGrassLayer()
        {
            // Get the node ID of the terrain
            var terrainId = await GetNodeId("terrain");

            // Get the textures of the grass for the layer
            var diffuseFilePath = @"data\NetworkEngine\textures\grass_diffuse.png";
            var normalFilePath = @"data\NetworkEngine\textures\grass_normal.png";

            var addTerrainLayerCommand = Formatting.TerrainAddLayer(terrainId, diffuseFilePath, normalFilePath);
            var tunnelMessage = Formatting.TunnelSend(_tunnelId, addTerrainLayerCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        /// <summary>
        ///     Get height of terrain at position
        /// </summary>
        /// <param name="position"> contains x, y, z position of model </param>
        private static async Task<float> GetTerrainHeight(Vector3 position)
        {
            var heightGetCommand = Formatting.GetHeight(position);
            var tunnelMessage = Formatting.TunnelSend(_tunnelId, heightGetCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            var heightJson = await VrCommunication.ReceiveJsonObject();

            var height = (float)heightJson["data"]["data"]["data"]["height"]; //TODO error handling in case terrain is not added
            return height;
        }
        #endregion

        #region Models
        /// <summary>
        ///     Add a model in VR scene at the height of the terrain on a location
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="position">position array containing x, y, z</param>
        /// <param name="scale">ses scaling of model</param>
        /// <param name="fileName"> filepath of the obj file of the model</param>
        public static async Task<JObject> AddModelOnTerrain(string name, Vector3 position, double scale, string fileName, int rotation)
        {
            // Get height of terrain at position
            var heightJson = await GetTerrainHeight(position);
            position.Y = heightJson; // set height of model to height of terrain

            var modelAddCommand = Formatting.Add3DObject(name, position, scale, fileName, rotation);
            var tunnelMessage = Formatting.TunnelSend(_tunnelId, modelAddCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        /// <summary>
        ///     Add an animated model in VR scene
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="position">position array containing x, y, z</param>
        /// <param name="scale">sets scaling of model</param>
        /// <param name="rotation"> rotates model in x-, y-, z-ax </param>
        /// <param name="fileName"> filepath of the obj file of the model</param>
        /// <param name="animationName"> filepath of the animation file</param>
        public static async Task<JObject> AddAnimatedModel(string name, Vector3 position, double scale, Vector3 rotation,
            string fileName, string animationName)
        {
            var modelAddCommand = Formatting.AddAnimatedObject(
                name, position, scale, rotation, fileName, animationName
            );
            var tunnelMessage = Formatting.TunnelSend(_tunnelId, modelAddCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        /// <summary>
        ///     Add a number of trees randomly in VR scene placed on terrain
        /// </summary>
        /// <param name="amount">amount of spawned trees</param>
        /// <returns>json responses of the tree placements</returns>
        public static async Task<JObject[]> RandomlyPlaceTrees(int amount)
        {
            var jsonResponses = new JObject[amount]; // array to save json responses
            var random = new Random();

            // iterate over amount of trees and add them to the scene
            for (var i = 0; i < amount; i++)
            {
                // generate random x,z position
                float x = random.Next(-128, 128);
                float z = random.Next(-128, 128);
                var position = new Vector3(x, 0, z);
                position.Y = await GetTerrainHeight(position); // set height of model to height of terrain

                var modelAddCommand = Formatting.Add3DObject(
                    $"tree{i}",
                    position,
                    1.5,
                    @"data\NetworkEngine\models\trees\fantasy\tree7.obj",
                    0
                );

                var tunnelMessage = Formatting.TunnelSend(_tunnelId, modelAddCommand);
                await VrCommunication.SendAsJson(tunnelMessage);

                jsonResponses[i] = await VrCommunication.ReceiveJsonObject(); // save response in array
            }

            return jsonResponses; // return array of json responses
        }

        /// <summary>
        /// Put head on bike
        /// </summary>
        /// <param name="cameraID"> ID of camera</param> 
        /// <param name="bikeID"> ID of bike </param>
        /// <returns> Response from server </returns>
        public static async Task<JObject> HeadOnBike(string cameraID, string bikeID)
        {
            object headOnBike = Formatting.SceneNodeUpdate(cameraID, bikeID);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, headOnBike);

            //TODO move to another method
            string headId = await GetNodeId("Head");
            Vector3 rotation = new Vector3(0, 0, 0);
            object turnHead = Formatting.SceneNodeUpdate(headId, rotation);
            object tunnelMessage1 = Formatting.TunnelSend(_tunnelId, headOnBike);

            await VrCommunication.SendAsJson(tunnelMessage1);
            await VrCommunication.ReceiveJsonObject();

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }
        #endregion

        #region Routes
        /// <summary>
        /// Add route to VR scene
        /// Nodes are added in the order they are given
        /// </summary>
        public static async Task<JObject> AddRoute(PosVector[] nodes)
        {
            object routeAddCommand = Formatting.RouteAdd(nodes);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, routeAddCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        public static string GetRouteId(JObject routeResponse)
        {
            string routeId = (string)routeResponse?["data"]?["data"]?["data"]?["uuid"];
            if (routeId == null)
                throw new CommunicationException("Could not find route from route response.");

            return routeId;
        }


        /// <summary>
        /// Let an object follow the route
        /// </summary>
        public static async Task<JObject> FollowRoute(string route, string node, double speed)
        {
            object routeFollowCommand = Formatting.RouteFollow(route, node, speed);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, routeFollowCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        public static async Task<JObject> HideRoute()
        {
            object hide = Formatting.RouteHide();
            await VrCommunication.SendAsJson(Formatting.TunnelSend(_tunnelId, hide));
            return await VrCommunication.ReceiveJsonObject();
        }

        public static async Task<JObject> UpdateSpeed(string node, double speed)
        {
            object updateSpeedCommand = Formatting.RouteSpeed(node, speed);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, updateSpeedCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        #endregion

        #region Roads
        /// <summary>
        /// Add road to VR scene
        /// Road follows route
        /// The diffuse, normal and specular are textures used for the road
        /// </summary>
        public static async Task<JObject> AddRoad(string routeId)
        {
            // command data
            string normal = @"data\NetworkEngine\textures\terrain\ground_mud2_d.jpg";

            // create command and send to VR
            object roadAddCommand = Formatting.RoadAdd(routeId, normal);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, roadAddCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        public static async Task<JObject> AddPanel(String bikeId)
        {
            String name = "panel1";
            //Color color = Color.FromArgb(1, 0, 0, 1);
            //Transform transform = new Transform(1, new double[] { 0, 0, 0}, new double[] { 0, 0, 0 });
            Vector3 position = new Vector3(-0.1f, 1f, -0.22f);
            Vector3 rotation = new Vector3(-45, 90, 0);
            double sizeX = 0.8f;
            double sizeY = 0.8f;
            int resolutionX = 512;
            int resolutionY = 512;
            bool castShadow = true;


            object panelAddCommand = Formatting.PanelAdd(name, bikeId, position, rotation, sizeX, sizeY, resolutionX, resolutionY, castShadow);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, panelAddCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        public static async Task<JObject> AddText(string text)
        {
            string panelID = await GetNodeId("panel1");

            object textAddCommand = Formatting.TextAdd(panelID, text);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, textAddCommand);
            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();

            //  string panelID = await GetNodeId("panel1");
            //  
            //  object textAddCommand = Formatting.TextAdd(panelID, text);
            //  object tunnelMessage = Formatting.TunnelSend(_tunnelId, textAddCommand);
            //
            //  await VrCommunication.SendAsJson(tunnelMessage);
            //  return await VrCommunication.ReceiveJsonObject();
        }

        public static async Task<JObject> ClearPanel()
        {
            string panelID = await GetNodeId("panel1");

            object panelClearCommand = Formatting.PanelClear(panelID);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, panelClearCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        public static async Task<JObject> SwapPanel()
        {
            string panelID = await GetNodeId("panel1");

            object panelSwapCommand = Formatting.PanelSwap(panelID);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, panelSwapCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        public static async Task<JObject> SetColorPanel()
        {
            string panelID = await GetNodeId("panel1");

            object panelSetColorCommand = Formatting.PanelSetColor(panelID);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, panelSetColorCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        #endregion
    }
}


