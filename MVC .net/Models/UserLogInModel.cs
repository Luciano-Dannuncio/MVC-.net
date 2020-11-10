using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCnetcore.Models
{
    public class UserLogInModel
    {
        public int Id { get; set; }

        public string  Email { get; set; }

        public string Password { get; set; }
    }
}
