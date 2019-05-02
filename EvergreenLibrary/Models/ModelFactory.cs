using EvergreenLibrary.Infrastructure;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;

namespace EvergreenLibrary.Models
{
    //то, что мы вернем пользователю (что у него отобразиться на экране)
    public class ModelFactory
    {
        //private UrlHelper _UrlHelper;
        private ApplicationUserManager _AppUserManager;

        public ModelFactory(HttpRequestMessage request, ApplicationUserManager appUserManager)
        {
            //_UrlHelper = new UrlHelper(request);
            _AppUserManager = appUserManager;
        }

        public UserReturnModel Create(ApplicationUser appUser)
        {
            return new UserReturnModel
            {
                //Url = _UrlHelper.Link("GetUserById", new { id = appUser.Id }),
                Id = appUser.Id,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Email = appUser.Email,
                Roles = _AppUserManager.GetRolesAsync(appUser.Id).Result,
            };
        }

        public RoleReturnModel Create(IdentityRole appRole)
        {

            return new RoleReturnModel
            {
                //Url = _UrlHelper.Link("GetRoleById", new { id = appRole.Id }),
                Id = appRole.Id,
                Name = appRole.Name
            };
        }

        public BookReturnModel Create(Book appBook)
        {
            var state = appBook.ApplicationUser != null ? appBook.ApplicationUser.Email : "free";
            return new BookReturnModel
            {
                //Url = _UrlHelper.Link("GetBookById", new { id = appBook.Id }),
                Id = appBook.Id,
                Title = appBook.Title,
                Author = appBook.Author,
                Year = appBook.Year,
                State = state,
            };
        }
    }

    public class UserReturnModel
    {
        //public string Url { get; set; }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }

    public class RoleReturnModel
    {
        //public string Url { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class BookReturnModel
    {
        //public string Url { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public string State { get; set; }
    }

}