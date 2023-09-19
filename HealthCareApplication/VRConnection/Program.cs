using System.Numerics;
using VRConnection.Communication;
using VRConnection.Graphics;

namespace VRConnection;

public class Program
{
    private static async Task Main(string[] args)
    {
        VrSession session = new();

        try
        {
            await session.Initialize("145.48.6.10", 6666);

            // reset scene to default
            var reset = await session.ResetScene();
            Console.WriteLine(reset);

            // Opgave 3a/3e Voeg plat/ terrein toe
            var size = new[] { 256, 256 };
            // var heightMap = new float[256 * 256];
            var heightMap = PerlinNoiseGenerator.GenerateHeightMap(20); // TODO save heightMap as prop

            // Send the terrain to the server and receive the response
            var terrain = await session.AddTerrain(size, heightMap);
            Console.WriteLine(terrain);

            // Opgave 3d voeg een aantal 3d modellen toe aan de scene, op verschillende posities
            Vector3 position = new(0, 0, 0);
            var tree = await session.AddModel(
                "tree",
                position,
                1,
                @"data\NetworkEngine\models\trees\fantasy\tree7.obj"
            );
            Console.WriteLine(tree);

            var trees = await session.PlaceTrees(10);
            foreach (var t in trees) Console.WriteLine(t);


            // Opgave 3e Verander de code van 3a zodat het terrein hoogteverschillen krijgt

            var scene = await session.GetScene();
            Console.WriteLine(scene);

            var terrainNode = await session.AddTerrainNode();
            Console.WriteLine(terrainNode);

            var terrainLayer = await session.AddTerrainLayer();
            Console.WriteLine(terrainLayer);

                JsonObject setSkyboxObj = await session.SetSkyTime(23.5);
                Console.WriteLine(setSkyboxObj);

                session.Close();
            }
            catch (CommunicationException ex)
            {
                await Console.Out.WriteLineAsync($"CommunicationException: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}