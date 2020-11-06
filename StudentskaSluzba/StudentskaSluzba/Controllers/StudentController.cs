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

            if (korisnik == null)
            {
                TempData["error_poruka"] = "Nemate pravo pristupa";
                return RedirectToAction("Login", "Autentifikacija");
            }

            List<Student> list = new List<Student>();
            if (!string.IsNullOrEmpty(pretraga))
            {
                list = _db.Student.Where(s => (s.Ime.ToLower() + " " + s.Prezime.ToLower()).StartsWith(pretraga.ToLower()))
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
            Student student = new Student();
            ViewData["student"] = student;

            return View("Edit");
        }

        public IActionResult Poruka(int? id)
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

        // ADD OR UPDATE
        public IActionResult Edit(int id)
        {

            IEnumerable<Grad> gradovi = _db.Grad.ToList();
            IEnumerable<GodinaStudija> godine = _db.GodinaStudja.ToList();

            ViewData["gradkey"] = gradovi;
            ViewData["godinekey"] = godine;

            Student student = id == 0 ? new Student() : _db.Student.Find(id);
            ViewData["student"] = student;

            return View("Edit");
        }

        // ADD OR UPDATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student s)
        {
            Student student;
            if (s.Id == 0)
            {
                student = new Student();
                _db.Add(student);
                TempData["poruka"] = "Uspjesno ste dodali studenta: ";
            }
            else
            {
                student = _db.Student.Find(s.Id);
                TempData["poruka"] = "Uspjesno ste editovali studenta: ";
            }

            student.Ime = s.Ime;
            student.Prezime = s.Prezime;
            student.AdresaStanovanja = s.AdresaStanovanja;
            student.GodinaStudijaId = s.GodinaStudijaId;
            student.GradId = s.GradId;
            student.JMBG = s.JMBG;
            student.DatumRodjenja = s.DatumRodjenja;
            student.Email = s.Email;

            if (!ModelState.IsValid)
            {
                ViewData["student"] = student;
                IEnumerable<Grad> gradovi = _db.Grad.ToList();
                IEnumerable<GodinaStudija> godine = _db.GodinaStudja.ToList();

                ViewData["gradkey"] = gradovi;
                ViewData["godinekey"] = godine;
                return View("Edit");
            }

            _db.SaveChanges();

            TempData["poruka"] += student.ToString();
            return RedirectToAction("Poruka");
        }

        public IActionResult Uspjeh(int? id)
        {
            var student = _db.Student
                        .Include(s => s.Uspjeh)
                        .ThenInclude(s => s.Predmet)
                        .SingleOrDefault(s => s.Id == id);

            return View(student);
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
        public IActionResult AddUspjeh(int id)
        {
            if (id != 0)
            {
                List<Predmet> predmeti = _db.Predmet.ToList();
                ViewData["predmeti"] = predmeti;

                StudentiPredmeti uspjeh = new StudentiPredmeti() { StudentId = id };
                return View("EditUspjeh", uspjeh);
            }

            return View("Index");
        }

        public IActionResult EditUspjeh(int id)
        {
            StudentiPredmeti uspjeh = _db.Uspjeh.Include(p => p.Predmet).SingleOrDefault(sp => sp.Id == id);

            ViewData["uspjeh"] = uspjeh;
            ViewData["predmeti"] = _db.Predmet.ToList();

            return View(uspjeh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUspjeh(StudentiPredmeti sp)
        {
            StudentiPredmeti uspjeh;
            if (sp.Id == 0)
            {
                uspjeh = new StudentiPredmeti();
                _db.Add(uspjeh);
            }
            else
            {
                uspjeh = _db.Uspjeh.Find(sp.Id);
            }

            uspjeh.Ocjena = sp.Ocjena;
            uspjeh.PredmetId = sp.PredmetId;
            uspjeh.StudentId = sp.StudentId;
            uspjeh.DatumPolaganja = sp.DatumPolaganja;

            if (!ModelState.IsValid)
            {
                List<Predmet> predmeti = _db.Predmet.ToList();
                ViewData["predmeti"] = predmeti;
                ViewData["uspjeh"] = uspjeh;
                return View("EditUspjeh");
            }

            _db.SaveChanges();
            return RedirectToAction("Uspjeh", new { id = uspjeh.StudentId });
        }
    }
}