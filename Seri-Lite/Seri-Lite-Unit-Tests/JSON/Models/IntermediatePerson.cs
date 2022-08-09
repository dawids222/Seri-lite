using System;
using System.Collections.Generic;

namespace LibLite.Seri.Lite.Tests.JSON.Models
{
    internal class IntermediatePerson : SimplePerson
    {
        public int Age { get; set; }
        public double Height { get; set; }
        public float Salary { get; set; }
        public bool IsMarried { get; set; }
        public SimplePerson Partner { get; set; }

        public override bool Equals(object obj)
        {
            return obj is IntermediatePerson person &&
                   base.Equals(obj) &&
                   Age == person.Age &&
                   Height == person.Height &&
                   Salary == person.Salary &&
                   IsMarried == person.IsMarried &&
                   EqualityComparer<SimplePerson>.Default.Equals(Partner, person.Partner);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Age, Height, Salary, IsMarried, Partner);
        }
    }
}
