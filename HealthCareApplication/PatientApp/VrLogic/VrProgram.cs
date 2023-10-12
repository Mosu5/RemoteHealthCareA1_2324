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
using Utilities.Logging;

namespace PatientApp.VrLogic
{
    internal class VrProgram
    {
        private static double _initialBikeSpeed = 7.0;

        /// <summary>
        /// Builds the entire VR environment. Takes a while to complete.
        /// </summary>
        public static async Task Initialize()
        {
            await VrSession.Initialize("145.48.6.10", 6666);
            await VrSession.ResetScene();

            // Add a terrain with hills
            int length = 256, width = 256, height = 5;
            string terrainResponse = await VrSession.AddHillTerrain(length, width, new Vector3(-128, 0, -128), Vector3.Zero, height);
            Logger.Log($"Added Terrain: {terrainResponse}", LogType.VrInfo);

            // Remove the ground plane
            JsonObject removeGroundPlane = await VrSession.RemoveNode("GroundPlane");
            Logger.Log($"Removed ground plane: {removeGroundPlane}", LogType.VrInfo);

            string rt = @"data\NetworkEngine\textures\SkyBoxes\clouds\bluecloud_rt.jpg";
            string lf = @"data\NetworkEngine\textures\SkyBoxes\clouds\bluecloud_lf.jpg";
            string up = @"data\NetworkEngine\textures\SkyBoxes\clouds\bluecloud_up.jpg";
            string dn = @"data\NetworkEngine\textures\SkyBoxes\clouds\bluecloud_dn.jpg";
            string bk = @"data\NetworkEngine\textures\SkyBoxes\clouds\bluecloud_bk.jpg";
            string ft = @"data\NetworkEngine\textures\SkyBoxes\clouds\bluecloud_ft.jpg";
            JsonObject skyboxUpdate = await VrSession.UpdateSkybox(rt, lf, up, dn, bk, ft);
            Logger.Log($"Changed skybox: {skyboxUpdate}", LogType.VrInfo);

            //==== Set up player, panel and route

            // Add bike
            await VrSession.AddModelOnTerrain(
                "bike",
                new Vector3(0, 0, 0),
                2,
                @"data\NetworkEngine\models\bike\bike.fbx",
                0
            );
            Logger.Log("Added bike", LogType.VrInfo);

            // Add a route
            PosVector[] posVectors = new PosVector[]
            {
                    new PosVector(new int[] { -40, 0, -40 }, new int[] { 5, 0, 5 }),
                    new PosVector(new int[] { -40, 0, 40 }, new int[] { 5, 0, -5 }),
                    new PosVector(new int[] { 40, 0, 40 }, new int[] { -5, 0, 5 }),
                    new PosVector(new int[] { 40, 0, -40 }, new int[] { -5, 0, -5 })
            };
            JsonObject route = await VrSession.AddRoute(posVectors);
            Logger.Log($"Added a route: {route}", LogType.VrInfo);

            // Make the camera fixed to bike
            string bikeID = await VrSession.GetNodeId("bike");
            string headID = await VrSession.GetNodeId("Camera");
            JsonObject headOnBike = await VrSession.HeadOnBike(headID, bikeID);
            Logger.Log($"Fixed camera to bike: {headOnBike}", LogType.VrInfo);

            // Make the bike follow the route
            string routeID = VrSession.GetRouteId(route);
            JsonObject bikeFollowingRoute = await VrSession.FollowRoute(routeID, bikeID, _initialBikeSpeed);
            Logger.Log($"Bike now follows route: {bikeFollowingRoute}", LogType.VrInfo);

            // Add panel
            JsonObject panel = await VrSession.AddPanel(bikeID);
            Logger.Log($"Added a panel: {panel}", LogType.VrInfo);

            // Clear panel
            JsonObject clear = await VrSession.ClearPanel();

            // Set clear color panel
            JsonObject setcolor = await VrSession.SetColorPanel();

            // Add text to panel
            JsonObject text = await VrSession.AddText("Speed: " + Math.Round(_initialBikeSpeed * 3.6, 1) + " km/h");
            JsonObject swap = await VrSession.SwapPanel();

            //==== Add 3d models like trees and houses

            // Add road
            JsonObject road = await VrSession.AddRoad(routeID);
            Logger.Log($"Added a road: {road}", LogType.VrInfo);

            // Add trees
            await VrSession.AddModelOnTerrain(
                 "tree",
                 new Vector3(-30, 0, -1),
                 1.9,
                 @"data\NetworkEngine\models\trees\fantasy\tree7.obj",
                 0
            );
            await VrSession.AddModelOnTerrain(
                 "tree",
                 new Vector3(-27, 0, -8),
                 1.5,
                 @"data\NetworkEngine\models\trees\fantasy\tree7.obj",
                 0
            );
            await VrSession.AddModelOnTerrain(
                 "tree",
                 new Vector3(-31, 0, -19),
                 2,
                 @"data\NetworkEngine\models\trees\fantasy\tree7.obj",
                 0
            );
            await VrSession.AddModelOnTerrain(
                 "tree",
                 new Vector3(-25, 0, -26),
                 2.1,
                 @"data\NetworkEngine\models\trees\fantasy\tree7.obj",
                 0
            );
            await VrSession.AddModelOnTerrain(
                 "tree",
                 new Vector3(-30, 0, -34),
                 1.7,
                 @"data\NetworkEngine\models\trees\fantasy\tree7.obj",
                 0
            );
            await VrSession.AddModelOnTerrain(
                 "tree",
                 new Vector3(-45, 0, 16),
                 1.7,
                 @"data\NetworkEngine\models\trees\fantasy\tree7.obj",
                 0
            );

            // Add houses
            await VrSession.AddModelOnTerrain(
                 "house",
                 new Vector3(-52, 0, 25),
                 9,
                 @"data\NetworkEngine\models\houses\set1\house6.obj",
                 90
            );
            await VrSession.AddModelOnTerrain(
                 "house",
                 new Vector3(-49, 0, -5),
                 9,
                 @"data\NetworkEngine\models\houses\set1\house7.obj",
                 90
            );
            await VrSession.AddModelOnTerrain(
                 "house",
                 new Vector3(-52, 0, -35),
                 9,
                 @"data\NetworkEngine\models\houses\set1\house6.obj",
                 90
            );
            await VrSession.AddModelOnTerrain(
                 "house",
                 new Vector3(0, 0, 50),
                 9,
                 @"data\NetworkEngine\models\houses\set1\house6.obj",
                 180
            );
            await VrSession.AddModelOnTerrain(
                 "house",
                 new Vector3(30, 0, 50),
                 9,
                 @"data\NetworkEngine\models\houses\set1\house7.obj",
                 180
            );
            await VrSession.AddModelOnTerrain(
                 "house",
                 new Vector3(20, 0, 28),
                 9,
                 @"data\NetworkEngine\models\houses\set1\house7.obj",
                 0
            );
            await VrSession.AddModelOnTerrain(
                 "house",
                 new Vector3(-30, 0, 28),
                 9,
                 @"data\NetworkEngine\models\houses\set1\house6.obj",
                 0
            );
            await VrSession.AddModelOnTerrain(
                 "house",
                 new Vector3(-10, 0, 28),
                 9,
                 @"data\NetworkEngine\models\houses\set1\house6.obj",
                 0
            );
            await VrSession.AddModelOnTerrain(
                 "house",
                 new Vector3(-28, 0, 10),
                 9,
                 @"data\NetworkEngine\models\houses\set1\house6.obj",
                 -90
            );

            // Add cars
            await VrSession.AddModelOnTerrain(
                 "car",
                 new Vector3(-35, 0, -11),
                 1.4,
                 @"data\NetworkEngine\models\cars\generic\black.obj",
                 0
             );
            await VrSession.AddModelOnTerrain(
                 "car",
                 new Vector3(-12, 0, 45),
                 1.4,
                 @"data\NetworkEngine\models\cars\generic\white.obj",
                 90
             );
        }

        /// <summary>
        /// Update the speed at which the bike is driving
        /// </summary>
        public static async Task UpdateBikeSpeed(double speed)
        {
            // Clear Panel
            await VrSession.ClearPanel();

            // Add text to panel
            JsonObject text = await VrSession.AddText("Speed: " + Math.Round(speed * 3.6, 1) + "km/h");
            JsonObject swap = await VrSession.SwapPanel();

            string bikeId = await VrSession.GetNodeId("bike");
            await VrSession.UpdateSpeed(bikeId, speed);
        }
    }
}
