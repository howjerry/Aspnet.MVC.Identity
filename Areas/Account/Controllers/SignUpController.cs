﻿using AspNetIdentity.Data.Entity;
using AspNetIdentity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AspNetIdentity.Areas.Account.Controllers
{
    [AllowAnonymous]
    public class SignUpController : Controller
    {
        private UserManager<AppUser> UserManager => HttpContext.GetOwinContext().GetUserManager<UserManager<AppUser>>();
        

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AccountSignUpModel request)
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
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item);
                }
                
                return View(request);
            }

            return Redirect("/Login");
        }
    }
}