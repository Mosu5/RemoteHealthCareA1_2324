using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using VRConnection.Communication;
using VRConnection.Graphics;

namespace VRConnection
{
    public class VrSession
    {
        private string? _sessionId;
        private string? _tunnelId;

        /// <summary>
        /// Initialize the connection by receiving the session ID of the NetworkEngine running on the same
        /// computer that is running the application, and the tunnel ID
        /// </summary>
        /// <returns>Wether a successful connection was established to the server.</returns>
        public async Task<bool> Initialize(string serverIpAddress, int serverPort)
        {
            if (!await VrCommunication.ConnectToServer(serverIpAddress, serverPort))
            {
                await Console.Out.WriteLineAsync($"Could not connect to address {serverIpAddress} on port {serverPort}. Maybe there is an already active session?");
                return false;
            }

            await VrCommunication.SendAsJson(Formatting.SessionListGet());

            JsonObject sessionListResponse = await VrCommunication.ReceiveJsonObject();
            _sessionId = Formatting.ValidateAndGetSessionId(sessionListResponse);

            await VrCommunication.SendAsJson(Formatting.TunnelAdd(_sessionId));

            JsonObject tunnelCreateResponse = await VrCommunication.ReceiveJsonObject();
            _tunnelId = Formatting.ValidateAndGetTunnelId(tunnelCreateResponse);

            return true;
        }

        /// <summary>
        /// Close the connection with the VR server
        /// </summary>
        public void Close() => VrCommunication.CloseConnection();

        /// <summary>
        /// Send a request to VR server and retrieve the VR scene data
        /// </summary>
        public async Task<JsonObject> GetScene()
        {
            object sceneGetCommand = Formatting.SceneGet();
            // TODO create separate tunnel message
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, sceneGetCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        /// <summary>
        ///  Add a model in VR scene
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="position">position array containing x, y, z</param>
        /// <param name="scale">ses scaling of model</param>
        /// <param name="fileName"> filepath of the obj file of the model</param>
        public async Task<JsonObject> AddModel(string name, int[] position, double scale, string fileName)
        {
            // TODO add position data
            object modelAddCommand = Formatting.Add3DObject(name, position, scale, fileName);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, modelAddCommand);
            Console.WriteLine(tunnelMessage);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }


        /// <summary>
        ///  Add a model in VR scene
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="position">position array containing x, y, z</param>
        /// <param name="scale">ses scaling of model</param>
        /// <param name="fileName"> filepath of the obj file of the model</param>
        /// <param name="animationName"> filepath of the animation file</param>
        public async Task<JsonObject> AddAnimatedModel(string name, int[] position, double scale, string fileName, string animationName)
        {
            // TODO add position data
            // TODO add position data
            object modelAddCommand = Formatting.AddAnimatedObject(name, position, scale, fileName, animationName);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, modelAddCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        /// <summary>
        /// Add terrain data to VR scene
        /// Since its only positional data, no visual component is rendered in the scene
        /// </summary>
        public async Task<JsonObject> AddTerrain(int[] size, float[] heightMap)
        {
            object terrainAddCommand = Formatting.TerrainAdd(size, heightMap);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, terrainAddCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        /// <summary>
        /// Add terrain node to VR scene
        /// Visual component (layers) can be added after this
        /// </summary>
        public async Task<JsonObject> AddTerrainNode()
        {
            object addTerrainNodeCommand = Formatting.TerrainAddNode();
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, addTerrainNodeCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        /// <summary>
        /// Add visual component to terrain
        /// Requires actual node to add visual component
        /// </summary>
        public async Task<JsonObject> AddTerrainLayer()
        {
            // command data
            string terrainId = await GetNodeId("terrain");
            string diffuseFilePath = @"data\NetworkEngine\textures\grass_diffuse.png";
            string normalFilePath = @"data\NetworkEngine\textures\grass_normal.png";

            // create command and send to VR
            object addTerrainLayerCommand = Formatting.TerrainAddLayer(terrainId, diffuseFilePath, normalFilePath);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, addTerrainLayerCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }

        /// <summary>
        /// Get uuid from node based on name
        /// </summary>
        /// <param name="name">name of the requested node</param>
        /// <returns>uuid as string</returns>
        public async Task<string> GetNodeId(string name)
        {
            object sceneFindNodeCommand = Formatting.SceneNodeFind(name);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, sceneFindNodeCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            JsonObject response = await VrCommunication.ReceiveJsonObject();
            var nodes = response?["data"]?["data"]?["data"]?.AsArray();

            if (nodes == null)
                throw new CommunicationException("Could not retrieve the nodes JsonArray from message.");

            return GetNode(nodes);
        }

        /// <summary>
        /// Helper method to get the first node's UUID, or else an empty string.
        /// It is assumed that the first node is the terrain node.
        /// TODO check if the terrain is always the first node in this JsonArray
        /// </summary>
        private string GetNode(JsonArray nodes)
        {
            string uuid = string.Empty;
            if (nodes != null)
            {
                var node = nodes.First(); // only need one node with the name needed
                uuid = node?["uuid"]?.ToString() ?? string.Empty;
            }
            return uuid;
        }

        /// <summary>
        /// Add route to VR scene
        /// Nodes are added in the order they are given
        /// </summary>
        public async Task<JsonObject> AddRoute(PosVector[] nodes)
        {
            object routeAddCommand = Formatting.RouteAdd(nodes);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, routeAddCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }
        
        /// <summary>
        /// Add road to VR scene
        /// Road follows route
        /// The diffuse, normal and specular are textures used for the road
        /// </summary>
        public async Task<JsonObject> AddRoad()
        {
            // command data
            string routeId = await GetNodeId("route");
            string normal = @"data\NetworkEngine\textures\terrain\ground_cracked_n.jpg";
            string diffuse = @"data\NetworkEngine\textures\terrain\ground_mud2_d.jpg";
            string specular = @"data\NetworkEngine\textures\terrain\ground_mud2_s.jpg";
            
            // create command and send to VR
            object roadAddCommand = Formatting.RoadAdd(routeId, diffuse, normal, specular);
            object tunnelMessage = Formatting.TunnelSend(_tunnelId, roadAddCommand);

            await VrCommunication.SendAsJson(tunnelMessage);
            return await VrCommunication.ReceiveJsonObject();
        }
        
    }
}
