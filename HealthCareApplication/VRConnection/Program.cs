using System.Diagnostics;
using System.Net.Sockets;
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

                var size = new int[] { 256, 256 };
                var heightMap = new float[65536];
                
                PosVector[] posVectors = new PosVector[]
                {
                    new PosVector(new int[]{-22,0,40 }, new int[]{5,0,5}),
                    new PosVector(new int[]{0,0,62}, new int[]{5,0,5}),
                    new PosVector(new int[]{42,0, 63}, new int[]{5,0,-5}),
                    new PosVector(new int[]{65,0,42 }, new int[]{5,0,-5}),
                    new PosVector(new int[]{75,0,10 }, new int[]{5,0,-5}),
                    new PosVector(new int[]{63,0,-30 }, new int[]{-5,0,5}),
                    new PosVector(new int[]{20,0,-40 }, new int[]{-5,0,-5}),
                    new PosVector(new int[]{-10,0,-30 }, new int[]{-5,0,5}),
                    new PosVector(new int[]{-25,0,-5 }, new int[]{-5,0,5})
                };
                
                // Opgave 3f Voeg route toe
                JsonObject route = await session.AddRoute(posVectors);
                Console.WriteLine(route);

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