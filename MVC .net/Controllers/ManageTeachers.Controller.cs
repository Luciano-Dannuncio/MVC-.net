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
    public class ManageTeachers : Controller
    {
        public IActionResult Index()
        {
         List<TeacherModel> teacherlist = ListTeachers();

            if (TempData["Success"] != null) { ViewBag.Success = TempData["Success"]; }
            
            if (TempData["Error"] != null) {ViewBag.Error = TempData["Error"];}

            if (teacherlist.Count == 0) { ViewBag.Warning = "No se encontraron profesores registrados para mostrar"; }
                       
         return View(teacherlist);
            
        }
        public IActionResult NewTeacher()
        {
            return View();
        }
        [HttpPost]
        public IActionResult NewTeacher(string teachername, string teacherlastname, int teacherdni, bool isactive)
        {
            try
            {
                Models.DB.Teachers newteacher = new Models.DB.Teachers();
                using (var db = new Models.DB.AlkemyChallengeCDBContext()) 
                {
                   var isteacher = (from d in db.Teachers
                                  where d.DniTeachers == teacherdni
                                  select d
                                    ).FirstOrDefault();
                    if (isteacher == null)
                    {

                        newteacher.NameTeachers = teachername;
                        newteacher.LastNameTeachers = teacherlastname;
                        newteacher.DniTeachers = teacherdni;
                        newteacher.ActiveTeachers = isactive;

                        db.Teachers.Add(newteacher);
                        db.SaveChanges();
                    }
                    else 
                    {
                        TempData["Error"] = "El Dni ingresado ya pertenece a un profesor registrado";
                        return RedirectToAction("Index","ManageTeachers");
                    }
                    TempData["Success"] = "Se ha registrado un nuevo profesor exitosamente";
                    return RedirectToAction("Index", "ManageTeachers");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"]= "Error Inesperado. Detalle del Error: " + ex;
                return RedirectToAction("Index","ManageTeachers");
            }
        }
        [HttpPost]
        public IActionResult ModifyTeacher(int teacherid)
        {
            TeacherModel modifiedteacher = TeacherData(teacherid);
            ViewBag.TeacherId = modifiedteacher.Id;
            ViewBag.TeacherName = modifiedteacher.Name;
            ViewBag.TeacherLastName = modifiedteacher.LastName;
            ViewBag.TeacherDni = modifiedteacher.Dni;
            ViewBag.IsActive = modifiedteacher.Active;
            return View();

        }
        [HttpPost]
        public IActionResult UpdateTeacher(int teacherid, 
            string teachername, string teacherlastname, 
            int teacherdni, bool isactive)
        {
            try
            {
                Models.DB.Teachers teacherupdate = new Models.DB.Teachers();
                using (var db = new Models.DB.AlkemyChallengeCDBContext())
                {
                    teacherupdate = (from d in db.Teachers
                                     where d.IdTeachers == teacherid
                                     && d.DniTeachers == teacherdni
                                     select d).FirstOrDefault();
                
                    if (teacherupdate != null)
                    {
                        if (teacherdni != default ) { teacherupdate.DniTeachers = teacherdni; }
                        if (teachername != default && teachername != "") { teacherupdate.NameTeachers = teachername; }
                        if (teacherlastname != default && teacherlastname != "") { teacherupdate.LastNameTeachers = teacherlastname; }
                        teacherupdate.ActiveTeachers = isactive;
                        
                        db.SaveChanges();
                        TempData["Success"] = "Se ha actualizado los datos del profesor exitosamente";
                    }
                    else 
                    {
                        TempData["error"] = "El profesor que se quiere modificar no exite o los datos ingresados son incorrectos";
                        return RedirectToAction("Index","ManageTeachers");
                    }
                }

                return RedirectToAction("Index","ManageTeachers");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error Inesperado. Detalle del Error: " + ex;
                return RedirectToAction("Index", "ManageTeachers");
            }

        }

        public TeacherModel TeacherData(int teacherid)
        {
            TeacherModel teacherdata = new TeacherModel();
            try
            {
                using (var db = new Models.DB.AlkemyChallengeCDBContext()) 
                {
                    teacherdata = (from d in db.Teachers
                                   where d.IdTeachers == teacherid
                                   select new TeacherModel 
                                   { 
                                    Id = teacherid,
                                    Name = d.NameTeachers,
                                    LastName = d.LastNameTeachers,
                                    Dni = d.DniTeachers,
                                    Active = d.ActiveTeachers
                                   }
                                  ).FirstOrDefault();
                }
                return teacherdata;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error Inesperado. Detalle del Error: " + ex;
                return teacherdata;
            }
        }

        public List<TeacherModel> ListTeachers() //Función que trae LISTADO DE Profesores 
        {
            List<TeacherModel> TeachersList = new List<TeacherModel>();
            try
            {
                using (var db = new Models.DB.AlkemyChallengeCDBContext())
                {
                    TeachersList = (from d in db.Teachers
                                    where d.NameTeachers != ""
                                    select new TeacherModel
                                    {
                                        Id = d.IdTeachers,
                                        Name = d.NameTeachers,
                                        LastName = d.LastNameTeachers,
                                        Active = d.ActiveTeachers,
                                        Dni = d.DniTeachers
                                    
                                    }).ToList();

                }
                return TeachersList;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error Inesperado. Detalle del Error: " + ex;

                return TeachersList;
            }
        }

    }
}
