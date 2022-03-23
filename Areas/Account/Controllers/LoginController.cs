using AspNetIdentity.Areas.Account.Models;
using AspNetIdentity.Data;
using AspNetIdentity.Data.Entity;
using CHC.ToastrNotify;
using Microsoft.AspNet.Identity.Owin;
using Serilog;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AspNetIdentity.Areas.Account.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private AppDbContext _db;
        private ILogger _logger;
        private SignInManager<AppUser, string> SignInManager => HttpContext.GetOwinContext().Get<SignInManager<AppUser, string>>();

        public LoginController(ILogger logger, AppDbContext db)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AccountLoginModel request)
        {
            if (!ModelState.IsValid) return View(request);

            var result = await SignInManager.PasswordSignInAsync(request.UserName, request.Password, request.RememberMe, true);
            switch (result)
            {
                case SignInStatus.Success:
                    NotifyHelper.Append(NotifyType.Info, "歡迎回來");
                    return Redirect("/Home");
                case SignInStatus.LockedOut:
                    ModelState.AddModelError("", "多次異常操作，帳號已被被鎖住五分鐘。");
                    return View(request);
                case SignInStatus.RequiresVerification://雙因子驗證，電子郵件、手機簡訊
                    return RedirectToAction("SendCode", new { ReturnUrl = request.ReturnUrl, RememberMe = request.RememberMe });
                case SignInStatus.Failure:
                    ModelState.AddModelError("", "使用者名稱或密碼錯誤");
                    return View(request);
                default:
                    ModelState.AddModelError("", "登入嘗試失試。");
                    return View(request);
            }
        }
    }
}