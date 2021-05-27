using System.IO;
using System.Text.Json;

namespace RadioTools
{
    // Using UTF-8 serialization since it's
    // standard for JSON
    public static class Serializer
    {
        // Generates new opbject only when
        // necessary and disposes when they're
        // out of scope to save memory
        public static JsonSerializerOptions options{
            get
            {
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.Converters.Add(new IPAddressConverter());
                options.WriteIndented = true;
                return options;
            }
        }

        public static void SaveJSON(object data, string path, JsonSerializerOptions recycle = null)
        {
            recycle = recycle == null ? options : recycle;

            byte[] JSON = JsonSerializer.SerializeToUtf8Bytes(data, recycle);
            File.WriteAllBytes(path + ".json", JSON);
        }

        public static T LoadJSON<T>(string path) where T : class
        {
            JsonSerializerOptions opts = options;
            byte[] JSON;
            try{
                JSON = File.ReadAllBytes(path + ".json");
            }
            catch(FileNotFoundException)
            {
                Logger.Println(System.String.Format("No {0} found, generating...", path));
                T init = System.Activator.CreateInstance(typeof(T)) as T;
                SaveJSON(init, path, opts);
                Logger.Println("Generated " + path + ".json");
                return init;
            }
            return JsonSerializer.Deserialize<T>(JSON, opts);
        }
    }
}