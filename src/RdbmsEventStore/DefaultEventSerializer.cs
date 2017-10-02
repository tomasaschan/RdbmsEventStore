using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace RdbmsEventStore
{
    public class DefaultEventSerializer : IEventSerializer
    {
        protected static readonly JsonSerializer serializer = new JsonSerializer { Formatting = Formatting.None };


        public byte[] Serialize(object payload)
        {
            var writer = new StringWriter();
            serializer.Serialize(writer, payload);
            return Encoding.UTF8.GetBytes(writer.ToString());
        }

        public object Deserialize(byte[] data, Type type)
        {
            var reader = new StringReader(Encoding.UTF8.GetString(data));
            return serializer.Deserialize(reader, type);
        }

        public string Show(byte[] data) => Encoding.UTF8.GetString(data);
    }
}