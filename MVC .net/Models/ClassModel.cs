using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCnetcore.Models
{
    public class ClassModel
    {
        public int IdClasses { get; set; }
        public int IdSubjects { get; set; }
        public string SubjectName { get; set; }
        public int TeacherDni { get; set; }
        public string NameTeacher { get; set; }
        public int MaxStudentClasses { get; set; }
        public bool ActiveClasses { get; set; }
        public bool MaxCapacityClasses { get; set; }
        public int ClassroomClasses { get; set; }
    }
}
