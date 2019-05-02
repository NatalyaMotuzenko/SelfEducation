using EvergreenLibrary.Infrastructure;
using EvergreenLibrary.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Transactions;
using System.Data.Entity.Infrastructure;

namespace EvergreenLibrary.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin")]
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            return Ok(this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }

        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUserById(string Id)
        {
            var user = await AppUserManager.FindByIdAsync(Id).ConfigureAwait(false);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }

        [Authorize(Roles = "Admin")]
        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await AppUserManager.FindByNameAsync(username).ConfigureAwait(false);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }

        [AllowAnonymous]
        [Route("", Name = "Register")]
        public async Task<IHttpActionResult> PostUser(CreateUserBindingModel createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new ApplicationUser()
            {
                UserName = createUserModel.Email,
                Email = createUserModel.Email,
                FirstName = createUserModel.FirstName,
                LastName = createUserModel.LastName,
            };
            var role = this.AppRoleManager.FindByName(createUserModel.RoleName);
            if (role == null)
            {
                return BadRequest("Role " + role + " does not exixts in the system");
            }

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                IdentityResult addUserResult = await AppUserManager.CreateAsync(user, createUserModel.Password).ConfigureAwait(false);

                if (!addUserResult.Succeeded)
                {
                    return GetErrorResult(addUserResult);
                }

                //add role
                IdentityResult addResult = await AppUserManager.AddToRolesAsync(user.Id, createUserModel.RoleName).ConfigureAwait(false);

                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to add user roles");
                    return BadRequest(ModelState);
                }

                //send email
                string code = await AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id).ConfigureAwait(false);

                var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new { userId = user.Id, code = code }));

                await AppUserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>").ConfigureAwait(false);
                scope.Complete();
            }
            //return the resource created in the location header and return 201 created status (хорошая практика)
            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));
            return Created(locationHeader, TheModelFactory.Create(user));
        }

        [Authorize]
        [Route("changePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await AppUserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}")]
        public  IHttpActionResult DeleteUser(string id)
        {
            var appUser = this.AppUserManager.FindById(id);

            if (appUser != null)
            {
                var userBooksId = appUser.Books.Select(a => a.Id);
                List<Book> books = new List<Book>();
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var bookId in userBooksId)
                    {
                        Book book = db.Books.Find(bookId);
                        book.ApplicationUserId = null;
                        books.Add(book);
                    }

                    IdentityResult result = this.AppUserManager.Delete(appUser);
                    if (!result.Succeeded)
                    {
                        return GetErrorResult(result);
                    }
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        scope.Complete();
                        return BadRequest();
                    }
                    finally
                    {
                        scope.Complete();
                    }
                }
                if (books.Count != 0)
                    return Ok(books.ToList().Select(u => this.TheModelFactory.Create(u)));
                else
                    return Ok();

            }

            return NotFound();

        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError(string.Empty, "User Id and Code are required");
                return BadRequest(ModelState);
            }

            IdentityResult result = await AppUserManager.ConfirmEmailAsync(userId, code).ConfigureAwait(false);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return GetErrorResult(result);
            }
        }

        //назначить роль пользователю
        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}/roles")]
        [HttpPut]
        public async Task<IHttpActionResult> AssignRolesToUser([FromUri] string id, [FromBody] string[] rolesToAssign)
        {

            var appUser = await AppUserManager.FindByIdAsync(id).ConfigureAwait(false);

            if (appUser == null)
            {
                return NotFound();
            }

            var currentRoles = await AppUserManager.GetRolesAsync(appUser.Id).ConfigureAwait(false);

            var rolesNotExists = rolesToAssign.Except(this.AppRoleManager.Roles.Select(x => x.Name)).ToArray();

            if (rolesNotExists.Count() > 0)
            {

                ModelState.AddModelError(string.Empty, errorMessage: $"Roles '{string.Join(",", rolesNotExists)}' does not exixts in the system");
                return BadRequest(ModelState);
            }

            IdentityResult removeResult = await AppUserManager.RemoveFromRolesAsync(appUser.Id, currentRoles.ToArray()).ConfigureAwait(false);

            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to remove user roles");
                return BadRequest(ModelState);
            }

            IdentityResult addResult = await AppUserManager.AddToRolesAsync(appUser.Id, rolesToAssign).ConfigureAwait(false);

            if (!addResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to add user roles");
                return BadRequest(ModelState);
            }

            return Ok();
        }
    }
}
