using System;
using System.Collections.Generic;

namespace MVCnetcore.Models.DB
{
    public partial class Inscriptions
    {
        public int IdInscriptions { get; set; }
        public int IdUsersInscriptions { get; set; }
        public int IdClassesInscriptions { get; set; }
        public bool? ActiveInscriptions { get; set; }
        public int IdSubjectsInscriptions { get; set; }

        
        public Inscriptions() { }
        public Inscriptions(int idclass, int idsubject, int iduser, bool active)
        {
            IdClassesInscriptions = idclass;
            IdSubjectsInscriptions = idsubject;
            IdUsersInscriptions = iduser;
            ActiveInscriptions = active;
        }

    }
    
}
