using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudentskaSluzba.Data;
using StudentskaSluzba.Models;

namespace StudentskaSluzba.Controllers
{
    public class PredmetController : Controller
    {
        private readonly ApplicationDbContext _db;
        public PredmetController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(string pretraga)
        {
            List<Predmet> predmeti;
            if (!string.IsNullOrEmpty(pretraga))
            {
                predmeti = _db.Predmet.Where(p => p.Naziv.ToLower().StartsWith(pretraga.ToLower())).ToList();
            }
            else
            {
                predmeti = _db.Predmet.ToList();
            }
            ViewData["predmeti"] = predmeti;
            ViewData["pretraga"] = pretraga;

            return View();
        }

        public IActionResult AddPredmet()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPredmet(Predmet predmet)
        {
            if (ModelState.IsValid)
            {
                _db.Predmet.Add(predmet);
                _db.SaveChanges();
            }

            return RedirectToAction("AddPredmet");
        }

        public IActionResult Delete(int? id)
        {
            if (id != null || id != 0)
            {
                var obj = _db.Predmet.Find(id);
                _db.Remove(obj);
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            if (id != null || id != 0)
            {
                var predmet = _db.Predmet.Find(id);
                return View(predmet);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Predmet predmet)
        {
            if (ModelState.IsValid)
            {
                if (predmet != null)
                {
                    _db.Update(predmet);
                    _db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
    }
}