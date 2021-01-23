using System;
using System.ComponentModel.DataAnnotations;

namespace Orca.Entities
{
    public class Student
    {
        [Required]
        public String ID { get; init; }
        public String FirstName { get; init; }
        public String LastName { get; init; }
        public String Email { get; init; }


        public override string ToString()
        {
            return $"{ID}-{FirstName}-{LastName}-{Email}";
        }
    }
}
