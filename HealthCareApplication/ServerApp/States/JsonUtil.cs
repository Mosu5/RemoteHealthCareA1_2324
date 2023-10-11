using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.States
{
    internal class JsonUtil
    {
        public static object GetValueFromPacket(JsonObject packet, string key, string value = "")
        {
            //checking if the packet has a valid format
            if (!packet.ContainsKey(key))
            {
                throw new FormatException("Json packet format corrupted!");
            }

            //extracting the needed part of the JsonObject from the packet
            object valueString = packet[key];

            if (String.IsNullOrEmpty(value))
                return valueString;

            JsonObject valuePacket = packet[key] as JsonObject;
            //extracting the command from the command JsonObject
            object valueFromField = valuePacket[value];

            return valueFromField;
        }

    }
}
