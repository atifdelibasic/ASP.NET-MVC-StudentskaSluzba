using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StudentskaSluzba.Data;
using StudentskaSluzba.Helper;
using StudentskaSluzba.Models;

namespace StudentskaSluzba.Controllers
{
    [Autorizacija(referent: true, student: false)]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _db;

        public StudentController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(string pretraga)
        {
            KorisnickiNalog korisnik = HttpContext.GetLogiraniKorisnik();

            if(korisnik == null)
            {
                TempData["error_poruka"] = "Nemate pravo pristupa"; 
                return RedirectToAction("Login", "Autentifikacija");
            }

            List<Student> list = new List<Student>();
            if (!string.IsNullOrEmpty(pretraga))
            {
                list = _db.Student.Where(s => s.Ime.ToLower().StartsWith(pretraga.ToLower()) || s.Prezime.ToLower().StartsWith(pretraga.ToLower()))
                    .Include(s => s.GodinaStudija).Include(s => s.Grad).ToList();
            }
            else
            {
                list = _db.Student.Include(s => s.GodinaStudija).Include(s => s.Grad).ToList();
            }
            ViewData["pretragakey"] = pretraga;

            return View(list);
        }

        public IActionResult Add()
        {
            IEnumerable<Grad> gradovi = _db.Grad.ToList();
            IEnumerable<GodinaStudija> godine = _db.GodinaStudja.ToList();

            ViewData["gradkey"] = gradovi;
            ViewData["godinekey"] = godine;

            return View();
        }

        // POST add new Student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Student student)
        {
            if (ModelState.IsValid)
            {
                _db.Student.Add(student);
                _db.SaveChanges();
                return RedirectToAction("UspjesanAdd", new { id = student.Id });
            }
            IEnumerable<Grad> gradovi = _db.Grad.ToList();
            IEnumerable<GodinaStudija> godine = _db.GodinaStudja.ToList();

            ViewData["gradkey"] = gradovi;
            ViewData["godinekey"] = godine;

            return View(student);
        }
        public IActionResult UspjesanAdd(int? id)
        {
            var student = _db.Student.Find(id);

            ViewData["student"] = student;
            return View();
        }
        // DELETE Student
        public IActionResult Delete(int? id)
        {
            if (id != null)
            {
                var obj = _db.Student.Find(id);
                _db.Remove(obj);
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            Student obj = _db.Student
                        .Include(g => g.GodinaStudija)
                        .Include(gr => gr.Grad)
                        .FirstOrDefault(a => a.Id == id);

            IEnumerable<Grad> gradovi = _db.Grad.ToList();
            IEnumerable<GodinaStudija> godine = _db.GodinaStudja.ToList();

            ViewData["gradkey"] = gradovi;
            ViewData["godinekey"] = godine;

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student s)
        {
            if (ModelState.IsValid)
            {
                _db.Student.Update(s);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            Student obj = _db.Student
                .Include(g => g.Grad)
                .Include(god => god.GodinaStudija)
                .FirstOrDefault(st => st.Id == s.Id);

            IEnumerable<Grad> gradovi = _db.Grad.ToList();
            IEnumerable<GodinaStudija> godine = _db.GodinaStudja.ToList();

            ViewData["gradkey"] = gradovi;
            ViewData["godinekey"] = godine;

            //return RedirectToAction("Edit", new { s.Id });
            return View(obj);
        }

        public IActionResult Uspjeh(int? id)
        {

            //var student = _db.Student
            //        .Include(s => s.Uspjeh.Select(p => p.Predmet))
            //        .SingleOrDefault(s => s.Id == id);

            var student = _db.Student
                        .Include(s => s.Uspjeh)
                        .ThenInclude(s => s.Predmet)
                        .SingleOrDefault(s => s.Id == id);

            return View(student);
        }

        public IActionResult AddUspjeh(int? id)
        {
            if (id != null || id != 0)
            {
                ViewData["id"] = id;
            }

            List<Predmet> predmeti = _db.Predmet.ToList();
            ViewData["predmeti"] = predmeti;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUspjeh(StudentiPredmeti uspjeh)
        {
            if (ModelState.IsValid)
            {
                uspjeh.Id = 0;

                _db.Uspjeh.Add(uspjeh);
                _db.SaveChanges();
            }

            return RedirectToAction("AddUspjeh", new { id = uspjeh.StudentId });
        }

        public IActionResult DeleteUspjeh(int? id)
        {
            if (id != null || id != 0)
            {
                var obj = _db.Uspjeh.Find(id);
                _db.Remove(obj);
                _db.SaveChanges();
                return RedirectToAction("Uspjeh", new { id = obj.StudentId });
            }

            return RedirectToAction("Index");
        }

        public IActionResult EditUspjeh(int? id)
        {
            StudentiPredmeti uspjeh = _db.Uspjeh
                    .Include(u => u.Predmet)
                    .Include(u => u.Student)
                    .FirstOrDefault(s => s.Id == id);

            ViewData["predmeti"] = _db.Predmet.ToList();

            return View(uspjeh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUspjeh(StudentiPredmeti obj)
        {
            if (ModelState.IsValid)
            {
                _db.Update(obj);
                _db.SaveChanges();

                return RedirectToAction("Uspjeh", new { id = obj.StudentId });
            }

            return View(obj);
        }

    }
}