using System.Reflection;

namespace Seri_Lite.JSON.Serialization.Property
{
    public interface IPropertyNameResolver
    {
        string Resolve(PropertyInfo property);
    }
}
