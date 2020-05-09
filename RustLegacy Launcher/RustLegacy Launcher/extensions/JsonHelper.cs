using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;

namespace RustLegacy_Launcher.extensions
{
    class JsonHelper
    {
        public static T Deserialize<T>(string stringJson)
        {
            return JsonConvert.DeserializeObject<T>(stringJson);
        }

        public static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static void SaveFile<T>(T obj, string path)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                StreamWriter sw = File.CreateText(path);

                using (JsonWriter writerr = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writerr, obj);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Concat("ERROR saveFile: ", ex));
            }
        }

        public static T ReadyFile<T>(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Concat("ERROR saveFile: ", ex));
                return default(T);
            }

        }

    }

}
