using AspNetIdentity.Data;
using AspNetIdentity.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Serilog;
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

        private AppDbContext _db;
        private ILogger _logger;
        public HomeController(AppDbContext db, ILogger logger)
        {
            _db = db;
            _logger = logger;
        }

        private UserManager<AppUser> UserManager => HttpContext.GetOwinContext().GetUserManager<UserManager<AppUser>>();
        private RoleManager<IdentityRole> RoleManager => HttpContext.GetOwinContext().GetUserManager<RoleManager<IdentityRole>>();
        // GET: Home
        public ActionResult Index()
        {

            return View();
        }
    }
}