using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace parser 
{
    [JsonConverter(typeof(ASTSerializer))]
    public interface ASTNode { }

    public class ASTSerializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(ASTNode).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var node = value as ASTNode;

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            serializer.Serialize(writer, node.GetType().Name);

            foreach (FieldInfo field in value.GetType().GetFields())
            {
                object propValue = field.GetValue(value);
                if (propValue != null)
                {
                    writer.WritePropertyName(field.Name);

                    if(propValue is lexer.Token)
                        writer.WriteValue(propValue.ToString());
                    else
                        serializer.Serialize(writer, propValue);
                }
            }

            writer.WriteEndObject();
        }
    }
}