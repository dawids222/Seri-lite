using LibLite.Seri.Lite.JSON.Parsing.Models;

namespace LibLite.Seri.Lite.JSON.Parsing.Readers
{
    public interface IJsonReader
    {
        JsonToken Read(string value);
        bool TryRead(string value, out JsonToken result);
    }
}
