using AspNetIdentity.Data.Entity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspNetIdentity.Areas.Account.Controllers
{
    [AllowAnonymous]
    public class LogoutController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();

            return Redirect("Login");
        }
    }
}