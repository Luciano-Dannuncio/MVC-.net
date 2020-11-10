using System;
using System.Collections.Generic;

namespace MVCnetcore.Models.DB
{
    public partial class Users
    {
        public int IdUsers { get; set; }
        public string EmailUsers { get; set; }
        public string NameUsers { get; set; }
        public bool ActiveUsers { get; set; }
        public string PasswordUsers { get; set; }

        public int IdRoles { get; set; }

        public Users() { }
        public Users(string emailuser, string nameuser, string password)
        {

            EmailUsers = emailuser;
            NameUsers = nameuser;
            PasswordUsers = password;
            IdRoles = 2; //este formulario solo sirve para crear usuarios estudiantes. 
            ActiveUsers = true; //por ahora siempre activo, 
            //cuando implemente el envio de mail de corroboracion esta propiedad estara en false hasta
            //que el usuario confirme su usuario
        }
    }
}
