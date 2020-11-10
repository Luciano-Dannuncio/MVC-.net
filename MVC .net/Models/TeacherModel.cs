using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCnetcore.Models
{
    public class TeacherModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Dni { get; set; }
        public bool Active { get; set; }
    }
}
