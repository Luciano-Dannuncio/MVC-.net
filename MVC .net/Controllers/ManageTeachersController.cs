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
    [Authorize(Roles = "admin")]
    public class ManageTeachers : Controller
    {
        public IActionResult Index()
        {
         List<TeacherModel> teacherlist = ListTeachers();

            if (TempData["Success"] != null) { ViewBag.Success = TempData["Success"]; }
            
            if (TempData["Error"] != null) {ViewBag.Error = TempData["Error"];}

            if (teacherlist.Count == 0) { ViewBag.Warning = "No se encontraron profesores registrados para mostrar."; }
                       
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
               
                using (var db = new Models.DB.ChallengeCDBContext()) 
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
                        newteacher.ActiveTeachers = true;

                        db.Teachers.Add(newteacher);
                        db.SaveChanges();
                    }
                    else 
                    {
                        TempData["Error"] = "El Dni ingresado ya pertenece a un profesor registrado";
                        return RedirectToAction("Index","ManageTeachers");
                    }
                    TempData["Success"] = "Se ha registrado un nuevo profesor.";
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
            int teacherdni)
        {
            try
            {
                Models.DB.Teachers teacherupdate = new Models.DB.Teachers();
                
                using (var db = new Models.DB.ChallengeCDBContext())
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
                        
                        
                        db.SaveChanges();
                        TempData["Success"] = "Se han actualizado los datos del profesor.";
                    }
                    else 
                    {
                        TempData["error"] = "El profesor que intenta actualizar no exite o los datos ingresados son incorrectos";
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
        [HttpPost]
        public IActionResult DeleteTeacher(int teacherid)
        {
            try
            {
                using (var db = new Models.DB.ChallengeCDBContext()) 
                {
                    var TeacherToDelete = (from d in db.Teachers
                                           where d.IdTeachers == teacherid
                                           && d.ActiveTeachers == true
                                           select d
                                            ).FirstOrDefault();
                    if (TeacherToDelete != null)
                    {
                        TeacherToDelete.ActiveTeachers = false;
                        db.SaveChanges();
                        TempData["Success"] = "Se ha eliminado al profesor.";
                    }
                    else 
                    {
                        throw new Exception("No se ha encontrado el profesor a eliminar, intentálo de nuevo o contactate con la universidad");
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
//-------------------------------------------Funciones---------------------------------------------------------------------
        public TeacherModel TeacherData(int teacherid)
        {
            TeacherModel teacherdata = new TeacherModel();
            try
            {
                using (var db = new Models.DB.ChallengeCDBContext()) 
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
                using (var db = new Models.DB.ChallengeCDBContext())
                {
                    TeachersList = (from d in db.Teachers
                                    where d.NameTeachers != ""
                                    && d.ActiveTeachers == true
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
