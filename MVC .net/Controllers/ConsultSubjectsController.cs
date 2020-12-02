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
    public class ConsultSubjectsController : Controller
    {
        public IActionResult Index()
        {
            
            
            List<SubjectModel> ListOfSubjects = ListSubjects();

            if (TempData["Success"] != null) { ViewBag.Success = TempData["Success"]; }

            if (TempData["Error"] != null) { ViewBag.Error = TempData["Error"]; }

            if (TempData["Warning"] != null) { ViewBag.Error = TempData["Warning"]; }

            if (ListOfSubjects.Count == 0)
            {
                ViewBag.Error = "No hay materias que mostrar.";
                return View(ListOfSubjects);
            }
            else
            {
                return View(ListOfSubjects);
            }
            
        }
        //--------------------------------------------Funciones----------------------------------------
        public List<SubjectModel> ListSubjects()
        {
            List<SubjectModel> SubjectsList = new List<SubjectModel>();
            try
            {
                var claimmail = User.Claims.ToArray();
                string usermail = claimmail[0].Value;
              
                using (var db = new Models.DB.ChallengeCDBContext()) 
                {
                    var userid = (from d in db.Users
                                  where d.EmailUsers == usermail
                                  select d.IdUsers
                                  ).FirstOrDefault();

                    if (userid == 0)
                    {
                        throw new Exception("Ha ocurrido un error inesperado, intentálo de nuevo o contactate con la universidad");
                    }
                             
                    SubjectsList = (from d in db.Subjects
                                    where d.ActiveSubjects == true
                                    select new SubjectModel
                                    {
                                        Id = d.IdSubjects,
                                        NameSubject = d.NameSubjects,
                                        isactive = d.ActiveSubjects,
                                        IsInscript = (from c in db.Inscriptions
                                                      where c.IdUsersInscriptions == userid
                                                      && c.IdSubjectsInscriptions == d.IdSubjects 
                                                      && c.ActiveInscriptions == true
                                                      select true ).FirstOrDefault()                       
                                    }).ToList();

                }
                return SubjectsList;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error Inesperado. Detalle de Error: " + ex;
                 
                return SubjectsList;
            }
        }
    }
}
