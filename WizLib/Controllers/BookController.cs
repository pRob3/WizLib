using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WizLib_DataAccess.Data;
using WizLib_Model.Models;
using WizLib_Model.ViewModels;

namespace WizLib.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext db;

        public BookController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {

            // Eager loading
            List<Book> objList = db.Books.Include(p => p.Publisher).ToList();

            //foreach (var obj in objList)
            //{
            //    // Less Efficient
            //    //obj.Publisher = db.Publishers.FirstOrDefault(p => p.Publisher_Id == obj.Publisher_Id);

            //    // More efficient
            //    // Explicted loading
            //    db.Entry(obj).Reference(p => p.Publisher).Load();
            //}

            return View(objList);
        }

        public IActionResult Upsert(int? id)
        {
            BookVM obj = new BookVM();

            obj.PublisherList = db.Publishers.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Publisher_Id.ToString()
            });

            if (id == null) // New
            {
                return View(obj);
            }
            else // Edit
            {
                obj.Book = db.Books.FirstOrDefault(u => u.Book_Id == id);

                if (obj == null)
                    return NotFound();

                return View(obj);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(BookVM obj)
        {
            if (obj.Book.Book_Id == 0) // Create
            {
                db.Books.Add(obj.Book);
            }
            else // Update
            {
                db.Books.Update(obj.Book);
            }
            db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Details(int? id)
        {
            BookVM obj = new BookVM();


            if (id == null) // New
            {
                return View(obj);
            }
            else // Edit
            {
                // Eager loading
                obj.Book = db.Books
                    .Include(x => x.BookDetail)
                    .FirstOrDefault(u => u.Book_Id == id);

                //obj.Book.BookDetail = db.BookDetails.FirstOrDefault(x => x.BookDetail_Id == obj.Book.BookDetail_Id);

                if (obj == null)
                    return NotFound();

                return View(obj);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(BookVM obj)
        {
            if (obj.Book.BookDetail.BookDetail_Id == 0) // Create
            {
                db.BookDetails.Add(obj.Book.BookDetail);
                db.SaveChanges();

                var BookFromDb = db.Books.FirstOrDefault(x => x.Book_Id == obj.Book.Book_Id);
                BookFromDb.BookDetail_Id = obj.Book.BookDetail.BookDetail_Id;

                db.SaveChanges();

            }
            else // Update
            {
                db.BookDetails.Update(obj.Book.BookDetail);
                db.SaveChanges();
            }



            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int id)
        {
            var book = db.Books.FirstOrDefault(x => x.Book_Id == id);
            if (book != null)
            {
                db.Books.Remove(book);
                db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }


        public IActionResult PlayGround()
        {

            // Deferred Execution - Play/Test
            var bookTemp = db.Books.FirstOrDefault();
            bookTemp.Price = 100;

            var bookCollection = db.Books;
            double totalPrice = 0;

            foreach (var book in bookCollection)
            {
                totalPrice += book.Price;
            }

            var bookList = db.Books.ToList();
            foreach (var book in bookList)
            {
                totalPrice += book.Price;
            }

            var bookCollection2 = db.Books;
            var bookCount1 = bookCollection2.Count();

            var bookCount2 = db.Books.Count();


            // IEnumerable vs. IQueryable
            
            // Returns all records from memory
            IEnumerable<Book> bookList1 = db.Books;
            var filteredBook1 = bookList1.Where(b => b.Price > 500).ToList();

            // Returns filtered records
            IQueryable<Book> bookList2 = db.Books;
            var filteredBook2 = bookList2.Where(b => b.Price > 500).ToList();

            // IEnumerable vs. IQueryable END


            return RedirectToAction(nameof(Index));
        }
    }
}