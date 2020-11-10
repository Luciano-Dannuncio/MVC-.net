using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using MVCnetcore.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MVCnetcore.Controllers
{
    [AllowAnonymous]
    public class LogInController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;


        /*public LogInController() { }
        public LogInController(UserManager<IdentityUser> userManager,
               SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        */
        public IActionResult Index()
        {
            if (TempData["Success"] != null) { ViewBag.Success = TempData["Success"]; }

            if (TempData["Warning"] != null) { ViewBag.Error = TempData["Warning"]; }

            if (TempData["Error"] != null) { ViewBag.Error = TempData["Error"]; }
           
            return View();
        }

        public IActionResult NewUser()
        {
            if (TempData["Success"] != null) { ViewBag.Success = TempData["Success"]; }

            if (TempData["Warning"] != null) { ViewBag.Error = TempData["Warning"]; }

            if (TempData["Error"] != null) { ViewBag.Error = TempData["Error"]; }
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string email, string password) 
        {
            var encpass = EncryptPass(password);
           
            try
            {
                UserLogInModel oUser = new UserLogInModel();
                using (var db = new Models.DB.AlkemyChallengeCDBContext())
                {
                    oUser = (from d in db.Users
                             where d.EmailUsers == email
                             && d.PasswordUsers == encpass
                             && d.ActiveUsers == true
                             select new UserLogInModel
                             {
                                 Id = d.IdUsers,
                                 Email = d.EmailUsers,
                                 Password = d.PasswordUsers
                             }).FirstOrDefault();
                    
                    if (oUser == null)
                    {
                        ViewBag.Error = "Usuario o Contraseña Incorrectas";
                        return View();
                    }
                   var user = new IdentityUser { Id = oUser.Id.ToString() , UserName = email };

                    await signInManager.SignInAsync(user, isPersistent: false);
                                   
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error Inesperado. Detalle: " + ex;
                return View();
            }            
        }

        [HttpPost]
        public IActionResult CreateUser(string email, string nameuser,string password, string password2)
        {
            var correctpass = password.Equals(password2);
            if (correctpass == true)
            {
                try
                {
                    string encpass = EncryptPass(password);
                    
                    using (var db = new Models.DB.AlkemyChallengeCDBContext())
                    {
                        var userposible = (from d in db.Users
                                           where d.EmailUsers == email
                                           select d
                                           ).FirstOrDefault();
                        
                        if (userposible == null)
                        {
                            Models.DB.Users newuser = new Models.DB.Users(email, nameuser, encpass);
                            db.Users.Add(newuser);
                            
                            db.SaveChanges();


                        }
                        else 
                        {
                            TempData["Error"] = "Ya existe un usuario registrado con ese Email.";
                            return RedirectToAction("NewUser", "LogIn");
                        }

                    }
                    TempData["Success"] ="Usuario creado exitosamente";

                    return RedirectToAction("Index", "LogIn");
                }
                catch (Exception ex)
                {
                    TempData["Error"] ="Ha ocurrido un error inesperado. Detalle: "+ ex;
                    return RedirectToAction("Index", "LogIn");
                }
            }
            else
            {
                TempData["Warning"] ="Las contraseñas no coinciden, intentelo nuevamente";
                return RedirectToAction("NewUser", "LogIn");
            }
        }

        // ---------------------------------------------Funciones----------------------------------------------

        public string EncryptPass(string pass) 
        {

            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;        
            StringBuilder sb = new StringBuilder();

            stream = sha256.ComputeHash(encoding.GetBytes(pass));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();

        }
    

    }
}
