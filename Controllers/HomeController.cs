using AspNetIdentity.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspNetIdentity.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private UserManager<AppUser> UserManager => HttpContext.GetOwinContext().GetUserManager<UserManager<AppUser>>();
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Roles = UserManager.GetRoles(User.Identity.GetUserId());

            return View();
        }
    }
}