
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Orca.Entities
{
    public class StudentEvent
    {
        [Required]
        public String CourseID { get; init; }
        public DateTime Timestamp { get; init; }
        [Required]
        public Student Student { get; init; }
        [Required]
        public String EventType { get; init; }
        public String ActivityType { get; init; }

        public override string ToString()
        {
            return $"{{CourseID: {CourseID}, Student: {{ {Student} }}, EventType: {EventType}, ActivityType: {ActivityType}, Timestamp: {Timestamp} }}";
        }

    }
}
