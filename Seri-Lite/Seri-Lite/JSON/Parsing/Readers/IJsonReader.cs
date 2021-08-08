using Seri_Lite.JSON.Parsing.Models;

namespace Seri_Lite.JSON.Parsing.Readers
{
    public interface IJsonReader
    {
        JsonToken Read(string value);
        bool TryRead(string value, out JsonToken result);
    }
}
