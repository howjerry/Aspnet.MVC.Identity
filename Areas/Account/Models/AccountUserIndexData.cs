using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetIdentity.Areas.Account.Models
{
    public class AccountUserIndexData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; } = true;
        public byte[] ProfilePicture { get; set; }
    }
}