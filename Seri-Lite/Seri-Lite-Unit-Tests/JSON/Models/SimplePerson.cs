using System;

namespace LibLite.Seri.Lite.Tests.JSON.Models
{
    internal class SimplePerson
    {
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            return obj is SimplePerson person &&
                   Name == person.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
