using Good.Core;
using Newtonsoft.Json;
using System.IO;

namespace Good.Editor
{
    public static class LevelSaver
    {
        public static void SaveLevel() 
        {
            var level = Layout.Current;

            using (StreamWriter file = File.CreateText("level.json"))
            {
                JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
                serializer.Serialize(file, level);
            }
        }
    }
}
