using AspNetIdentity.Data.Entity;
using CHC.ToastrNotify;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AspNetIdentity.Areas.Account.Controllers
{
    [AllowAnonymous]
    public class ConfirmEmailController : Controller
    {
        private UserManager<AppUser> UserManager => HttpContext.GetOwinContext().GetUserManager<UserManager<AppUser>>();

        public async Task<ActionResult> Index(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Error", new { area = "" });
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                NotifyHelper.Append(NotifyType.Success, "電子郵件驗證成功");
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return RedirectToAction("Index", "Error", new { area = "" });
            }
        }
    }
}