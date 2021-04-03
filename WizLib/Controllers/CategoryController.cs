using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WizLib_DataAccess.Data;
using WizLib_Model.Models;

namespace WizLib.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext db;

        public CategoryController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {

            List<Category> objList = db.Categories.ToList();

            return View(objList);
        }

        public IActionResult Upsert(int? id)
        {
            Category obj = new Category();

            if(id == null) // New
            {
                return View(obj);
            }
            else // Edit
            {
                obj = db.Categories.FirstOrDefault(u => u.Category_Id == id);

                if (obj == null)
                    return NotFound();

                return View(obj);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category obj)
        {
            if (ModelState.IsValid)
            {
                if(obj.Category_Id == 0) // Create
                {
                    db.Categories.Add(obj);
                }
                else // Update
                {
                    db.Categories.Update(obj);
                }
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(obj);
        }

        public IActionResult Delete(int id)
        {
            var category = db.Categories.FirstOrDefault(x => x.Category_Id == id);
            if(category != null)
            {
                db.Categories.Remove(category);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
            
            
        }
    }
}
