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
    public class ManageSubjectsController : Controller
    {
 //------------------------------------------------- CÁTEDRAS -----------------------------------------------------------------------------------
 
        public IActionResult Index()  //INDEX CON LISTADO DE MATERIAS
        {
            List<SubjectModel> ListOfSubjects = ListSubjects();
            if (TempData["Success"] != null){ViewBag.Success = TempData["Success"];}
            
            if (TempData["Warning"] != null){ViewBag.Warning = TempData["Warning"];}

            if (ListOfSubjects.Count == 0){ ViewBag.Error = "No se encontraron materias que mostrar."; }
            
            return View(ListOfSubjects);
            
        }
        
        public IActionResult NewSubject()  // VISTA DE NUEVA MATERIA
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewSubject(string namesubject, bool isactive , int headteacherdni)
        {     //ENVIO DEL FORMULARIO DE LA NUEVA MATERIA, CORROBORACIÓN y CREACIÓN DE LA MISMA.
            try
            {
                using (var db = new Models.DB.ChallengeCDBContext())
                {
                    var newsubject = (from d in db.Subjects
                                      where d.NameSubjects == namesubject

                                      && d.ActiveSubjects == isactive
                                      select d
                                      ).FirstOrDefault();
                    if (newsubject == default)
                    {
                        Models.DB.Subjects AddSubject = new Models.DB.Subjects();

                        AddSubject.NameSubjects = namesubject;
                        AddSubject.ActiveSubjects = true;
                        AddSubject.IdHeadTeacher = (from d in db.Teachers
                                                    where d.DniTeachers == headteacherdni
                                                    && d.ActiveTeachers == true
                                                    select d.IdTeachers
                                                    ).FirstOrDefault();
                        
                        if(AddSubject.IdHeadTeacher == default)
                        {
                            TempData["warning"] = "El DNI ingresado no corresponde a un profesor registrado.";
                            return RedirectToAction("Index", "ManageSubjects");
                        }

                        db.Subjects.Add(AddSubject);
                        db.SaveChanges();

                        TempData["Success"] = "Se ha agregado la nueva materia.";
                  
                    }
                    else
                    {
                        TempData["warning"] = "La materia que intenta crear ya existe.";
                                         
                    }
                }

                return RedirectToAction("Index", "ManageSubjects");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error Inesperado. Detalle del Error:" + ex;
                return View();
            }
        }
        [HttpGet]
        public IActionResult ModifySubject()
        {//MODIFICACION de MATERIA ELEGIDA Y LISTADO DE LAS COMISIONES QUE PERTENECEN A LA MISMA
            
            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"];
            }
            else if (TempData["Warning"] != null)
            {
                ViewBag.Warning = TempData["Warning"];
            }

            if (TempData["SubjectId"] != null) { ViewBag.SubjectId = TempData["SubjectId"]; }

            if (TempData["NameSubject"] != null) { ViewBag.NameSubject = TempData["NameSubject"]; }

            if (TempData["HeadTeacherId"] != null) { ViewBag.HeadTeacherId = TempData["HeadTeacherId"]; }

            if (TempData["HeadTeacherName"] != null) { ViewBag.HeadTeacherName = TempData["HeadTeacherName"]; }

            List<ClassModel> ListOfClasses = ListClasses(ViewBag.SubjectId);
           
            if (ListOfClasses.Count == 0) { ViewBag.Error = "La materia no tiene comisiones."; }

            return View(ListOfClasses);
        }

        [HttpPost]
        public IActionResult ModifySubject(int SubjectIdClass, string subjectName, int SubjectHeadTeacherId)
        {//MODIFICACION de MATERIA ELEGIDA Y LISTADO DE LAS COMISIONES QUE PERTENECEN A LA MISMA
            List<ClassModel> ListOfClasses = ListClasses(SubjectIdClass);
           
            ViewBag.NameSubject = subjectName;           
            ViewBag.SubjectId = SubjectIdClass;
            ViewBag.HeadTeacherId = SubjectHeadTeacherId;

            if (SubjectHeadTeacherId != 0)
            {
                ViewBag.HeadTeacherName = HeadTeacherSubject(SubjectHeadTeacherId);
            }
            else
            {
                ViewBag.HeadTeacherName = TempData["HeadTeacherName"];
            }

            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"];
            }
            else if (TempData["Warning"] != null)
            {
                ViewBag.Warning = TempData["Warning"];
            }

            if (TempData["SubjectId"] != null) { ViewBag.SubjectId = TempData["SubjectId"]; }

            if (TempData["NameSubject"] != null) { ViewBag.NameSubject = TempData["NameSubject"]; }

            if (ListOfClasses.Count == 0) {ViewBag.Error = "La materia no tiene comisiones."; }
           
           
            return View(ListOfClasses);
        }
       
        
        [HttpGet]
        public IActionResult EditSubject()
        { //FORMULARIO EDICIÓN DE DATOS DE CÁTEDRA
            if (TempData["SubjectId"] != null) { ViewBag.SubjectId = TempData["SubjectId"]; }
            
                    SubjectModel subjectmodified = SubjectData(ViewBag.SubjectId);          
                    ViewBag.HeadTeacherName = subjectmodified.HeadTeacherName;
                    ViewBag.NameSubject = subjectmodified.NameSubject;
                    ViewBag.HeadTeacherDni = subjectmodified.HeadTeacherDni;
                    ViewBag.IsActive = subjectmodified.isactive;
            
            return View();
        }

        [HttpPost]
        public IActionResult EditSubject(int subjectid)
        { //FORMULARIO EDICIÓN DE DATOS DE CÁTEDRA
            SubjectModel subjectmodified = SubjectData(subjectid);

            ViewBag.SubjectId = subjectid;
            ViewBag.HeadTeacherName = subjectmodified.HeadTeacherName;
            ViewBag.NameSubject = subjectmodified.NameSubject;
            ViewBag.HeadTeacherDni = subjectmodified.HeadTeacherDni;
            ViewBag.IsActive = subjectmodified.isactive;

            return View();
        }
        
        
        [HttpPost]
        public IActionResult UpdateSubject(int subjectid, string namesubject, int headteacherdni, bool isactive, string headteachername)
        {
            try
            {
                using (var db = new Models.DB.ChallengeCDBContext())
                {
                    Models.DB.Subjects updatesubject = (from d in db.Subjects
                                                  where d.IdSubjects == subjectid
                                                  select d
                                                  ).FirstOrDefault();
                    
                    int NewHeadTeacherId = (from d in db.Teachers
                                            where d.DniTeachers == headteacherdni
                                            select d.IdTeachers
                                            ).FirstOrDefault();

                    if (updatesubject != null)
                    {
                        if (NewHeadTeacherId != default) { updatesubject.IdHeadTeacher = NewHeadTeacherId; }
                        if (namesubject != default && namesubject != "") { updatesubject.NameSubjects = namesubject; }
                        updatesubject.ActiveSubjects = isactive;
                        
                        db.SaveChanges();

                    }
                    else
                    {
                        TempData["Warning"] = "La materia que intenta modificar no existe.";
                        TempData["SubjectId"] = subjectid;
                        return RedirectToAction("EditSubject", "ManageSubjects");
                    }

                    TempData["Success"] = "Cátedra Actualizada.";
                    TempData["SubjectId"] = subjectid;
                    TempData["NameSubject"] = namesubject;
                    TempData["HeadTeacherId"] = NewHeadTeacherId;
                    TempData["HeadTeacherName"] = headteachername;  
                    return RedirectToAction("ModifySubject","ManageSubjects");

                }
            }
            catch (Exception ex) 
            {
                ViewBag.Error = "Error Inesperado. Detalle del Error: " + ex;
                return RedirectToAction("EditSubject", "ManageSubjects");

            }
        }

        [HttpPost]
        public IActionResult DeleteSubject(int subjectid)
        {
            try
            {
                using (var db = new Models.DB.ChallengeCDBContext())
                {
                    var SubjectToDelete = (from d in db.Subjects
                                           where d.IdSubjects == subjectid
                                           && d.ActiveSubjects == true
                                           select d
                                            ).FirstOrDefault();
                    if (SubjectToDelete != null)
                    {
                        SubjectToDelete.ActiveSubjects = false;
                        db.SaveChanges();
                        TempData["Success"] = "Se ha eliminado la cátedra.";
                    }
                    else
                    {
                        throw new Exception("No se ha encontrado la cátedra a eliminar, intentálo de nuevo o contactate con la universidad");
                    }
                }
                return RedirectToAction("Index", "ManageSubjects");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error Inesperado. Detalle del Error: " + ex;
                return RedirectToAction("Index", "ManageSubjects");
            }
        }

 //------------------------------------------------- COMISIONES ------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult EditClass()
        {
            if (TempData["SubjectId"] != null) { ViewBag.SubjectId = TempData["SubjectId"]; }
    
            ClassModel classmodified = ClassData(ViewBag.SubjectId);

            ViewBag.SubjectId = ViewBag.SubjectId;
            ViewBag.TeacherName = classmodified.NameTeacher;
            ViewBag.NameSubject = classmodified.SubjectName;
            ViewBag.Classroom = classmodified.ClassroomClasses;
            ViewBag.TeacherDni = classmodified.TeacherDni;
            ViewBag.IsActive = classmodified.ActiveClasses;
            ViewBag.MaxStudent = classmodified.MaxStudentClasses;

            return View(ViewBag.SubjectId);
        }

        [HttpPost]
        public IActionResult EditClass(int subjectid , int classid)
        {
            ClassModel classmodified = ClassData(subjectid);

            ViewBag.SubjectId = subjectid;
            ViewBag.ClassId = classid;
            ViewBag.TeacherName = classmodified.NameTeacher;
            ViewBag.NameSubject = classmodified.SubjectName;
            ViewBag.Classroom = classmodified.ClassroomClasses;
            ViewBag.TeacherDni = classmodified.TeacherDni;
            ViewBag.IsActive = classmodified.ActiveClasses;
            ViewBag.MaxStudent = classmodified.MaxStudentClasses;

            return View(subjectid);
        }

        

        [HttpPost]
        public IActionResult UpdateClass(int subjectid,int classid, int classroom, int maxstudent, int teacherdni)
        {
            try
            {
                using (var db = new Models.DB.ChallengeCDBContext())
                {
                    Models.DB.Classes updateclass = (from d in db.Classes
                                                     where d.IdClasses == classid
                                                        select d
                                                  ).FirstOrDefault();

                    int NewTeacherId = (from d in db.Teachers
                                            where d.DniTeachers == teacherdni
                                            select d.IdTeachers
                                            ).FirstOrDefault();

                    if (updateclass != null)
                    {
                        if (NewTeacherId != default) { updateclass.IdTeachers = NewTeacherId; }
                        if (classroom != default) { updateclass.ClassroomClasses = classroom; }
                        if (maxstudent != default) { updateclass.MaxStudentClasses = maxstudent; }
                        

                        db.SaveChanges();

                    }
                    else
                    {
                        TempData["Warning"] = "La materia que intenta modificar no existe.";
                        TempData["SubjectId"] = subjectid;
                        return RedirectToAction("EditSubject", "ManageSubjects");
                    }

                    TempData["Success"] = "Comisión Actualizada.";
                    TempData["SubjectId"] = subjectid;
                    SubjectModel data = SubjectData(subjectid);
                    TempData["NameSubject"] = data.NameSubject;
                    TempData["HeadTeacherId"] = data.IdHeadTeacher;
                    TempData["HeadTeacherName"] = data.HeadTeacherName;
                    
                    return RedirectToAction("ModifySubject", "ManageSubjects");

                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error Inesperado. Detalle del Error: " + ex;
                return RedirectToAction("EditSubject", "ManageSubjects");

            }
        }

        [HttpPost]
        public IActionResult NewClass(int subjectid) 
        {
           return View(subjectid);
        }

        [HttpPost]
        public IActionResult CreateClass(int subjectid, int classroom, 
            int teacherdni, int maxstudent)
        {     //ENVIO DEL FORMULARIO DE LA NUEVA COMISION, CORROBORACIÓN y CREACIÓN DE LA MISMA.
            try
            {
                using (var db = new Models.DB.ChallengeCDBContext())
                {
                    var newclass = (from d in db.Classes
                                      where d.ClassroomClasses == classroom
                                      select d
                                      ).FirstOrDefault();
                    if (newclass == null)
                    {
                        Models.DB.Classes AddClass = new Models.DB.Classes();

                        AddClass.ClassroomClasses = classroom;
                        AddClass.MaxStudentClasses = maxstudent;
                        AddClass.IdSubjects = subjectid;
                        AddClass.ActiveClasses = true;
                        AddClass.MaxCapacityClasses = false;
                        AddClass.IdTeachers = (from d in db.Teachers
                                               where d.DniTeachers == teacherdni
                                               && d.ActiveTeachers == true
                                               select d.IdTeachers).FirstOrDefault();

                        if (AddClass.IdTeachers == default)
                        {
                            TempData["warning"] = "El DNI ingresado no corresponde a un profesor registrado.";
                            return RedirectToAction("Index", "ManageSubjects");
                        }

                        db.Classes.Add(AddClass);
                        db.SaveChanges();

                        TempData["Success"] = "Se ha agregado la nueva materia.";
                      
                    }
                    else
                    {
                        TempData["warning"] = "La materia que intenta crear ya existe.";
                                         
                        
                    }
                }
              // TempData["SubjectId"]= subjectid;
               return RedirectToAction("Index","ManageSubjects");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error Inesperado. Detalle del Error:" + ex;

               // TempData["SubjectId"] = subjectid;
                return RedirectToAction("Index", "ManageSubjects");
                //return View();
            }
        }
        [HttpPost]
        public IActionResult DeleteClass(int classid)
        {
            try
            {
                using (var db = new Models.DB.ChallengeCDBContext())
                {
                    var ClassToDelete = (from d in db.Classes
                                           where d.IdClasses == classid
                                           && d.ActiveClasses == true
                                           select d
                                            ).FirstOrDefault();
                    if (ClassToDelete != null)
                    {
                        ClassToDelete.ActiveClasses = false;
                        db.SaveChanges();
                        TempData["Success"] = "Se ha eliminado la comisión.";
                    }
                    else
                    {
                        throw new Exception("No se ha encontrado la comisión a eliminar, intentálo de nuevo o contactate con la universidad");
                    }
                }
                return RedirectToAction("Index", "ManageSubjects");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error Inesperado. Detalle del Error: " + ex;
                return RedirectToAction("Index", "ManageSubjects");
            }
        }
        
        // ------------------------------------------------   FUNCIONES     -----------------------------------------------------------------------------
        public ClassModel ClassData(int subjectid)
        {
            ClassModel ClassData = new ClassModel();
            try
            {
                using (var db = new Models.DB.ChallengeCDBContext())
                {
                    ClassData = (from d in db.Classes
                                 where d.IdSubjects == subjectid
                                 select new ClassModel
                                 {
                                     IdSubjects = subjectid,
                                     ClassroomClasses = d.ClassroomClasses,
                                     ActiveClasses = d.ActiveClasses,
                                     MaxStudentClasses = d.MaxStudentClasses,
                                     TeacherDni =
                                     (from c in db.Teachers
                                      where c.IdTeachers == d.IdTeachers
                                      select c.DniTeachers
                                      ).FirstOrDefault(),
                                     NameTeacher =
                                     (from e in db.Teachers
                                      where e.IdTeachers == d.IdTeachers
                                      select new string
                                      (e.NameTeachers + " " + e.LastNameTeachers)
                                      ).FirstOrDefault(),
                                     SubjectName =
                                     (from f in db.Subjects
                                      where f.IdSubjects == subjectid
                                      select f.NameSubjects
                                      ).FirstOrDefault(),
                                 }).FirstOrDefault();
                }
                return ClassData;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error Inesperado. Detalle del Error: " + ex;

                return ClassData;
            }
        }

        public List<ClassModel> ListClasses(int IdSubject)
        { //Función que genera listado de comisiones pertenecientes a una materia
            List<ClassModel> ClassesList = new List<ClassModel>();
            try
            {
                using (var db = new Models.DB.ChallengeCDBContext())
                {
                    ClassesList = (from d in db.Classes
                                   where d.IdSubjects == IdSubject
                                   && d.ActiveClasses == true
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

        public List<SubjectModel> ListSubjects() //Función que trae LISTADO DE MATERIAS 
        {
            List<SubjectModel> SubjectsList = new List<SubjectModel>();
            try
            {
                using (var db = new Models.DB.ChallengeCDBContext())
                {
                    SubjectsList = (from d in db.Subjects
                                    where d.NameSubjects != ""
                                    && d.ActiveSubjects == true
                                    select new SubjectModel
                                    {
                                        Id = d.IdSubjects,
                                        NameSubject = d.NameSubjects,
                                        IdHeadTeacher = d.IdHeadTeacher

                                    }).ToList();

                }
                return SubjectsList;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error Inesperado. Detalle del Error: " + ex;

                return SubjectsList;
            }
        }
        public SubjectModel SubjectData(int subjectid)
        {
            SubjectModel subjectData = new SubjectModel();
            try
            {
                using (var db = new Models.DB.ChallengeCDBContext())
                {
                    subjectData = (from d in db.Subjects
                                   where d.IdSubjects == subjectid
                                   select new SubjectModel
                                   {
                                       Id = d.IdSubjects,
                                       NameSubject = d.NameSubjects,
                                       isactive = d.ActiveSubjects,
                                       IdHeadTeacher = d.IdHeadTeacher,
                                       HeadTeacherDni =
                                       (from c in db.Teachers
                                        where c.IdTeachers == d.IdHeadTeacher
                                        select c.DniTeachers
                                        ).FirstOrDefault(),
                                       HeadTeacherName =
                                       (from c in db.Teachers
                                        where c.IdTeachers == d.IdHeadTeacher
                                        select new string
                                        (c.NameTeachers + " " + c.LastNameTeachers)
                                        ).FirstOrDefault()
                                   }).FirstOrDefault();
                }
                return subjectData;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error Inesperado. Detalle del Error: " + ex;

                return subjectData;
            }
        }

        public string HeadTeacherSubject(int SubjectHeadTeacherId)
        {
            string headTeacher = null;
            try
            {

                using (var db = new Models.DB.ChallengeCDBContext())
                {
                    headTeacher = (from d in db.Teachers
                                   where d.IdTeachers == SubjectHeadTeacherId
                                   select new string
                                   (d.NameTeachers + " " + d.LastNameTeachers)
                                   ).FirstOrDefault();
                }
                return headTeacher;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error Inesperado. Detalle del Error: " + ex;

                return headTeacher;
            }
        }
    }
}
