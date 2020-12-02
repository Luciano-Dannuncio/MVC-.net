using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCnetcore.Models
{
    public class SubjectModel
    {
        public int Id { get; set; }
        public string NameSubject { get; set; }
        public int IdHeadTeacher { get; set; }
        public bool isactive { get; set; }
        public int HeadTeacherDni { get; set; } 
        public string HeadTeacherName { get; set; }
        public bool IsInscript { get; set; }

    }
}
