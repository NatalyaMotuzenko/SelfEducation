using EvergreenLibrary.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace EvergreenLibrary.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/roles")]
    public class RolesController : BaseApiController
    {

        [Route("{id:guid}", Name = "GetRoleById")]
        public async Task<IHttpActionResult> GetRole(string Id)
        {
            var role = await AppRoleManager.FindByIdAsync(Id).ConfigureAwait(false);

            if (role != null)
            {
                return Ok(TheModelFactory.Create(role));
            }

            return NotFound();

        }

        [Route("", Name = "GetAllRoles")]
        public IHttpActionResult GetAllRoles()
        {
            var roles = this.AppRoleManager.Roles;

            return Ok(roles);
        }

        [Route("create")]
        public async Task<IHttpActionResult> Create(CreateRoleBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = new IdentityRole { Name = model.Name };

            var result = await AppRoleManager.CreateAsync(role).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            Uri locationHeader = new Uri(Url.Link("GetRoleById", new { id = role.Id }));

            return Created(locationHeader, TheModelFactory.Create(role));

        }

        [Route("{id:guid}")]
        public async Task<IHttpActionResult> DeleteRole(string Id)
        {

            var role = await AppRoleManager.FindByIdAsync(Id).ConfigureAwait(false);

            if (role != null)
            {
                IdentityResult result = await AppRoleManager.DeleteAsync(role).ConfigureAwait(false);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                return Ok();
            }

            return NotFound();

        }

        [Route("ManageUsersInRole")]
        public async Task<IHttpActionResult> ManageUsersInRole(UsersInRoleModel model)
        {
            var role = await AppRoleManager.FindByIdAsync(model.Id).ConfigureAwait(false);

            if (role == null)
            {
                ModelState.AddModelError(string.Empty, "Role does not exist");
                return BadRequest(ModelState);
            }

            foreach (string user in model.EnrolledUsers)
            {
                var appUser = await AppUserManager.FindByIdAsync(user).ConfigureAwait(false);

                if (appUser == null)
                {
                    ModelState.AddModelError(string.Empty, $"User: {user} does not exists");
                    continue;
                }

                if (!this.AppUserManager.IsInRole(user, role.Name))
                {
                    IdentityResult result = await AppUserManager.AddToRoleAsync(user, role.Name).ConfigureAwait(false);

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, $"User: {user} could not be added to role");
                    }

                }
            }

            foreach (string user in model.RemovedUsers)
            {
                var appUser = await AppUserManager.FindByIdAsync(user).ConfigureAwait(false);

                if (appUser == null)
                {
                    ModelState.AddModelError(string.Empty, $"User: {user} does not exists");
                    continue;
                }

                IdentityResult result = await AppUserManager.RemoveFromRoleAsync(user, role.Name).ConfigureAwait(false);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, $"User: {user} could not be removed from role");
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }
    }
}
