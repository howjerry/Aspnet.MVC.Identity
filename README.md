# Aspnet.MVC.Identity
ASP NET MVC Identity 2.0 Sample

## Add Package

*	Microsoft.AspNet.Identity.Owin

*	Microsoft.AspNet.Identity.EntityFramework 

*	Microsoft.Owin.Host.SystemWeb 

## Entity Framework DbContext
```c#
namespace AspNetIdentity.Data
{
    public class AppDbContext : IdentityDbContext
    {

    }
}
```

## Custom
>不客製跳至 Migration DataBase

### 修改資料表名

```c#
namespace AspNetIdentity.Data
{
    public class AppDbContext : IdentityDbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUser>().ToTable("AppUser");
            modelBuilder.Entity<IdentityRole>().ToTable("AppRole");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("AppUserClaim");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("AppUserLogin");
            modelBuilder.Entity<IdentityUserRole>().ToTable("AppUserRole");
    
        }
    }
}
```

### 客製 User
新增 AppUser 並繼承 IdentityUser
```C#
namespace AspNetIdentity.Data.Entity
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
```

修改 DbContext 繼承 IdentityDbContext 並修改 IdentityUser 為 AppUser
```C#
namespace AspNetIdentity.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>().Property(x => x.Name).HasMaxLength(256).IsRequired().IsUnicode(true);
            modelBuilder.Entity<AppUser>().Property(x => x.IsDeleted).IsRequired();
            modelBuilder.Entity<AppUser>().Property(x => x.CreateDateTime).IsRequired();
            //...
        }
    }
}
```

## Migration Database
### Set ConnectionString

```xml
<connectionStrings>
	<add name="DefaultConnection" connectionString="Data Source=.;Initial Catalog=AspNetIdentity;Trusted_Connection=True" providerName="System.Data.SqlClient"/>
</connectionStrings>
```

1. Enable-Migrations
2. Add-Migration init
3. Upage-Database

## StartUp

App_Start 資料夾下新增項目，OWIN 啟動類別，命名Startup.cs

新增 class AppBuilderExtensions ，設定配置

```c#
    public static class AppBuilderExtensions
    {
        public static IAppBuilder AddIdentityContexts(this IAppBuilder app)
        {
            app.CreatePerOwinContext(() => new AppDbContext());
            app.CreatePerOwinContext<UserManager<AppUser>>((options, context) =>
            {
                var manager = new UserManager<AppUser>(new UserStore<AppUser>(context.Get<AppDbContext>()));
                // 設定 usernames 驗證邏輯
                manager.UserValidator = new UserValidator<AppUser>(manager)
                {
                    //帳號需要有字母與數字組合
                    AllowOnlyAlphanumericUserNames = false,
                    //Email 需是未註冊過的
                    RequireUniqueEmail = true,
                };

                // 設定密碼驗證邏輯
                manager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 6,
                    //必須有特殊符號
                    RequireNonLetterOrDigit = false,
                    //必須要有數字
                    RequireDigit = true,
                    //必須要有小寫字母
                    RequireLowercase = false,
                    //必須要有大寫字母
                    RequireUppercase = false,
                };


                // 設定 user 預設鎖定
                manager.UserLockoutEnabledByDefault = true;
                manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
                manager.MaxFailedAccessAttemptsBeforeLockout = 5;

                // 設定兩因子驗證 (two factor authentication). 這邊示範使用簡訊及 Emails
                //manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<Account>
                //{
                //    MessageFormat = "Your security code is {0}"
                //});
                //manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<Account>
                //{
                //    Subject = "Security Code",
                //    BodyFormat = "Your security code is {0}"
                //});
                // 設定 Email 服務
                // manager.EmailService = new EmailService();
                // 設定簡訊服務
                // manager.SmsService = new SmsService();
                var dataProtectionProvider = options.DataProtectionProvider;
                if (dataProtectionProvider != null)
                {
                    manager.UserTokenProvider =
                        new DataProtectorTokenProvider<AppUser>(dataProtectionProvider.Create("ASP.NET Identity"));
                }

                return manager;
            });
            app.CreatePerOwinContext<SignInManager<AppUser, string>>((options, context) => new SignInManager<AppUser, string>(context.GetUserManager<UserManager<AppUser>>(), context.Authentication));
            app.CreatePerOwinContext<RoleManager<IdentityRole>>((options, context) => new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context.Get<AppDbContext>())));

            return app;
        }
    }
```

```c#
public void Configuration(IAppBuilder app)
{
    app.AddIdentityContexts();
    // 讓應用程式使用 Cookie 儲存已登入使用者的資訊
    // 並使用 Cookie 暫時儲存使用者利用協力廠商登入提供者登入的相關資訊；
    // 在 Cookie 中設定簽章
    app.UseCookieAuthentication(new CookieAuthenticationOptions
    {
        CookieName = "IdentityCookie",
        AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie, //資料存放的方式
        LoginPath = new PathString("/Account/Login"), //登入頁面的路徑
        //可設定驗證的時效，以及過期時可以透過regenerateIdentity內的callback方法重新驗證
        Provider = new CookieAuthenticationProvider
        {
            // 讓應用程式在使用者登入時驗證安全性戳記。
            // 這是您變更密碼或將外部登入新增至帳戶時所使用的安全性功能。  
            OnValidateIdentity = 
                SecurityStampValidator.OnValidateIdentity<UserManager<AppUser>, AppUser>(
                validateInterval: TimeSpan.FromMinutes(30),
                regenerateIdentity: async (manager, user) => 
                await manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie))
        }
    });
}
```

## 註冊

```c#
using AspNetIdentity.Data.Entity;
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
```

