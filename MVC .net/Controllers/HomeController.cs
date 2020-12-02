using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVCnetcore.Models;
using MVCnetcore.Filters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace MVCnetcore.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
            
        public IActionResult Index()
        {
            var claimmail = User.Claims.ToArray();
            string usermail = claimmail[0].Value;
            int iduser;

            try
            {
                using (var db = new Models.DB.ChallengeCDBContext())
                {
                    var user = (from d in db.Users
                                  where d.EmailUsers == usermail
                                  select new UserLogInModel 
                                  { 
                                    Id = d.IdUsers,
                                    Name = d.NameUsers
                                  }
                                  ).FirstOrDefault();

                    if (user.Id == 0)
                    {
                        throw new Exception("Ha ocurrido un error inesperado, intentelo nuevamente o contactese con la universidad");
                    }
                    else
                    {
                        ViewBag.username = user.Name;
                        ViewBag.userid = user.Id;
                    }
                    return View();

                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error Inesperado. Detalle del Error: " + ex;
                return RedirectToAction("Index", "Home");

            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
