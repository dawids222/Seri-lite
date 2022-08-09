using System;

namespace LibLite.Seri.Lite
{
    public interface ISerializer
    {
        string Serialize(object value);
        T Deserialize<T>(string value);
        object Deserialize(Type type, string value);
    }
}
