using Newtonsoft.Json;
using System;
using System.Text;
using UnityEngine;

namespace Serializator
{
    internal class JSONSerializator : ISerializator
    {
        public JSONSerializator()
        {
        }

        public byte[] Serialize(object obj)
        {
            return Encoding.UTF8.GetBytes($"{obj.GetType().ToString()},{JsonConvert.SerializeObject(obj)}");
        }

        public bool TryDeserialize(byte[] message, out Type type, out object obj)
        {
            try
            {
                var json = Encoding.UTF8.GetString(message);
                var jsonsplit = json.Split(new char[] { ',' }, 2);
                type = Type.GetType(jsonsplit[0]);
                obj = JsonConvert.DeserializeObject(jsonsplit[1], type);
                return true;
            }
            catch
            {
                Debug.Log("ERROR");
                obj = null;
                type = null;
                return false;
            }
        }
    }
}