using AspNetIdentity.Areas.Account.Models;
using AspNetIdentity.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspNetIdentity.Areas.Account.Controllers
{
    public class RoleController : Controller
    {

        private AppDbContext _db;
        private ILogger _logger;
        public RoleController(AppDbContext db, ILogger logger)
        {
            _db = db;
            _logger = logger;
        }
        // GET: Account/Role
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult List()
        {
            var response = new List<AccountRoleIndexData>();
            try
            {
                response = _db.Roles.Select(x => new AccountRoleIndexData { Id = x.Id, Name = x.Name }).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}