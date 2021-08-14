using System;
using System.Collections.Generic;

namespace Seri_Lite_Unit_Tests.JSON.Models
{
    internal class IntermediatePerson : SimplePerson
    {
        public int Age { get; set; }
        public double Height { get; set; }
        public bool IsMarried { get; set; }
        public SimplePerson Partner { get; set; }

        public override bool Equals(object obj)
        {
            return obj is IntermediatePerson person &&
                   base.Equals(obj) &&
                   Age == person.Age &&
                   Height == person.Height &&
                   IsMarried == person.IsMarried &&
                   EqualityComparer<SimplePerson>.Default.Equals(Partner, person.Partner);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Age, Height, IsMarried, Partner);
        }
    }
}
