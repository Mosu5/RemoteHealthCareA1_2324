using PatientApp.VrLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Communication;

namespace PatientApp.VrLogic
{
    internal class VrProgram
    {
        public static async Task Run()
        {
            VrSession session = new VrSession();

            try
            {
                await VrSession.Initialize("145.48.6.10", 6666);
                await VrSession.ResetScene();

                // Opgave 3a Voeg een terrein toe
                // Opgave 3e Verander de code van 3a zodat het terrein hoogteverschillen krijgt
                int length = 256, width = 256;
                var size = new int[] { 256, 256 };
                int height = 5;

                float[] heightMap = PerlinNoiseGenerator.GenerateHeightMap(20); // TODO save heightMap as prop

                string terrainResponse = await VrSession.AddHillTerrain(length, width, new Vector3(-128, 0, -128), Vector3.Zero, height);
                //add hills around the flat area
                //int hillsWidth = 58, hillsLength = 256;

                //string terrainResponse1 = await VrSession.AddHillTerrain(100, 100, new Vector3(-70, 0, -70), Vector3.Zero, 0);


                //Console.WriteLine(terrainResponse);

                // Opgave 3b Verwijder de groundplane
                JsonObject removeGroundPane = await VrSession.RemoveNode("GroundPlane");
                Console.WriteLine(removeGroundPane);

                // Opgave 3c Verander de tijd van de skybox
                JsonObject setSkyboxObj = await VrSession.SetSkyTime(12.0);
                Console.WriteLine(setSkyboxObj);

                //update the skybox

                String rt = @"data\NetworkEngine\textures\SkyBoxes\clouds\bluecloud_rt.jpg";
                String lf = @"data\NetworkEngine\textures\SkyBoxes\clouds\bluecloud_lf.jpg";
                String up = @"data\NetworkEngine\textures\SkyBoxes\clouds\bluecloud_up.jpg";
                String dn = @"data\NetworkEngine\textures\SkyBoxes\clouds\bluecloud_dn.jpg";
                String bk = @"data\NetworkEngine\textures\SkyBoxes\clouds\bluecloud_bk.jpg";
                String ft = @"data\NetworkEngine\textures\SkyBoxes\clouds\bluecloud_ft.jpg";
                JsonObject skyboxUpdate = await VrSession.UpdateSkybox(rt, lf, up, dn, bk, ft);
                Console.WriteLine(skyboxUpdate);


                

                // Opgave 3d voeg een aantal 3d modellen toe aan de scene, op verschillende posities

                //trees
                JsonObject tree1 = await VrSession.AddModelOnTerrain(
                     "tree",
                     new Vector3(-30, 0, -1),
                     1.9,
                     @"data\NetworkEngine\models\trees\fantasy\tree7.obj",
                     0
                 );
                JsonObject tree2 = await VrSession.AddModelOnTerrain(
                     "tree",
                     new Vector3(-27, 0, -8),
                     1.5,
                     @"data\NetworkEngine\models\trees\fantasy\tree7.obj",
                     0
                 );
                JsonObject tree3 = await VrSession.AddModelOnTerrain(
                     "tree",
                     new Vector3(-31, 0, -19),
                     2,
                     @"data\NetworkEngine\models\trees\fantasy\tree7.obj",
                     0
                 );
                JsonObject tree4 = await VrSession.AddModelOnTerrain(
                     "tree",
                     new Vector3(-25, 0, -26),
                     2.1,
                     @"data\NetworkEngine\models\trees\fantasy\tree7.obj",
                     0
                 );
                JsonObject tree5 = await VrSession.AddModelOnTerrain(
                     "tree",
                     new Vector3(-30, 0, -34),
                     1.7,
                     @"data\NetworkEngine\models\trees\fantasy\tree7.obj",
                     0
                 );

                JsonObject tree6 = await VrSession.AddModelOnTerrain(
                     "tree",
                     new Vector3(-45, 0, 16),
                     1.7,
                     @"data\NetworkEngine\models\trees\fantasy\tree7.obj",
                     0
                 );

                //houses

                JsonObject house1 = await VrSession.AddModelOnTerrain(
                     "house",
                     new Vector3(-52, 0, 25),
                     9,
                     @"data\NetworkEngine\models\houses\set1\house6.obj",
                     90
                 );
                JsonObject house2 = await VrSession.AddModelOnTerrain(
                     "house",
                     new Vector3(-49, 0, -5),
                     9,
                     @"data\NetworkEngine\models\houses\set1\house7.obj",
                     90
                 );
                JsonObject house3 = await VrSession.AddModelOnTerrain(
                     "house",
                     new Vector3(-52, 0, -35),
                     9,
                     @"data\NetworkEngine\models\houses\set1\house6.obj",
                     90
                 );
                JsonObject house4 = await VrSession.AddModelOnTerrain(
                     "house",
                     new Vector3(0, 0, 50),
                     9,
                     @"data\NetworkEngine\models\houses\set1\house6.obj",
                     180
                 );
                JsonObject house5 = await VrSession.AddModelOnTerrain(
                     "house",
                     new Vector3(30, 0, 50),
                     9,
                     @"data\NetworkEngine\models\houses\set1\house7.obj",
                     180
                 );
                JsonObject house6 = await VrSession.AddModelOnTerrain(
                     "house",
                     new Vector3(20, 0, 28),
                     9,
                     @"data\NetworkEngine\models\houses\set1\house7.obj",
                     0
                 );
                JsonObject house7 = await VrSession.AddModelOnTerrain(
                     "house",
                     new Vector3(-30, 0, 28),
                     9,
                     @"data\NetworkEngine\models\houses\set1\house6.obj",
                     0
                 );
                JsonObject house8 = await VrSession.AddModelOnTerrain(
                     "house",
                     new Vector3(-10, 0, 28),
                     9,
                     @"data\NetworkEngine\models\houses\set1\house6.obj",
                     0
                 );
                JsonObject house9 = await VrSession.AddModelOnTerrain(
                     "house",
                     new Vector3(-28, 0, 10),
                     9,
                     @"data\NetworkEngine\models\houses\set1\house6.obj",
                     -90
                 );

                //cars

                JsonObject car1 = await VrSession.AddModelOnTerrain(
                     "car",
                     new Vector3(-35, 0, -11),
                     1.4,
                     @"data\NetworkEngine\models\cars\generic\black.obj",
                     0
                 );

                JsonObject car2 = await VrSession.AddModelOnTerrain(
                     "car",
                     new Vector3(-12, 0, 45),
                     1.4,
                     @"data\NetworkEngine\models\cars\generic\white.obj",
                     90
                 );


                //bike

                JsonObject bike = await VrSession.AddModelOnTerrain(
                    "bike",
                    new Vector3(0, 0, 0),
                    2,
                    @"data\NetworkEngine\models\bike\bike.fbx",
                    0
                );


                //JsonObject[] trees = await VrSession.RandomlyPlaceTrees(30);
                //foreach (var t in trees) Console.WriteLine(t);

                //  Vector3 position1 = new(0, 0, 0);
                //  JsonObject tree1 = await VrSession.AddModelOnTerrain(
                //      "tree",
                //      position,
                //      1,
                //      @"data\NetworkEngine\models\trees\fantasy\tree7.obj"
                //  );
                //  Console.WriteLine(tree);

                //  JsonObject[] trees = await VrSession.RandomlyPlaceTrees(10);
                //  foreach (var t in trees) Console.WriteLine(t);

                // Opgave 3f Voeg route toe
                PosVector[] posVectors = new PosVector[]
                {
                new PosVector(new int[] { -40, 0, -40 }, new int[] { 5, 0, 5 }),
                new PosVector(new int[] { -40, 0, 40 }, new int[] { 5, 0, -5 }),
                new PosVector(new int[] { 40, 0, 40 }, new int[] { -5, 0, 5 }),
                new PosVector(new int[] { 40, 0, -40 }, new int[] { -5, 0, -5 })
                };

                JsonObject route = await VrSession.AddRoute(posVectors);
                Console.WriteLine(route);

                // Opgave 3h Laat een 3D opbject de route volgen met een gegeven snelheid
                string bikeID = await VrSession.GetNodeId("bike");
                string routeID = VrSession.GetRouteId(route);
                double speed = 7.0;
                JsonObject bikeFollowingRoute = await VrSession.FollowRoute(routeID, bikeID, speed);
                Console.WriteLine(bike);



                // Opgave 3f Voeg route toe
                JsonObject road = await VrSession.AddRoad(routeID);
                Console.WriteLine(road);

                // Head op de fiets
                string headID = await VrSession.GetNodeId("Camera");
                JsonObject headOnBike = await VrSession.HeadOnBike(headID, bikeID);
                Console.WriteLine(headOnBike);


                JsonObject getScene = await VrSession.GetScene();
                Console.WriteLine(getScene);

                // Voegt een Panel toe
                JsonObject panel = await VrSession.AddPanel();
                Console.WriteLine(panel);

                // Cleart Panel
                JsonObject clear = await VrSession.ClearPanel();
                Console.WriteLine(clear);

                // Set clear color Panel
                JsonObject setcolor = await VrSession.setColorPanel();
                Console.WriteLine(setcolor);


                //using (StreamReader streamReader = new StreamReader(@"mytest1.json"))
                //{
                //    string line;
                //    int times = 0;
                //    string text2 = "";
                //    while ((line = streamReader.ReadLine()) != null)
                //    {
                //        // Cleart Panel
                //        JsonObject clear2 = await VrSession.ClearPanel();
                //        Console.WriteLine(clear2);
                //        
                //        // Deserialize the JSON data from the current line
                //        var jsonDocument = JsonDocument.Parse(line);
                //        var root = jsonDocument.RootElement;
                //
                //        // Access the properties "DataType" and "Value"
                //        if (root.TryGetProperty("DataType", out var dataType) && root.TryGetProperty("Value", out var value))
                //        {
                //            text2 = $"DataType: {dataType.GetString()}, Value: {value}";
                //        }
                //        
                //        //Voeg text aan Panel toe
                //        JsonObject text = await VrSession.AddText(text2);
                //        Console.WriteLine(text);
                //
                //        Console.WriteLine(text2);
                //        
                //        // Swapt Panel
                //        JsonObject swap = await VrSession.SwapPanel();
                //        Console.WriteLine(swap);
                //    }
                //    
                //}
                //
                //
                // Not strictly necessary, but looks clean

                //Boolean boolean = true;
                //while (true)
                //{
                //    Thread.Sleep(500);
                //    if (boolean)
                //    {
                //        speed += 0.1;
                //        if(speed > 20)
                //        {
                //            boolean = false;
                //        }
                //    }
                //    else
                //    {
                //        speed -= 0.1;
                //        if(speed < 1)
                //        {
                //            boolean = true;
                //        }
                //    }
                //    // Cleart Panel
                //    JsonObject clear2 = await VrSession.ClearPanel();
                //    Console.WriteLine(clear2);
                                        
                //    //Voeg text aan Panel toe
                //    JsonObject text = await VrSession.AddText(speed+"");
                //    JsonObject swap = await VrSession.SwapPanel();

                //    JsonObject updateSpeed = await VrSession.UpdateSpeed(bikeID, speed);
                //}
            }
            catch (CommunicationException ex)
            {
                await Console.Out.WriteLineAsync($"CommunicationException: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public static async Task UpdateBikeSpeed(double speed)
        {
            // Cleart Panel
            JsonObject clear2 = await VrSession.ClearPanel();
            Console.WriteLine(clear2);

            //Voeg text aan Panel toe
            JsonObject text = await VrSession.AddText(speed + "");
            JsonObject swap = await VrSession.SwapPanel();

            string bikeId = await VrSession.GetNodeId("bike");
            await VrSession.UpdateSpeed(bikeId, speed);
        }
    }
}
