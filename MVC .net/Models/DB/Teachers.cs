using System;
using System.Collections.Generic;

namespace MVCnetcore.Models.DB
{
    public partial class Teachers
    {
        public int IdTeachers { get; set; }
        public string NameTeachers { get; set; }
        public string LastNameTeachers { get; set; }
        public int DniTeachers { get; set; }
        public bool ActiveTeachers { get; set; }
    }
}
