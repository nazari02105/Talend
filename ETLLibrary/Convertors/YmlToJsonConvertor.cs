using System.IO;
using YamlDotNet.Serialization;

namespace ETLLibrary.Convertors
{
    public static class YmlToJsonConvertor
    {
        public static string Convert(string yml)
        {
            var reader = new StringReader(yml);
            var deserializer = new Deserializer();
            var yamlObject = deserializer.Deserialize(reader);

            var serializer = new Newtonsoft.Json.JsonSerializer();
            var writer = new StringWriter();
            serializer.Serialize(writer, yamlObject);
            return writer.ToString();
        }
        
    }
}