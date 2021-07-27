using System.Reflection;

namespace Seri_Lite.JSON.Serialization.Property
{
    public class CamelCasePropertyNameResolver : IPropertyNameResolver
    {
        public string Resolve(PropertyInfo property)
        {
            var name = property.Name;
            return $"{char.ToLowerInvariant(name[0])}{name[1..]}";
        }
    }
}
