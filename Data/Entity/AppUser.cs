using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetIdentity.Data.Entity
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}