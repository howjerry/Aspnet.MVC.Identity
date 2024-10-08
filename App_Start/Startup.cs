﻿using AspNetIdentity.Data;
using AspNetIdentity.Data.Entity;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

[assembly: OwinStartup(typeof(AspNetIdentity.App_Start.Startup))]

namespace AspNetIdentity.App_Start
{
    public class Startup
    {
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
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<UserManager<AppUser>, AppUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: async (manager, user) => await manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie))
                }
            });

            app.AddAutofacConfigurations();
              
            //NOTE: 新增角色
            try
            {
                HttpContext.Current.GetOwinContext().GetUserManager<RoleManager<IdentityRole>>().CreateAsync(new IdentityRole("Guest"));
            }
            catch 
            {
                DependencyResolver.Current.GetService<ILogger>().Warning("Role 資料已經存在");
            }
        }
    }

    public static class AppBuilderExtensions
    {
        //NOTE: CreatePerOwinContext註冊一個靜態回調，您的應用程序將使用該回調來取回指定類型的新實例。
        //      此回調將在每個請求中調用一次，並將對象 / 對象存儲在OwinContext中，以便您能夠在整個應用程序中使用它們。
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


                // 設定 user 鎖定相關配置
                manager.UserLockoutEnabledByDefault = true;
                manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
                manager.MaxFailedAccessAttemptsBeforeLockout = 5;

                // 設定雙因子驗證 (two factor authentication). 這邊示範使用簡訊及 Emails
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

        public static IAppBuilder AddAutofacConfigurations(this IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            // STANDARD MVC SETUP:

            // Register your MVC controllers.
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            //Register your Service
            //==========================================================================================
            builder.RegisterType<AppDbContext>()
                    .As<AppDbContext>()
                    .InstancePerLifetimeScope();
            var columnOptions = new ColumnOptions
            {
                AdditionalColumns = new Collection<SqlColumn>
                    {
                        new SqlColumn
                            {ColumnName = "UserName", PropertyName = "UserName", DataType = SqlDbType.NVarChar, DataLength = 256},
                    }
            };
            builder.Register<ILogger>((c, p) =>
            {
                return new LoggerConfiguration()
                  //.Enrich.WithProperty("UserName", HttpContext.Current.User.Identity.Name)
                  .Enrich.With<BaseEnricher>()
                  .WriteTo
                  .MSSqlServer(
                                connectionString: WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString,
                                sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs" },
                                columnOptions: columnOptions)
                  .CreateLogger();
            }).SingleInstance();
            //==========================================================================================
            // Run other optional steps, like registering model binders,
            // web abstractions, etc., then set the dependency resolver
            // to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // OWIN MVC SETUP:
            // Register the Autofac middleware FIRST, then the Autofac MVC middleware.
            app.UseAutofacMiddleware(container);
            app.UseAutofacMvc();

            return app;
        }
    }
    /// <summary>
    /// Serilog 擴增欄位配置
    /// </summary>
    public class BaseEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var userName = HttpContext.Current.User?.Identity.Name;
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserName", userName));
        }
    }
}
