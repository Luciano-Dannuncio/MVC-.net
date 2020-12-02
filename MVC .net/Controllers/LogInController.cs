using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using MVCnetcore.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;


namespace MVCnetcore.Controllers
{
    
    public class LogInController : Controller
    {
    
        public IActionResult Index()
        {
            if ( (User.IsInRole("admin")) || (User.IsInRole("student"))) 
            {
                return RedirectToAction("Index", "Home");
            }
            else 
            {
                if (TempData["Success"] != null) { ViewBag.Success = TempData["Success"]; }

                if (TempData["Warning"] != null) { ViewBag.Error = TempData["Warning"]; }

                if (TempData["Error"] != null) { ViewBag.Error = TempData["Error"]; }

                return View();
            }
        }
        [AllowAnonymous]
        public IActionResult NewUser()
        {
            if (TempData["Success"] != null) { ViewBag.Success = TempData["Success"]; }

            if (TempData["Warning"] != null) { ViewBag.Error = TempData["Warning"]; }

            if (TempData["Error"] != null) { ViewBag.Error = TempData["Error"]; }
            
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Index(string email, string password) 
        {
            var encpass = EncryptPass(password);
            string role = "";
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
                                 Name = d.NameUsers,
                                 Password = d.PasswordUsers,
                                 IdRoles = d.IdRoles
                             }).FirstOrDefault();
                    
                    if (oUser == null)
                    {
                        ViewBag.Error = "Usuario o Contraseña Incorrectas";
                        return View();
                    }
                    
                    if (oUser.IdRoles == 1) { role = "admin"; } 
                    else if(oUser.IdRoles == 2) { role = "student"; }
                    
                    var claims = new List<Claim>
                    {                                            
                        new Claim(ClaimTypes.Email, oUser.Email),
                        new Claim(ClaimTypes.Name, oUser.Name ),
                        new Claim(ClaimTypes.Role, role)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "./LogIn");

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(claimsIdentity));
                   
                }
                
                if (role == "admin") { return RedirectToAction("Index", "ManageSubjects"); }
                
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error Inesperado. Detalle: " + ex;
                return View();
            }            
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateUser(string email, string nameuser,string password, string password2)
        {
            var correctpass = password.Equals(password2);
            string role = "";
            if (correctpass == true)
            {
                try
                {
                    UserLogInModel oUser = new UserLogInModel();
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
                            TempData["Error"] = "Ya existe una cuenta registrada con ese email.";
                            return RedirectToAction("NewUser", "LogIn");
                        }

                        TempData["Success"] = "Cuenta creada exitosamente";
                        oUser = (from d in db.Users
                                 where d.EmailUsers == email
                                 && d.PasswordUsers == encpass
                                 && d.ActiveUsers == true
                                 select new UserLogInModel
                                 {
                                     Id = d.IdUsers,
                                     Email = d.EmailUsers,
                                     Name = d.NameUsers,
                                     Password = d.PasswordUsers,
                                     IdRoles = d.IdRoles
                                 }).FirstOrDefault();

                        if (oUser == null)
                        {
                            ViewBag.Error = "Usuario o Contraseña Incorrectas";
                            return View();
                        }

                        if (oUser.IdRoles == 1) { role = "admin"; }
                        else if (oUser.IdRoles == 2) { role = "student"; }

                        var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, oUser.Email),
                        new Claim(ClaimTypes.Name, oUser.Name ),
                        new Claim(ClaimTypes.Role, role)
                    };

                        var claimsIdentity = new ClaimsIdentity(claims, "./LogIn");

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    }

                    if (role == "admin") { return RedirectToAction("Index", "ManageSubjects"); }

                    return RedirectToAction("Index", "Home");
                    // return RedirectToAction("Index", "LogIn"); //loggear directamente
                }
                catch (Exception ex)
                {
                    TempData["Error"] ="Ha ocurrido un error inesperado. Detalle: "+ ex;
                    return RedirectToAction("Index", "LogIn");
                }
            }
            else
            {
                TempData["Warning"] ="Las contraseñas no coinciden, intentalo nuevamente";
                return RedirectToAction("NewUser", "LogIn");
            }
        }
        [Authorize]
        public async  Task<IActionResult> LogOut() 
        {
            await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","LogIn");
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
