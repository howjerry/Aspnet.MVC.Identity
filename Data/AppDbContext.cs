using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using AspNetIdentity.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AspNetIdentity.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppUser>().ToTable("AppUser");
            modelBuilder.Entity<AppUser>().Property(x => x.Name).HasMaxLength(256).IsRequired().IsUnicode(true);
            modelBuilder.Entity<AppUser>().Property(x => x.IsDeleted).IsRequired();
            modelBuilder.Entity<AppUser>().Property(x => x.CreateDateTime).IsRequired();
            modelBuilder.Entity<IdentityRole>().ToTable("AppRole");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("AppUserClaim");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("AppUserLogin");
            modelBuilder.Entity<IdentityUserRole>().ToTable("AppUserRole");

        }
    }
}