using System.Numerics;
using System.Text.Json.Nodes;
using VRConnection.Communication;
using VRConnection.Graphics;

namespace VRConnection;

public class VrSession
{
    private string? _sessionId;
    private string? _tunnelId;

    /// <summary>
    ///     Initialize the connection by receiving the session ID of the NetworkEngine running on the same
    ///     computer that is running the application, and the tunnel ID
    /// </summary>
    /// <returns>Wether a successful connection was established to the server.</returns>
    public async Task<bool> Initialize(string serverIpAddress, int serverPort)
    {
        if (!await VrCommunication.ConnectToServer(serverIpAddress, serverPort))
        {
            await Console.Out.WriteLineAsync(
                $"Could not connect to address {serverIpAddress} on port {serverPort}. Maybe there is an already active session?");
            return false;
        }

        await VrCommunication.SendAsJson(Formatting.SessionListGet());

        var sessionListResponse = await VrCommunication.ReceiveJsonObject();
        _sessionId = Formatting.ValidateAndGetSessionId(sessionListResponse);

        await VrCommunication.SendAsJson(Formatting.TunnelAdd(_sessionId));

        var tunnelCreateResponse = await VrCommunication.ReceiveJsonObject();
        _tunnelId = Formatting.ValidateAndGetTunnelId(tunnelCreateResponse);

        return true;
    }

    public async Task<JsonObject> SetSkyTime(double time)
    {
        object setSkyCommand = Formatting.SetSkyboxTime(time);
        object tunnelMessage = Formatting.TunnelSend(_tunnelId, setSkyCommand);
        Console.WriteLine(tunnelMessage);

        await VrCommunication.SendAsJson(tunnelMessage);
        return await VrCommunication.ReceiveJsonObject();
    }


    public async Task<JsonObject> RemoveNode(string nodeName)
    {
        string uuid = await GetNodeId(nodeName);
        await Console.Out.WriteLineAsync("Ground plane id: " + uuid);

        object removeNodeCommand = Formatting.RemoveNode(uuid);
        object tunnelMessage = Formatting.TunnelSend(_tunnelId, removeNodeCommand);
        Console.WriteLine(tunnelMessage);

        await VrCommunication.SendAsJson(tunnelMessage);
        return await VrCommunication.ReceiveJsonObject();
    }

    /// <summary>
    /// Close the connection with the VR server
    /// </summary>
    public void Close() => VrCommunication.CloseConnection();

    /// <summary>
    ///     Send a request to VR server and retrieve the VR scene data
    /// </summary>
    public async Task<JsonObject> GetScene()
    {
        var sceneGetCommand = Formatting.SceneGet();
        var tunnelMessage = Formatting.TunnelSend(_tunnelId, sceneGetCommand);

        await VrCommunication.SendAsJson(tunnelMessage);
        return await VrCommunication.ReceiveJsonObject();
    }

    /// <summary>
    ///     Reset the VR scene to default
    /// </summary>
    public async Task<JsonObject> ResetScene()
    {
        var sceneResetCommand = Formatting.SceneReset();
        var tunnelMessage = Formatting.TunnelSend(_tunnelId, sceneResetCommand);

        await VrCommunication.SendAsJson(tunnelMessage);
        return await VrCommunication.ReceiveJsonObject();
    }

    /// <summary>
    ///     Get height of terrain at position
    /// </summary>
    /// <param name="position"> contains x, y, z position of model </param>
    private async Task<float> GetHeight(Vector3 position)
    {
        var heightGetCommand = Formatting.GetHeight(position);
        var tunnelMessage = Formatting.TunnelSend(_tunnelId, heightGetCommand);

        await VrCommunication.SendAsJson(tunnelMessage);
        var heightJson = await VrCommunication.ReceiveJsonObject();

        var height = heightJson["data"]["data"]["data"]["height"].GetValue<float>(); //TODO error handling in case terrain is not added
        return height;
    }

    /// <summary>
    ///     Add a model in VR scene
    /// </summary>
    /// <param name="name">name of node</param>
    /// <param name="position">position array containing x, y, z</param>
    /// <param name="scale">ses scaling of model</param>
    /// <param name="fileName"> filepath of the obj file of the model</param>
    public async Task<JsonObject> AddModel(string name, Vector3 position, double scale, string fileName)
    {
        // Get height of terrain at position
        var heightJson = await GetHeight(position);
        position.Y = heightJson; // set height of model to height of terrain

        var modelAddCommand = Formatting.Add3DObject(name, position, scale, fileName);
        var tunnelMessage = Formatting.TunnelSend(_tunnelId, modelAddCommand);
        Console.WriteLine(tunnelMessage.ToString());

        await VrCommunication.SendAsJson(tunnelMessage);
        return await VrCommunication.ReceiveJsonObject();
    }


