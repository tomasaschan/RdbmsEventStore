using System;

namespace RdbmsEventStore
{
    public interface IEventSerializer
    {
        byte[] Serialize(object payload);

        object Deserialize(byte[] data, Type type);

        string Show(byte[] data);
    }
}