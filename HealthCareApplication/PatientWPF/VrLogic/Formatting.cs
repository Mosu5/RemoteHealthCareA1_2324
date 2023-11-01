using Newtonsoft.Json.Linq;
using PatientApp.VrLogic.Graphics;
using Utilities;
using System;
using System.Linq;
using System.Numerics;
using Utilities.Communication;

namespace PatientApp.VrLogic
{
    /// <summary>
    ///     Helper for creating objects to be parsed as JSON messages and validating incoming JSON messages.
    /// </summary>
    public class Formatting
    {
        #region Connectivity
        public static object SessionListGet()
        {
            return new
            {
                id = "session/list"
            };
        }

        public static object TunnelCreate(string sessionId)
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
        #endregion

        #region Scene

        public static object SceneSave()
        {
            return new
            {
                id = "scene/save",
                data = new
                {
                    filename = "rh_a1_vr_environment.json",
                    overwrite = true
                }
            };
        }

        public static object SceneLoad()
        {
            return new
            {
                id = "scene/load",
                data = new
                {
                    filename = "rh_a1_vr_environment.json"
                }
            };
        }

        public static object SceneGet()
        {
            return new
            {
                id = "scene/get"
            };
        }

        public static object SceneReset()
        {
            return new
            {
                id = "scene/reset"
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

        /// <summary> 
        /// Update node in VR scene (use to put the head on the bike)
        /// </summary>
        /// <param name="id"> Node ID </param>
        /// <param name="parent"> Parent Node ID </param>
        /// <returns> Object for Json </returns>
        public static object SceneNodeUpdate(string id, string parent)
        {
            return new
            {
                id = "scene/node/update",
                data = new
                {
                    id,
                    parent,
                    transform = new
                    {
                        position = new int[] { 0, 0, 0 },
                        scale = 1.0,
                        rotation = new[] { 0, 90, 0 },
                    },

                }
            };
        }
        /// <summary> 
        /// Update node in VR scene (use to put the head on the bike)
        /// </summary>
        /// <param name="id"> Node ID </param>
        /// <param name="rotation"> Rotation of object </param>
        /// <returns> Object for Json </returns>
        public static object SceneNodeUpdate(string id, Vector3 rotation)
        {
            return new
            {
                id = "scene/node/update",
                data = new
                {
                    id,
                    transform = new
                    {
                        position = new int[] { 0, 0, 0 },
                        scale = 1.0,
                        rotation = new[] { rotation.X, rotation.Y, rotation.Z },
                    },

                }
            };
        }
        #endregion

        #region Nodes
        public static object RemoveNode(string id)
        {
            return new
            {
                id = "scene/node/delete",
                data = new
                {
                    id
                }
            };
        }
        #endregion


        #region Skybox
        /// <summary>
        /// Sets the time of the sky if the skybox is set to the dynamic skybox. Time value ranges from 0 to 24. 12.5 is equal to 12:30
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static object SetSkyboxTime(double time)
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

        public static object SkyboxUpdate(String rt, String lf, String up, String dn, String bk, String ft)
        {
            return new
            {
                id = "scene/skybox/update",
                data = new
                {
                    type = "static",
                    files = new
                    {
                        xpos = rt,
                        xneg = lf,
                        ypos = up,
                        yneg = dn,
                        zpos = bk,
                        zneg = ft
                    }
                }

            };

        }

        #endregion

        #region Terrain
        public static object TerrainAdd(int length, int width, float[] heights)
        {
            int[] size = new int[] { length, width };
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
                data = new { }
            };
        }

        public static object TerrainAddNode(Vector3 position, Vector3 rotation)
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
                            position = new int[] { (int)position.X, (int)position.Y, (int)position.Z },
                            scale = 1,
                            rotation = new int[] { (int)rotation.X, (int)rotation.Y, (int)rotation.Z }
                        },
                        terrain = new
                        {
                            smoothnormals = true
                        }
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

        public static object GetHeight(Vector3 position)
        {
            return new
            {
                id = "scene/terrain/getheight",
                data = new
                {
                    position = new[] { position.X, position.Z }
                }
            };
        }

        public static object GetHeights(Vector3[] positions)
        {
            return new
            {
                id = "scene/terrain/getheights",
                data = new
                {
                    positions = positions.Select(p => new[] { p.X, p.Z }).ToArray()
                }
            };
        }
        #endregion

