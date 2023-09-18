using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Diagnostics;

namespace VRConnection
{
    public class Program
    {


    public static void ExecuteBatFile()
    {
        Process proc = null;

        string _batDir = string.Format(@"C:\Users\moustapha\Downloads\NetworkEngine.21.10.13\NetworkEngine");
        proc = new Process();
        proc.StartInfo.WorkingDirectory = _batDir;
        proc.StartInfo.FileName = @"C:\Users\moustapha\Downloads\NetworkEngine.21.10.13\NetworkEngine\sim.bat";
        proc.StartInfo.CreateNoWindow = false;
        proc.Start();
        proc.WaitForExit();
        var exitCode = proc.ExitCode;
        proc.Close();
        Console.WriteLine("Bat file executed...");
    }
        public static void Main(string[] args)
        {
            // Executing NetworkEngine.exe for testing purposes
            new Thread(ExecuteBatFile).Start();
            Thread.Sleep(1000);
            // Application to test connection to VR server



            // Initialize Server connection
            TcpClient tcpClient = new TcpClient("145.48.6.10", 6666);
            NetworkStream networkStream = tcpClient.GetStream();

            TunnelHandler tunnelHandler = new TunnelHandler(networkStream);
            // tunnelHandler.CreateTunnel();
            VrManager vrManager = new VrManager(tunnelHandler);
            var size = new int[2] { 256, 256 };
            var heightMap = new float[65536];
            vrManager.AddTerrain(size, heightMap);
            Console.WriteLine(tunnelHandler.ReadString()); // TODO implement a way to limit duplicate code
            // when reading response


            vrManager.AddTerrainNode();
            Console.WriteLine(tunnelHandler.ReadString());


            vrManager.AddTerrainLayer();
            Console.WriteLine(tunnelHandler.ReadString());

            var fileName = @"data\NetworkEngine\models\cars\cartoon\Pony_cartoon.obj";
            int[] pos = { 0, 0, 0 };
            double scale = 0.010f;
            vrManager.AddModel("testTree", pos, scale, fileName);
            Console.WriteLine(tunnelHandler.ReadString());
            vrManager.GetScene();
            Console.WriteLine(tunnelHandler.ReadString());

            //
            // bool running = true;
            //
            // while (running)
            // {
            //     Console.Write("Write message: ");
            //     string message = Console.ReadLine();
            //     if (message.Equals("quit"))
            //     {
            //         running = false;
            //     }
            //
            //     TunnelHandler.SendMessage(networkStream, message);
            //
            //     string response = TunnelHandler.ReadMessage(networkStream);
            //     Console.WriteLine("Response:" + JsonObject.Parse(response));
            //
            // }
            tcpClient.Close();
            networkStream.Close();
        }
    }
}