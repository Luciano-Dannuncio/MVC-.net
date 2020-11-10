using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCnetcore.Models.DB
{
    public class Classes
    {
        public int IdClasses { get; set; }
        public int IdSubjects { get; set; }
        public int IdTeachers { get; set; }
        public TimeSpan? TimeClasses { get; set; }
        public int MaxStudentClasses { get; set; }
        public bool ActiveClasses { get; set; }
        public bool MaxCapacityClasses { get; set; }
        public int ClassroomClasses { get; set; }
    }
}
