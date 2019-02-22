using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EvergreenLibrary.Infrastructure;
using EvergreenLibrary.Models;

namespace EvergreenLibrary.Controllers
{

    [RoutePrefix("api/books")]
    public class BooksController : BaseApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Librarian,Customer")]
        [Route("", Name = "GetAllBooks")]
        public IHttpActionResult GetBooks()
        {
            return Ok(db.Books.ToList().Where((book)=>book.NeedToDelete==false).Select(b => this.TheModelFactory.Create(b)));
        }

        [Authorize(Roles = "Librarian")]
        [Route("{id}", Name = "GetBookById")]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            Book book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(this.TheModelFactory.Create(book));
        }

        [Authorize(Roles = "Librarian")]
        [Route("{id}", Name = "PutBook")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBook(int id, PutBookBindingModel putBook)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Book book = await db.Books.FindAsync(putBook.Id);
            book.Title = putBook.Title;
            book.Author = putBook.Author;
            book.Year = putBook.Year;

            if (id != book.Id)
            {
                return BadRequest();
            }

            db.Entry(book).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        [Authorize(Roles = "Librarian")]
        [Route("", Name = "Create")]
        public async Task<IHttpActionResult> PostBook(CreateBookBindingModel createBook)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var book = new Book
            {
                Title = createBook.Title,
                Author = createBook.Author,
                Year = createBook.Year
            };

            db.Books.Add(book);
            await db.SaveChangesAsync();

            Uri locationHeader = new Uri(Url.Link("GetBookById", new { id = book.Id }));

            return Created(locationHeader, TheModelFactory.Create(book));
        }

        [Authorize(Roles = "Librarian")]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteBook(int id)
        {
            Book book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            if (book.ApplicationUserId != null)
            {
                book.NeedToDelete = true;
                db.Entry(book).State = EntityState.Modified;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                db.Books.Remove(book);
                await db.SaveChangesAsync();
            }
            return Ok(book);
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        private bool BookExists(int id)
        {
            return db.Books.Count(e => e.Id == id) > 0;
        }
    }
}