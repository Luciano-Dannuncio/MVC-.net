using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCnetcore.Models;

namespace MVCnetcore.Controllers
{
    [Authorize(Roles = "student")]
    public class MyInscriptionsController : Controller
    {
        public IActionResult Index()
        {
            List<InscriptionsModel> ListOfInscriptions = ListInscriptions();

            if (TempData["Success"] != null) { ViewBag.Success = TempData["Success"]; }

            if (TempData["Error"] != null) { ViewBag.Error = TempData["Error"]; }

            if (TempData["Warning"] != null) { ViewBag.Error = TempData["Warning"]; }

            if (ListOfInscriptions.Count == 0)
            {
                ViewBag.Error = "No estás inscripto a ninguna materia.";
                return View(ListOfInscriptions);
            }
            else
            {
                return View(ListOfInscriptions);
            }
        }
        [HttpPost]
        public IActionResult CancelInscription(int inscriptionid)
        {
            try
            {
                var claimmail = User.Claims.ToArray();
                string usermail = claimmail[0].Value;

                using (var db = new Models.DB.AlkemyChallengeCDBContext())
                {
                    var userid = (from d in db.Users
                                  where d.EmailUsers == usermail
                                  select d.IdUsers
                                  ).FirstOrDefault();

                    if (userid == 0)
                    {
                        throw new Exception("Ha ocurrido un error inesperado, intentalo de nuevo o contactate con la universidad.");
                    }
                    var InscriptionToCancel = (from d in db.Inscriptions
                                               where d.ActiveInscriptions == true
                                               && d.IdInscriptions == inscriptionid
                                               && d.IdUsersInscriptions == userid
                                               select d
                                               ).FirstOrDefault();
                    if (InscriptionToCancel != null) 
                    {
                        InscriptionToCancel.ActiveInscriptions = false;
                        db.SaveChanges();
                        TempData["Success"] = "Se ha cancelado la inscripción";

                    }
                    else
                    {
                        throw new Exception("Ha ocurrido un error inesperado, intentálo de nuevo o contactate con la universidad");
                    }                 
                }
               return RedirectToAction("Index", "MyInscriptions");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error Inesperado. Detalle de Error: " + ex;

                return View();
            }
        }
      
        //----------------------------------------------------------------- Funciones --------------------------------------------------------------

        public List<InscriptionsModel> ListInscriptions() 
        {
            List<InscriptionsModel> InscriptionList = new List<InscriptionsModel>();
            try
            {
                var claimmail = User.Claims.ToArray();
                string usermail = claimmail[0].Value;
                

                using (var db = new Models.DB.AlkemyChallengeCDBContext())
                {
                    var userid = (from d in db.Users
                                  where d.EmailUsers == usermail
                                  select d.IdUsers
                                  ).FirstOrDefault();

                    if (userid == 0)
                    {
                        throw new Exception("Ha ocurrido un error inesperado, intentálo de nuevo o contactate con la universidad");
                    }

                    InscriptionList = (from d in db.Inscriptions
                                       where d.IdUsersInscriptions == userid
                                       && d.ActiveInscriptions == true
                                       select new InscriptionsModel 
                                       { 
                                        Id= d.IdInscriptions,

                                        SubjectName = (from c in db.Subjects
                                                       where c.IdSubjects == d.IdSubjectsInscriptions
                                                       select c.NameSubjects
                                                        ).FirstOrDefault(),
                                        ClassNumber = (from e in db.Classes
                                                       where e.IdClasses == d.IdClassesInscriptions
                                                       && e.IdSubjects == d.IdSubjectsInscriptions
                                                       select e.ClassroomClasses
                                                        ).FirstOrDefault()
                                      
                                       }).ToList();
                }
                return InscriptionList;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error Inesperado. Detalle de Error: " + ex;

                return InscriptionList;
            }
        }

    }
}
