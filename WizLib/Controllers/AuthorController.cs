using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WizLib_DataAccess.Data;
using WizLib_Model.Models;

namespace WizLib.Controllers
{
    public class AuthorController : Controller
    {
        private readonly ApplicationDbContext db;

        public AuthorController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            List<Author> objList = db.Authors.ToList();

            return View(objList);
        }

        public IActionResult Upsert(int? id)
        {
            Author obj = new Author();

            if (id == null) // New
            {
                return View(obj);
            }
            else // Edit
            {
                obj = db.Authors.FirstOrDefault(u => u.Author_Id == id);

                if (obj == null)
                    return NotFound();

                return View(obj);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Author obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Author_Id == 0) // Create
                {
                    db.Authors.Add(obj);
                }
                else // Update
                {
                    db.Authors.Update(obj);
                }
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(obj);
        }

        public IActionResult Delete(int id)
        {
            var author = db.Authors.FirstOrDefault(x => x.Author_Id == id);
            if (author != null)
            {
                db.Authors.Remove(author);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

    }
}