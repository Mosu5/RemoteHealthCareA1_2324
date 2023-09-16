using System.Text.Json;
using System.Text.Json.Nodes;

namespace VRConnection;

public class VrManager
{
    public VrManager(TunnelHandler tunnelHandler)
    {
        _tunnelHandler = tunnelHandler;
        tunnelHandler.CreateTunnel();
        if (tunnelHandler.TunnelId == "")
        {
            Console.WriteLine("Failed to create tunnel.");
        }
    }

    private readonly TunnelHandler _tunnelHandler;

    public void GetScene()
    {
        object sceneGetCommand = Formatting.SceneGet();
        // TODO create separate tunnel message
        object tunnelMessage = Formatting.TunnelSend(_tunnelHandler.TunnelId, sceneGetCommand);

        _tunnelHandler.SendMessage(tunnelMessage);
    }

    public void AddTerrain(int[] size, float[] heightMap)
    {
        object terrainAddCommand = Formatting.TerrainAdd(size, heightMap);
        object tunnelMessage = Formatting.TunnelSend(_tunnelHandler.TunnelId, terrainAddCommand);

        _tunnelHandler.SendMessage(tunnelMessage);
    }

    public void AddTerrainNode()
    {
        object addTerrainNodeCommand = Formatting.TerrainAddNode();
        object tunnelMessage = Formatting.TunnelSend(_tunnelHandler.TunnelId, addTerrainNodeCommand);

        _tunnelHandler.SendMessage(tunnelMessage);
    }

    public void AddTerrainLayer()
    {
        string terrainId = GetNodeId("terrain");
        string diffuseFilePath = "data\\NetworkEngine\\textures\\grass_diffuse.png";
        string normalFilePath = "data\\NetworkEngine\\textures\\grass_normal.png";
        object addTerrainLayerCommand = Formatting.TerrainAddLayer(terrainId, diffuseFilePath, normalFilePath);
        object tunnelMessage = Formatting.TunnelSend(_tunnelHandler.TunnelId, addTerrainLayerCommand);
        _tunnelHandler.SendMessage(tunnelMessage);
        
    }

    public string GetNodeId(string name)
    {
        object sceneFindNodeCommand = Formatting.SceneNodeFind(name);
        object tunnelMessage = Formatting.TunnelSend(_tunnelHandler.TunnelId, sceneFindNodeCommand);

        _tunnelHandler.SendMessage(tunnelMessage);
        var response = _tunnelHandler.ReadJsonObject();
        
        
        var nodes = response?["data"]?["data"]?["data"].AsArray();

        string uuid = string.Empty;
        if (nodes != null)
        {
            var node = nodes.First();
            uuid = node?["uuid"]?.ToString() ?? string.Empty;
        }
        
        return uuid;
    }
}