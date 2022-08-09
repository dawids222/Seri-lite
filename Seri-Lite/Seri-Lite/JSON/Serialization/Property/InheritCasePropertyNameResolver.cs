using System.Reflection;

namespace LibLite.Seri.Lite.JSON.Serialization.Property
{
    public class InheritCasePropertyNameResolver : IPropertyNameResolver
    {
        public string Resolve(PropertyInfo property) => property.Name;
    }
}
