using System.Numerics;
using System.Text.Json.Nodes;
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

            Vector3 position1 = new(0, 0, 0);
            var tree1 = await session.AddModel(
                "tree",
                position,
                1,
                @"data\NetworkEngine\models\trees\fantasy\tree7.obj"
            );
            Console.WriteLine(tree);

            PosVector[] posVectors = new PosVector[]
                {
                    new PosVector(new int[]{0,0,0}, new int[]{5,0,-5}),
                    new PosVector(new int[]{50,0,0}, new int[]{5,0,5}),
                    new PosVector(new int[]{50,0,50}, new int[]{-5,0,5}),
                    new PosVector(new int[]{0,0,50}, new int[]{-5,0,-5})
                };

            // Opgave 3f Voeg route toe
            JsonObject route = await session.AddRoute(posVectors);
            Console.WriteLine(route);

            // Opgave 3h Laat een 3D opbject de route volgen
            String treeID = await session.GetNodeId("tree");
            String routeID = route["data"]["data"]["data"]["uuid"].GetValue<String>();
            JsonObject bike = await session.FollowRoute(routeID, treeID, 20.0);
            Console.WriteLine(bike);

            var trees = await session.PlaceTrees(10);
            foreach (var t in trees) Console.WriteLine(t);


            // Opgave 3e Verander de code van 3a zodat het terrein hoogteverschillen krijgt

            var scene = await session.GetScene();
            Console.WriteLine(scene);

            var terrainNode = await session.AddTerrainNode();
            Console.WriteLine(terrainNode);
            
            var terrainLayer = await session.AddTerrainLayer();
            Console.WriteLine(terrainLayer);
            JsonObject removeGroundPane = await session.RemoveNode("GroundPlane");
            Console.WriteLine(removeGroundPane);
            session.Close();
        }
        catch (CommunicationException ex)
        {
            await Console.Out.WriteLineAsync($"CommunicationException: {ex.Message}\n{ex.StackTrace}");
        }
    }
}