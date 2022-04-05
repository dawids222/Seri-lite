using System;

namespace Seri_Lite.Extensions
{
    internal static class TypeExtensions
    {
        internal static bool IsJsonPrimitive(this Type type)
            => type.IsPrimitive ||
               type == typeof(Decimal) ||
               type == typeof(String) ||
               type == typeof(DateTime) ||
               type == typeof(Guid);
    }
}
