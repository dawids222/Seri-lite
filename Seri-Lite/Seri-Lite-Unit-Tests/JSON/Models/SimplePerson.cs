using System;

namespace Seri_Lite_Unit_Tests.JSON.Models
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
