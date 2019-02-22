using EvergreenLibrary.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace EvergreenLibrary.Controllers
{
    [Authorize(Roles = "Customer")]
    [RoutePrefix("api/users/{userId}/books")]
    public class UsersBooksController : BaseApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Route("", Name = "GetAllUserBooks")]
        public async Task<IHttpActionResult> GetBooks(string userId)
        {
            var user = await this.AppUserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.Books.ToList().Select(b => this.TheModelFactory.Create(b)));
        }

        [Route("{id}", Name = "PutUserBook")]
        public async Task<IHttpActionResult> PutUserBook(string userId, int id,[FromUri] bool take)
        {
            var user = await this.AppUserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            Book book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            if (take == false)
            {
                if (book.ApplicationUserId != userId)
                {
                    return BadRequest("Sorry, but you do not have this book");
                }
                if (book.NeedToDelete == true)
                {
                    db.Books.Remove(book);
                    await db.SaveChangesAsync();
                    return Ok();
                }
                book.ApplicationUserId = null;
            }
            else
            {
                if (book.ApplicationUserId != null)
                {
                    if (book.ApplicationUserId == userId)
                        return BadRequest("Sorry, but you have already taken this book");
                    return BadRequest("Sorry, but this book is already taken");
                }
                book.ApplicationUserId = userId;
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

        private bool BookExists(int id)
        {
            return db.Books.Count(e => e.Id == id) > 0;
        }

    }
}
