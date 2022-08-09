using System;

namespace LibLite.Seri.Lite.Tests.JSON.Models
{
    internal class AdvancePerson : IntermediatePerson
    {
        public Guid Id { get; set; }
        public DateTime BirthDate { get; set; }

        public override bool Equals(object obj)
        {
            return obj is AdvancePerson person &&
                   base.Equals(obj) &&
                   Id.Equals(person.Id) &&
                   BirthDate == person.BirthDate;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Id, BirthDate);
        }
    }
}
