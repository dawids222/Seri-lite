using System;

namespace Seri_Lite
{
    public interface IConverter
    {
        string Serialize(object value);
        T Deserialize<T>(string value);
        object Deserialize(Type type, string value);
    }
}
