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

        public IActionResult ManageAuthors(int id)
        {
            BookAuthorVM obj = new BookAuthorVM
            {
                BookAuthorList = db.BookAuthors
                    .Include(x => x.Author).Include(x => x.Book)
                    .Where(x => x.Book_Id == id).ToList(),
                BookAuthor = new BookAuthor()
                {
                    Book_Id = id
                },
                Book = db.Books.FirstOrDefault(x => x.Book_Id == id)
            };

            List<int> tempListOfAssignedAuthors = obj.BookAuthorList.Select(x => x.Author_Id).ToList();

            // NOT IN CLAUSE IN LINQ
            // Get all the authors who's id is not in temp list
            var tempList = db.Authors.Where(x => !tempListOfAssignedAuthors.Contains(x.Author_Id)).ToList();

            obj.AuthorList = tempList.Select(i => new SelectListItem
            {
                Text = i.FullName,
                Value = i.Author_Id.ToString()
            });

            return View(obj);
        }

        [HttpPost]
        public IActionResult ManageAuthors(BookAuthorVM bookAuthorVM)
        {
            if (bookAuthorVM.BookAuthor.Book_Id != 0 && bookAuthorVM.BookAuthor.Author_Id != 0)
            {
                db.BookAuthors.Add(bookAuthorVM.BookAuthor);
                db.SaveChanges();
            }

            return RedirectToAction(nameof(ManageAuthors), new { @id = bookAuthorVM.BookAuthor.Book_Id });
        }

        [HttpPost]
        public IActionResult RemoveAuthors(int authorId, BookAuthorVM bookAuthorVM)
        {
            int bookId = bookAuthorVM.Book.Book_Id;

            BookAuthor bookAuthor = db.BookAuthors
                .FirstOrDefault(x => x.Author_Id == authorId && x.Book_Id == bookId);

            db.BookAuthors.Remove(bookAuthor);
            db.SaveChanges();

            return RedirectToAction(nameof(ManageAuthors), new { @id = bookId });
        }


        public IActionResult PlayGround()
        {
            //// Deferred Execution - Play/Test
            //var bookTemp = db.Books.FirstOrDefault();
            //bookTemp.Price = 100;

            //var bookCollection = db.Books;
            //double totalPrice = 0;

            //foreach (var book in bookCollection)
            //{
            //    totalPrice += book.Price;
            //}

            //var bookList = db.Books.ToList();
            //foreach (var book in bookList)
            //{
            //    totalPrice += book.Price;
            //}

            //var bookCollection2 = db.Books;
            //var bookCount1 = bookCollection2.Count();

            //var bookCount2 = db.Books.Count();

            // IEnumerable vs. IQueryable START

            // Returns all records from memory
            IEnumerable<Book> bookList1 = db.Books;
            var filteredBook1 = bookList1.Where(b => b.Price > 500).ToList();

            // Returns filtered records
            IQueryable<Book> bookList2 = db.Books;
            var filteredBook2 = bookList2.Where(b => b.Price > 500).ToList();

            // IEnumerable vs. IQueryable END

            // Update State Manually - START

            //var category = db.Categories.FirstOrDefault();
            //db.Entry(category).State = EntityState.Modified;

            //db.SaveChanges();
            // Update State Manually - END

            // Updating related data START

            var bookTemp1 = db.Books.Include(b => b.BookDetail).FirstOrDefault(b => b.Book_Id == 2);
            bookTemp1.BookDetail.NumberOfChapters = 12345;
            db.Books.Update(bookTemp1);
            db.SaveChanges();

            var bookTemp2 = db.Books.Include(b => b.BookDetail).FirstOrDefault(b => b.Book_Id == 2);
            bookTemp1.BookDetail.Weight = 6969;
            db.Books.Attach(bookTemp2);
            db.SaveChanges();

            // Updating related data END

            return RedirectToAction(nameof(Index));
        }
    }
}