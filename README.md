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
[code](App_Start/Startup.cs)

## 註冊
[code](Areas/Account/Controllers/SignUpController.cs)
## 登入
[code](Areas/Account/Controllers/LoginController.cs)
## 登出
[code](Areas/Account/Controllers/LogoutController.cs)

