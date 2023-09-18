using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using VRConnection.Communication;

namespace VRConnection
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            VrSession session = new();

            try
            {
                await session.Initialize("145.48.6.10", 6666);

                var size = new int[] { 256, 256 };
                var heightMap = new float[65536];

                // Opgave 3a Voeg plat terrein toe
                JsonObject terrain = await session.AddTerrain(size, heightMap);
                Console.WriteLine(terrain);

                JsonObject scene = await session.GetScene();
                Console.WriteLine(scene);

                JsonObject terrainNode = await session.AddTerrainNode();
                Console.WriteLine(terrainNode);

                JsonObject terrainLayer = await session.AddTerrainLayer();
                Console.WriteLine(terrainLayer);

                session.Close();
            }
            catch (CommunicationException ex)
            {
                await Console.Out.WriteLineAsync($"CommunicationException: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}