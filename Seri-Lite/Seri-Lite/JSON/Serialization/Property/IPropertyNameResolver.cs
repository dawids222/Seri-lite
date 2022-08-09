using System.Reflection;

namespace LibLite.Seri.Lite.JSON.Serialization.Property
{
    public interface IPropertyNameResolver
    {
        string Resolve(PropertyInfo property);
    }
}
