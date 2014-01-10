using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Com.BaseLibrary.Utility
{
    public static class ObjectBinarySerializer
    {
        public static string SerializeObject(object obj)
        {
            IFormatter formatter = new BinaryFormatter();
            string result = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                byte[] byt = new byte[stream.Length];
                byt = stream.ToArray();
                //result = Encoding.UTF8.GetString(byt£¬ 0£¬ byt.Length); 
                result = Convert.ToBase64String(byt);
                stream.Flush();
            }
            return result;
        }
        public static object DeserializeObject(string str)
        {
            IFormatter formatter = new BinaryFormatter();
            //byte[] byt = Encoding.UTF8.GetBytes(str); 
            byte[] byt = Convert.FromBase64String(str);
            object obj = null;
            using (Stream stream = new MemoryStream(byt, 0, byt.Length))
            {
                obj = formatter.Deserialize(stream);
            }
            return obj;
        }

        public static T DeserializeObject<T>(string str)
            where T : class
        {
            object obj = DeserializeObject(str);
            return obj as T;
        }
    }
}