using System.Numerics;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using VRConnection.Communication;
using VRConnection.Graphics;
using System;
using System.IO;
using System.Text.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace VRConnection;

public class Program
{
    private static async Task Main(string[] args)
    {
        VrSession session = new();

        try
        {
            await session.Initialize("145.48.6.10", 6666);
            await session.ResetScene();

            // Opgave 3a Voeg een terrein toe
            // Opgave 3e Verander de code van 3a zodat het terrein hoogteverschillen krijgt
            int length = 256, width = 256;
            var size = new int[] { 256, 256 };
            float[] heightMap = PerlinNoiseGenerator.GenerateHeightMap(20); // TODO save heightMap as prop

            // string terrainResponse = await session.AddHillTerrain(length, width, new Vector3(-128, 0, -128), Vector3.Zero);
            //Console.WriteLine(terrainResponse);

            // Opgave 3b Verwijder de groundplane
            JsonObject removeGroundPane = await session.RemoveNode("GroundPlane");
            Console.WriteLine(removeGroundPane);

            // Opgave 3c Verander de tijd van de skybox
            JsonObject setSkyboxObj = await session.SetSkyTime(12.5);
            Console.WriteLine(setSkyboxObj);

            // Opgave 3d voeg een aantal 3d modellen toe aan de scene, op verschillende posities

            // Vector3 position = new(0, 0, 0);
            // JsonObject tree = await session.AddModelOnTerrain(
            //      "tree",
            //      position,
            //      1,
            //      @"data\NetworkEngine\models\trees\fantasy\tree7.obj"
            //  );
            //  Console.WriteLine(tree);


            //  Vector3 position1 = new(0, 0, 0);
            //  JsonObject tree1 = await session.AddModelOnTerrain(
            //      "tree",
            //      position,
            //      1,
            //      @"data\NetworkEngine\models\trees\fantasy\tree7.obj"
            //  );
            //  Console.WriteLine(tree);

            //  JsonObject[] trees = await session.RandomlyPlaceTrees(10);
            //  foreach (var t in trees) Console.WriteLine(t);

            // Opgave 3f Voeg route toe
            PosVector[] posVectors = new PosVector[]
            {
                new PosVector(new int[] { -22, 0, 40 }, new int[] { 5, 0, 5 }),
                new PosVector(new int[] { 0, 0, 62 }, new int[] { 5, 0, 5 }),
                new PosVector(new int[] { 42, 0, 63 }, new int[] { 5, 0, -5 }),
                new PosVector(new int[] { 65, 0, 42 }, new int[] { 5, 0, -5 }),
                new PosVector(new int[] { 75, 0, 10 }, new int[] { 5, 0, -5 }),
                new PosVector(new int[] { 63, 0, -30 }, new int[] { -5, 0, 5 }),
                new PosVector(new int[] { 20, 0, -40 }, new int[] { -5, 0, -5 }),
                new PosVector(new int[] { -10, 0, -30 }, new int[] { -5, 0, 5 }),
                new PosVector(new int[] { -25, 0, -5 }, new int[] { -5, 0, 5 })
            };

            JsonObject route = await session.AddRoute(posVectors);
            Console.WriteLine(route);

            // Opgave 3h Laat een 3D opbject de route volgen
            string treeID = await session.GetNodeId("tree");
            string routeID = session.GetRouteId(route);
            JsonObject bike = await session.FollowRoute(routeID, treeID, 20.0);
            Console.WriteLine(bike);

      

            // Opgave 3f Voeg route toe
            JsonObject road = await session.AddRoad(routeID);
            Console.WriteLine(road);

            // Voegt een Panel toe
            JsonObject panel = await session.AddPanel();
            Console.WriteLine(panel);
            
            // Cleart Panel
            JsonObject clear = await session.ClearPanel();
            Console.WriteLine(clear);

            // Set clear color Panel
            JsonObject setcolor = await session.setColorPanel();
            Console.WriteLine(setcolor);


            using (StreamReader streamReader = new StreamReader(@"C:\temp\mytest1.json"))
            {
                string line;
                int times = 0;
                string text2 = "";
                while ((line = streamReader.ReadLine()) != null)
                {
                    // Cleart Panel
                    JsonObject clear2 = await session.ClearPanel();
                    Console.WriteLine(clear2);
                    
                    // Deserialize the JSON data from the current line
                    var jsonDocument = JsonDocument.Parse(line);
                    var root = jsonDocument.RootElement;

                    // Access the properties "DataType" and "Value"
                    if (root.TryGetProperty("DataType", out var dataType) && root.TryGetProperty("Value", out var value))
                    {
                        text2 = $"DataType: {dataType.GetString()}, Value: {value}";
                    }
                    
                    //Voeg text aan Panel toe
                    JsonObject text = await session.AddText(text2);
                    Console.WriteLine(text);

                    Console.WriteLine(text2);
                    
                    // Swapt Panel
                    JsonObject swap = await session.SwapPanel();
                    Console.WriteLine(swap);
                }
                
            }
            

            // Not strictly necessary, but looks clean
            session.Close();
        }
        catch (CommunicationException ex)
        {
            await Console.Out.WriteLineAsync($"CommunicationException: {ex.Message}\n{ex.StackTrace}");
        }
    }
}