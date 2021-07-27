using System.Reflection;

namespace Seri_Lite.JSON.Serialization.Property
{
    public class InheritCasePropertyNameResolver : IPropertyNameResolver
    {
        public string Resolve(PropertyInfo property) => property.Name;
    }
}
