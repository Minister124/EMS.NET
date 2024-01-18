using System.Text.Json;

namespace ClientLibrary.Helper
{
    public static class Serializations
    {
        public static string SerializeObj<T>(T modelObject)
        {
            return JsonSerializer.Serialize(modelObject);
        }
        public static T DeserializeJsonString<T>(string jsonString) => JsonSerializer.Deserialize<T>(jsonString)!;

        public static IList<T> DeserializeJsonStringList<T>(string jsonString)
        {
            return JsonSerializer.Deserialize<IList<T>>(jsonString)!;
        }
    }
}
