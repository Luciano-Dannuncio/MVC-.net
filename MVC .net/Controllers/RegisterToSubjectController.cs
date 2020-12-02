using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MVCnetcore.Models;
using MVCnetcore.Filters;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace MVCnetcore.Controllers
{
    [Authorize(Roles = "student")]
    public class RegisterToSubjectController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(int subjectid)
        {
            return View(ListClasses(subjectid));
        }

        [HttpPost]
        public IActionResult Inscription(int classid, int subjectid)
        {         
            var claimmail = User.Claims.ToArray();
            string usermail = claimmail[0].Value;
            int iduser;
            ClassModel inscriptiondata;

            try
            {
                using (var db = new Models.DB.AlkemyChallengeCDBContext())
                {
                    var userid = (from d in db.Users
                                  where d.EmailUsers == usermail
                                  select d.IdUsers
                                  ).FirstOrDefault();

                    if (userid == 0)
                    {
                        throw new Exception("Ha ocurrido un error inesperado, intentelo nuevamente o contactese con la universidad");
                    }
                    else
                    {
                        iduser = userid;
                    }

                }

                using (var db = new Models.DB.AlkemyChallengeCDBContext())
                {
                        
                         var inscriptionposible = (from c in db.Classes
                                                   where c.IdClasses == classid
                                                   && c.ActiveClasses == true
                                                   && c.MaxCapacityClasses == false
                                                   select c
                                                  ).FirstOrDefault();

                    if (inscriptionposible != default)
                    {
                        Models.DB.Inscriptions inscription = (from d in db.Inscriptions
                                                                 where d.IdClassesInscriptions == classid
                                                                 && d.IdSubjectsInscriptions == subjectid
                                                                 && d.IdUsersInscriptions == iduser
                                                                 && d.ActiveInscriptions == true
                                                                 select d
                                                               ).FirstOrDefault();
                        if (inscription == null)
                        {
                             inscriptiondata = (from d in db.Classes
                                                          where d.IdClasses == classid
                                                          && d.IdSubjects == subjectid
                                                          select new ClassModel
                                                          { 
                                                            SubjectName = (from c in db.Subjects
                                                                           where c.IdSubjects == subjectid
                                                                           select c.NameSubjects
                                                                           ).FirstOrDefault(),
                                                            ClassroomClasses = d.ClassroomClasses
                                                          }).FirstOrDefault();

                            Models.DB.Inscriptions newinscription = new Models.DB.Inscriptions(classid,subjectid,iduser,true);
                            db.Inscriptions.Add(newinscription);

                            db.SaveChanges();

                            UpdateMaxCapacityClasses(classid, subjectid);
                            //generar reporte o informe para administrador
                        }
                        else
                        {
                            TempData["Warning"] = "Ya se encuentra inscripto a esta comisión";
                            return RedirectToAction("index", "ConsultSubjects");
                        }
                    }
                    else 
                    {
                        TempData["Error"] = "No se ha podido realizar la inscripción por no haber " +
                            "cupos disponibles o no encontrarse la materia/comision activas";
                        return RedirectToAction("index", "ConsultSubjects");
                    }
                    
                    TempData["Success"] = "Se ha realizado la inscripcion a la comisión : "
                        + inscriptiondata.ClassroomClasses + " de la materia " + inscriptiondata.SubjectName + " Exitosamente";

                    return RedirectToAction("index", "ConsultSubjects");

                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error Inesperado. Detalle del Error: " + ex;
                return RedirectToAction("Index", "ConsultSubjects");

            }
        }
        // ---------------------------------------Funciones------------------------------------

        public List<ClassModel> ListClasses(int IdSubject)
        { //Función que genera listado de comisiones pertenecientes a una materia
            List<ClassModel> ClassesList = new List<ClassModel>();
            try
            {
                using (var db = new Models.DB.AlkemyChallengeCDBContext())
                {
                    ClassesList = (from d in db.Classes
                                   where d.IdSubjects == IdSubject
                                   select new ClassModel
                                   {
                                       IdClasses = d.IdClasses,
                                       IdSubjects = d.IdSubjects,
                                       MaxStudentClasses = d.MaxStudentClasses,
                                       ClassroomClasses = d.ClassroomClasses,
                                       MaxCapacityClasses = d.MaxCapacityClasses,
                                       NameTeacher = (from c in db.Teachers
                                                      where c.IdTeachers == d.IdTeachers
                                                      select new string
                                                      (c.LastNameTeachers + " " + c.NameTeachers
                                                      )).FirstOrDefault()
                                   }).ToList();

                }
                return ClassesList;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error Inesperado. Detalle del Error: " + ex;

                return ClassesList;
            }
        }

        private void UpdateMaxCapacityClasses(int classid, int subjectid)
        {

            try
            {
                using (var db = new Models.DB.AlkemyChallengeCDBContext())
                {
                    List<Models.DB.Classes> updateposible = (from d in db.Classes
                                                             where d.IdSubjects == subjectid
                                                             && d.IdClasses == classid
                                                             select d
                                         ).ToList();

                    if (updateposible.Count != 0)
                    {
                        var maxclasscapacity = updateposible.First().MaxStudentClasses;
                        if (updateposible.Count == maxclasscapacity)
                        {
                            var changemaxcapacity = (from d in db.Classes
                                                     where d.IdClasses == classid
                                                     select d
                                                     ).FirstOrDefault();
                            if (changemaxcapacity != default)
                            {
                                changemaxcapacity.MaxCapacityClasses = true;
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                TempData["AdminError"] = "Error Inesperado. Detalle del Error: " + ex;
            }
        }
    }
}
