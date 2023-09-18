using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using VRConnection.Graphics;

namespace VRConnection.Communication
{
    /// <summary>
    /// Helper for creating objects to be parsed as JSON messages and validating incoming JSON messages.
    /// </summary>
    public class Formatting
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


        public static object SessionListGet()
        {
            return new
            {
                id = "session/list"
            };
        }


        public static object SceneGet()
        {
            return new
            {
                id = "scene/get"
            };
        }

        public static object SceneNodeFind(string name)
        {
            return new
            {
                id = "scene/node/find",
                data = new
                {
                    name
                }
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


        public static object Add3DObject(string name, int[] position, double scale, string fileName)
        {
            return new
            {
                id = "scene/node/add",
                data = new
                {
                    name,
                    components = new
                    {
                        transform = new
                        {
                            position,
                            scale,
                            rotation = new[] { 0, 0, 0 }
                        },
                        model = new
                        {
                            file = fileName
                        }
                    }
                }
            };
        }

        public static object AddAnimatedObject(string name, int[] position, double scale, string fileName, string animationName)
        {
            return new
            {
                id = "scene/node/add",
                data = new
                {
                    name,
                    components = new
                    {
                        transform = new
                        {
                            position,
                            scale = scale,
                            rotation = new[] { 0, 0, 0 }
                        },
                        model = new
                        {
                            file = fileName,
                            animated = true,
                            animation = animationName
                        }
                    }
                }
            };
        }

        public static object TerrainAddNode()
        {
            return new
            {
                id = "scene/node/add",
                data = new
                {
                    name = "terrain",
                    components = new
                    {
                        transform = new
                        {
                            position = new[] { -128, 0, -128 },
                            scale = 1,
                            rotation = new[] { 0, 0, 0 }
                        },
                        terrain = new
                        {
                            smoothnormals = true
                        },
                    }
                }
            };
        }

        public static object TerrainAddLayer(string nodeId, string diffuseFilePath, string normalFilePath)
        {
            return new
            {
                id = "scene/node/addlayer",
                data = new
                {
                    id = nodeId,
                    diffuse = diffuseFilePath,
                    normal = normalFilePath,
                    minHeight = -100,
                    maxHeight = 100,
                    fadeDist = 1
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

        public static string ValidateAndGetSessionId(JsonObject serverResponse)
        {
            if (serverResponse == null ||
                !HasValidIdAndData(serverResponse) ||
                !(serverResponse["data"] is JsonArray))
            {
                throw new CommunicationException("Received invalid data.");
            }

            JsonArray sessionList = serverResponse["data"].AsArray();
            string? sessionId = null;

            foreach (var session in sessionList)
            {
                string? hostName = session?["clientinfo"]?["host"]?.ToString();
                if (hostName == null || session?["id"] == null) continue;

                if (hostName.Equals(Environment.MachineName, StringComparison.CurrentCultureIgnoreCase))
                    sessionId = session?["id"]?.ToString();
            }

            if (sessionId != null) return sessionId;

            throw new CommunicationException("Could not retrieve session ID from this message. Are you actually running NetworkEngine?");
        }

        public static string ValidateAndGetTunnelId(JsonObject serverResponse)
        {
            if (serverResponse == null ||
                !HasValidIdAndData(serverResponse) ||
                !(serverResponse is JsonObject))
            {
                throw new CommunicationException("Received invalid data.");
            }

            JsonObject payload = serverResponse["data"].AsObject();
            if (!payload.ContainsKey("status") ||
                !payload.ContainsKey("id") ||
                !(payload["status"] is JsonValue) ||
                !(payload["id"] is JsonValue))
            {
                throw new CommunicationException("Received invalid status and/or id field.");
            }

            if (payload["status"].ToString() != "ok")
            {
                throw new CommunicationException($"The response has a status of {payload["status"]}.");
            }

            string tunnelId = payload["id"].ToString();
            return tunnelId;
        }

        private static bool HasValidIdAndData(JsonObject data)
        {
            return
                data.ContainsKey("id") &&
                data.ContainsKey("data") &&
                data["id"] is JsonValue;
        }
    }
}