    /// <summary>
    ///     Add a model in VR scene
    /// </summary>
    /// <param name="name">name of node</param>
    /// <param name="position">position array containing x, y, z</param>
    /// <param name="scale">sets scaling of model</param>
    /// <param name="rotation"> rotates model in x-, y-, z-ax </param>
    /// <param name="fileName"> filepath of the obj file of the model</param>
    /// <param name="animationName"> filepath of the animation file</param>
    public async Task<JsonObject> AddAnimatedModel(string name, Vector3 position, double scale, Vector3 rotation,
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
    public async Task<JsonObject[]> PlaceTrees(int amount)
    {
        var jsonResponses = new JsonObject[amount]; // array to save json responses
        var random = new Random();

        // iterate over amount of trees and add them to the scene
        for (var i = 0; i < amount; i++)
        {
            // generate random x,z position
            float x = random.Next(-128, 128);
            float z = random.Next(-128, 128);
            var position = new Vector3(x, 0, z);
            position.Y = await GetHeight(position); // set height of model to height of terrain

            var modelAddCommand = Formatting.Add3DObject(
                $"tree{i}",
                position,
                1,
                @"data\NetworkEngine\models\trees\fantasy\tree7.obj"
            );

            var tunnelMessage = Formatting.TunnelSend(_tunnelId, modelAddCommand);
            await VrCommunication.SendAsJson(tunnelMessage);

            jsonResponses[i] = await VrCommunication.ReceiveJsonObject(); // save response in array
        }

        return jsonResponses; // return array of json responses
    }

    /// <summary>
    ///     Add terrain data to VR scene
    ///     Since its only positional data, no visual component is rendered in the scene
    /// </summary>
    public async Task<JsonObject> AddTerrain(int[] size, float[] heightMap)
    {
        var terrainAddCommand = Formatting.TerrainAdd(size, heightMap);
        var tunnelMessage = Formatting.TunnelSend(_tunnelId, terrainAddCommand);

        await VrCommunication.SendAsJson(tunnelMessage);
        return await VrCommunication.ReceiveJsonObject();
    }


    /// <summary>
    ///     Add terrain node to VR scene
    ///     Visual component (layers) can be added after this
    /// </summary>
    public async Task<JsonObject> AddTerrainNode()
    {
        var addTerrainNodeCommand = Formatting.TerrainAddNode();
        var tunnelMessage = Formatting.TunnelSend(_tunnelId, addTerrainNodeCommand);

        await VrCommunication.SendAsJson(tunnelMessage);
        return await VrCommunication.ReceiveJsonObject();
    }

    /// <summary>
    ///     Add visual component to terrain
    ///     Requires actual node to add visual component
    /// </summary>
    public async Task<JsonObject> AddTerrainLayer()
    {
        // command data
        var terrainId = await GetNodeId("terrain");
        var diffuseFilePath = @"data\NetworkEngine\textures\grass_diffuse.png";
        var normalFilePath = @"data\NetworkEngine\textures\grass_normal.png";

        // create command and send to VR
        var addTerrainLayerCommand = Formatting.TerrainAddLayer(terrainId, diffuseFilePath, normalFilePath);
        var tunnelMessage = Formatting.TunnelSend(_tunnelId, addTerrainLayerCommand);

        await VrCommunication.SendAsJson(tunnelMessage);
        return await VrCommunication.ReceiveJsonObject();
    }

    /// <summary>
    ///     Get uuid from node based on name
    /// </summary>
    /// <param name="name">name of the requested node</param>
    /// <returns>uuid as string</returns>
    public async Task<string> GetNodeId(string name)
    {
        object sceneFindNodeCommand = Formatting.SceneNodeFind(name);
        object tunnelMessage = Formatting.TunnelSend(_tunnelId, sceneFindNodeCommand);

        await VrCommunication.SendAsJson(tunnelMessage);
        JsonObject response = await VrCommunication.ReceiveJsonObject();
        Console.WriteLine(response);

        var responseData = response?["data"]?["data"]?["data"];



        // Response can be an array or a single object, this needs to be handled differently
        if (responseData is JsonArray)
        {
            if (responseData == null)
                throw new CommunicationException("Could not retrieve the nodes JsonArray from message.");

            return GetNode(responseData.AsArray());
        }
        else
        {

            // Response is just a single object, so retrieve value as string
            return response?["data"]?["id"].GetValue<string>();
        }

    }

    /// <summary>
    ///     Helper method to get the first node's UUID, or else an empty string.
    ///     It is assumed that the first node is the terrain node.
    ///     TODO check if the terrain is always the first node in this JsonArray
    /// </summary>
    private string GetNode(JsonArray nodes)
    {
        var uuid = string.Empty;
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

    /// <summary>
    /// Let an object follow the route
    /// </summary>
    public async Task<JsonObject> FollowRoute(string route, string node, double speed)
    {
        object routeFollowCommand = Formatting.RouteFollow(route, node, speed);
        object tunnelMessage = Formatting.TunnelSend(_tunnelId, routeFollowCommand);

        await VrCommunication.SendAsJson(tunnelMessage);
        return await VrCommunication.ReceiveJsonObject();
    }
}
