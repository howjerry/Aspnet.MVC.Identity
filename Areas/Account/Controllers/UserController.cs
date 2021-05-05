using AspNetIdentity.Areas.Account.Models;
using AspNetIdentity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mapster;
using AspNetIdentity.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace AspNetIdentity.Areas.Account.Controllers
{
    public class UserController : Controller
    {
        private UserManager<AppUser> UserManager => HttpContext.GetOwinContext().GetUserManager<UserManager<AppUser>>();

        [HttpGet]
        public ActionResult Index()
        {
            var response = new AccountUserIndexModel();

            response.Datas = UserManager.Users.OrderByDescending(x => x.CreateDateTime).Adapt<IEnumerable<AccountUserIndexData>>().ToList();

            return View(response);
        }

        [HttpGet]
        public ActionResult List()
        {
            var response = new List<AccountUserIndexData>();

            response = UserManager.Users.OrderByDescending(x => x.CreateDateTime).Adapt<IEnumerable<AccountUserIndexData>>().ToList();

            return View("_List", response);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View("_Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountUserCreateModel request)
        {
            using (var db = new AppDbContext())
            {

                if (!ModelState.IsValid)
                {
                    return View(request);
                }

                if (!(await UserManager.PasswordValidator.ValidateAsync(request.Password)).Succeeded)
                {
                    ModelState.AddModelError("", "密碼需英數混和組合。");
                    return View(request);
                }

                var user = new AppUser()
                {
                    Name = request.Name,
                    UserName = request.UserName,
                    Email = request.Email,
                    IsDeleted = false,
                    CreateDateTime = DateTime.Now
                };


                var result = await UserManager.CreateAsync(user, request.Password);
                if (!result.Succeeded) return Json(new { status = false });
            }

            return Json(new { status = true });
        }
    }
}