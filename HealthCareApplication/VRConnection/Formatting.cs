using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRConnection
{
    internal class Formatting
    {
        public static object TunnelAdd(string sessionId)
        {
            return new
            {
                id = "tunnel/create",
                data = new
                {
                    session = sessionId,
                    key = "muffins"
                }
            };
        }

        public static object TunnelSend(string tunnelId, object commandData)
        {
            return new
            {
                id = "tunnel/send",
                data = new
                {
                    dest = tunnelId,
                    data = commandData
                }
            };
        }


        public static object GetSessionList()
        {
            return new
            {
                id = "session/list"
            };
        }


        public static object TerrainAdd(int[] size, float[] heights)
        {
            return new
            {
                id = "scene/terrain/add",
                data = new
                {
                    size,
                    heights
                }
            };
        }


        public static object TerrainDelete()
        {
            return new
            {
                id = "scene/terrain/delete",
                data = new
                {
                }
            };
        }


        public static object SkyBoxSetTime(double time)
        {
            return new
            {
                id = "scene/skybox/settime",
                data = new
                {
                    time
                }
            };
        }


        public static object Add3DObject(string name, string fileName, Transform transform)
        {
            return new
            {
                id = "scene/node/add",
                data = new
                {
                    name,
                    components = new
                    {
                        transform,
                        model = new
                        {
                            fileName
                        }
                    }
                }
            };
        }


        public static object TerrainHeightUpdate(int[] heights)
        {
            return new
            {
                id = "scene/update/update",
                data = new
                {
                    heights
                }
            };
        }


        public static object RouteAdd(PosVector[] nodes)
        {
            return new
            {
                id = "route/add",
                data = new
                {
                    nodes
                }
            };
        }


        public static object RoadAdd(string route, string diffuse, string normal, string specular)
        {
            return new
            {
                id = "scene/road/add",
                data = new
                {
                    route,
                    diffuse,
                    normal,
                    specular,
                    heightoffset = 0.01
                }
            };
        }

        public static object RouteFollow(string route, string node, double speed)
        {
            return new
            {
                id = "route/follow",
                data = new
                {
                    route,
                    node,
                    speed,
                    offset = 0.0,
                    rotate = "XZ",
                    smoothing = 1.0,
                    followHeight = false,
                    rotateOffset = new int[0, 0, 0],
                    positionOffset = new int[0, 0, 0]
                }
            };
        }


        public static object RouteUpdate(string node, double speed)
        {
            return new
            {
                id = "route/update",
                data = new
                {
                    node,
                    speed
                }
            };
        }
    }
}