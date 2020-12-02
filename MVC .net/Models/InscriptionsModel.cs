using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCnetcore.Models
{
    public class InscriptionsModel
    {
        public int Id { get; set; }
        public string SubjectName { get; set; }
        public int ClassNumber { get; set; }
        public string TeacherName { get; set; }
    }
}
