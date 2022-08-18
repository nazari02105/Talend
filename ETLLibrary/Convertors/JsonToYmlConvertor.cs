using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ETLLibrary.Convertors
{
    public static class JsonToYmlConvertor
    {
        public static string Convert(string json)
        {
            var expConverter = new ExpandoObjectConverter();
            dynamic deserializedObject = JsonConvert.DeserializeObject<ExpandoObject>(json, expConverter);

            var serializer = new YamlDotNet.Serialization.Serializer();
            return  serializer.Serialize(deserializedObject);
        }
    }
}