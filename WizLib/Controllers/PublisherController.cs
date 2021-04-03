using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WizLib_DataAccess.Data;
using WizLib_Model.Models;

namespace WizLib.Controllers
{
    public class PublisherController : Controller
    {
        private readonly ApplicationDbContext db;

        public PublisherController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            List<Publisher> objList = db.Publishers.ToList();

            return View(objList);
        }

        public IActionResult Upsert(int? id)
        {
            Publisher obj = new Publisher();

            if (id == null) // New
            {
                return View(obj);
            }
            else // Edit
            {
                obj = db.Publishers.FirstOrDefault(u => u.Publisher_Id == id);

                if (obj == null)
                    return NotFound();

                return View(obj);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Publisher obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Publisher_Id == 0) // Create
                {
                    db.Publishers.Add(obj);
                }
                else // Update
                {
                    db.Publishers.Update(obj);
                }
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(obj);
        }

        public IActionResult Delete(int id)
        {
            var category = db.Publishers.FirstOrDefault(x => x.Publisher_Id == id);
            if (category != null)
            {
                db.Publishers.Remove(category);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

    }
}