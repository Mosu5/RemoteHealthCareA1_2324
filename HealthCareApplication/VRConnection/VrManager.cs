using System.Text.Json;

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
}