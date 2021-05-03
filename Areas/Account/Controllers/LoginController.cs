using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AspNetIdentity.Data.Entity;
using AspNetIdentity.Models;
using Microsoft.AspNet.Identity.Owin;

namespace AspNetIdentity.Areas.Account.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        public SignInManager<AppUser, string> SignInManager => HttpContext.GetOwinContext().Get<SignInManager<AppUser, string>>();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AccountLoginModel request)
        {
            if (ModelState.IsValid)
            {
                var result = await SignInManager.PasswordSignInAsync(request.UserName, request.Password, request.Remember, true);
                switch (result)
                {
                    case SignInStatus.Success:
                        return Redirect("/Home");
                    case SignInStatus.LockedOut:
                        ModelState.AddModelError("", "多次異常操作，帳號已被被鎖住五分鐘。");
                        break;
                    case SignInStatus.RequiresVerification://雙因子驗證，電子郵件、手機簡訊
                        ModelState.AddModelError("", "帳號驗證未完成。");
                        break;
                    case SignInStatus.Failure:
                        ModelState.AddModelError("", "使用者名稱或密碼錯誤");
                        break;
                    default:
                        break;
                }
            }

            return View();
        }
    }
}