        #region Objects
        public static object Add3DObject(string name, Vector3 pos, double scale, string fileName, int rotation)
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
                            position = new[] { pos.X, pos.Y - 0.8, pos.Z },
                            scale,
                            rotation = new[] { 0, rotation, 0 }
                        },
                        model = new
                        {
                            file = fileName
                        }
                    }
                }
            };
        }

        public static object AddAnimatedObject(string name, Vector3 pos, double scale, Vector3 rot, string fileName,
            string animationName)
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
                            position = new[] { pos.X, pos.Y, pos.Z },
                            scale,
                            rotation = new[] { rot.X, rot.Y, rot.Z }
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
        #endregion

        #region Routes
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

        public static object RouteFollow(string routeID, string nodeID, double speed)
        {
            return new
            {
                id = "route/follow",
                data = new
                {
                    route = routeID,
                    node = nodeID,
                    speed,
                    offset = 0.0,
                    rotate = "XZ",
                    smoothing = 1.0,
                    followHeight = true,
                    rotateOffset = new[] { 0, 0, 0 },
                    positionOffset = new[] { 0, 0, 0 }
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

        public static object RouteSpeed(string node, double speed)
        {
            return new
            {
                id = "route/follow/speed",
                data = new
                {
                    node,
                    speed
                }
            };
        }

        public static object PanelAdd(string name, string parentID, Vector3 position, Vector3 rotation, double sizeX, double sizeY, int resolutionX, int resolutionY, bool castShadow)
        {
            return new
            {
                id = "scene/node/add",
                data = new
                {
                    name,
                    parent = parentID,
                    components = new
                    {
                        transform = new
                        {
                            position = new[] { position.X, position.Y, position.Z },
                            scale = 1,
                            rotation = new[] { rotation.X, rotation.Y, rotation.Z }
                        },
                        panel = new
                        {
                            size = new double[] { sizeX, sizeY },
                            resolution = new int[] { resolutionX, resolutionY },
                            background = new int[] { 10, 0, 0, 0 },
                            castShadow = true
                        }
                    }
                }
            };
        }

        public static object TextAdd(string panelID, string text)
        {
            return new
            {
                id = "scene/panel/drawtext",
                data = new
                {
                    id = panelID,
                    text,
                    position = new int[] { 50, 75 },
                    size = 16.0,
                    color = new int[] { 255, 0, 0, 5 },
                    font = "segoeui"
                }
            };
        }


        public static object PanelClear(string id)
        {
            return new
            {
                id = "scene/panel/clear",
                data = new
                {
                    id
                }
            };
        }

        public static object PanelSwap(string id)
        {
            return new
            {
                id = "scene/panel/swap",
                data = new
                {
                    id
                }
            };
        }

        public static object PanelSetColor(string id)
        {
            return new
            {
                id = "scene/panel/setclearcolor",
                data = new
                {
                    id,
                    color = new int[] { 0, 0, 0, 0 }
                }
            };
        }

        // public static object PanelGetLines(string id)
        // {
        //     return new
        //     {
        //         id = "scene/panel/drawlines",
        //         data = new
        //         {
        //             id,
        //             width= 1.0,
        //         }
        //     };
        // }

        #endregion


        #region Roads
        public static object RoadAdd(string route, string road)
        {
            return new
            {
                id = "scene/road/add",
                data = new
                {
                    route,
                    normal = road,
                    heightoffset = 0.01
                }
            };
        }
        #endregion

        #region Validation
        public static string ValidateAndGetSessionId(JObject serverResponse)
        {
            if (serverResponse == null ||
                !HasValidIdAndData(serverResponse) ||
                !(serverResponse["data"] is JArray))
                throw new CommunicationException("Received invalid data.");

            var sessionList = (JArray)serverResponse["data"];
            string sessionId = null;

            foreach (var session in sessionList)
            {
                var hostName = session?["clientinfo"]?["host"]?.ToString();
                if (hostName == null || session?["id"] == null) continue;

                if (hostName.Equals(Environment.MachineName, StringComparison.CurrentCultureIgnoreCase))
                    sessionId = session?["id"]?.ToString();
            }

            return sessionId;
        }

        public static string ValidateAndGetTunnelId(JObject serverResponse)
        {
            if (serverResponse == null ||
                !HasValidIdAndData(serverResponse) ||
                !(serverResponse is JObject))
                throw new CommunicationException("Received invalid data.");

            var payload = (JObject)serverResponse["data"];
            if (!payload.ContainsKey("status") ||
                !payload.ContainsKey("id"))
                throw new CommunicationException("Received invalid status and/or id field.");

            if (payload["status"].ToString() != "ok")
                throw new CommunicationException($"The response has a status of {payload["status"]}.");

            var tunnelId = payload["id"].ToString();
            return tunnelId;
        }

        private static bool HasValidIdAndData(JObject data)
        {
            return
                data.ContainsKey("id") &&
                data.ContainsKey("data");
        }
        #endregion
    }
}
