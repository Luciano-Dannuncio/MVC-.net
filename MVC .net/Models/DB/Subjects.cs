using System;
using System.Collections.Generic;

namespace MVCnetcore.Models.DB
{
    public partial class Subjects
    {
        public int IdSubjects { get; set; }
        public int IdHeadTeacher { get; set; }
        public string NameSubjects { get; set; }
        public TimeSpan? TimeSubjects { get; set; }
        public bool ActiveSubjects { get; set; }
       
    }
}
