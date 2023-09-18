using System.Diagnostics;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Text.Json.Nodes;
using VRConnection.Communication;
using VRConnection.Graphics;

namespace VRConnection
{
    public class Program
    {
        public static void ExecuteBatFile(string networkEnginePath)
        {
            Process proc = null;

            string _batDir = string.Format(networkEnginePath);
            proc = new Process();
            proc.StartInfo.WorkingDirectory = _batDir;
            proc.StartInfo.FileName = Path.Combine(networkEnginePath, "sim.bat");
            proc.StartInfo.CreateNoWindow = false;
            proc.Start();
            proc.WaitForExit();
            var exitCode = proc.ExitCode;
            proc.Close();
            Console.WriteLine("Bat file executed...");
        }

        static async Task Main(string[] args)
        {
            VrSession session = new();

            try
            {
                await session.Initialize("145.48.6.10", 6666);

                // Opgave 3a/3e Voeg plat/ terrein toe
                var size = new int[] { 256, 256 };
                // var heightMap = new float[256 * 256];
                var heightMap = PerlinNoiseGenerator.GenerateHeightMap(20); // TODO save heightMap as prop

                // Send the terrain to the server and receive the response
                JsonObject terrain = await session.AddTerrain(size, heightMap);
                Console.WriteLine(terrain);

                Vector3 position = new(0, 0, 0);
                // Opgave 3d voeg een aantal 3d modellen toe aan de scene, op verschillende posities
                JsonObject tree = await session.AddModel(
                    "tree",
                    // convert vector3 to float[]
                    position,
                    1,
                    @"data\NetworkEngine\models\trees\fantasy\tree7.obj"
                );
                Console.WriteLine(tree);


                // Opgave 3e Verander de code van 3a zodat het terrein hoogteverschillen krijgt

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