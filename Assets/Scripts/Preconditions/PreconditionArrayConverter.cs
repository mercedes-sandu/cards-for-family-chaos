using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Preconditions
{
    public class PreconditionArrayConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Precondition[] preconditions = (Precondition[])value;
            writer.WriteStartArray();

            if (preconditions == null) return;
            
            foreach (Precondition precondition in preconditions)
            {
                // string preconditionJson = PreconditionSerializer.Serialize(precondition);
                // writer.WriteRawValue(preconditionJson);
            }
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            Precondition[] preconditions = new Precondition[array.Count];

            for (int i = 0; i < array.Count; i++)
            {
                string preconditionJson = array[i].ToString();
                preconditions[i] = PreconditionSerializer.Deserialize(preconditionJson);
            }

            return preconditions;
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Precondition[]);
        }
    }
}