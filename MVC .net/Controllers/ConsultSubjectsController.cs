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
    [Authorize]
    public class ConsultSubjectsController : Controller
    {
        public IActionResult Index()
        {
            if (TempData["Success"] != null) { ViewBag.Success = TempData["Success"]; }

            if (TempData["Error"] != null) { ViewBag.Error = TempData["Error"]; }

            if (TempData["Warning"] != null) { ViewBag.Error = TempData["Warning"]; }
            
            List<SubjectModel> ListOfSubjects = ListSubjects();

            if (ListOfSubjects.Count == 0)
            {
                ViewBag.Error = "No se encontraron materias que mostrar";
                return View(ListOfSubjects);
            }
            else
            {
                return View(ListOfSubjects);
            }
            
        }

        public List<SubjectModel> ListSubjects()
        {
            List<SubjectModel> SubjectsList = new List<SubjectModel>();
            try
            {
                using (var db = new Models.DB.AlkemyChallengeCDBContext()) 
                {
                    SubjectsList = (from d in db.Subjects
                                    where d.ActiveSubjects == true                                 
                                    select new SubjectModel
                                    { 
                                        Id = d.IdSubjects,
                                        NameSubject = d.NameSubjects,
                                                                             
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
