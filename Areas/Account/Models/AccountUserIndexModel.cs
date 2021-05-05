using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetIdentity.Areas.Account.Models
{
    public class AccountUserIndexModel
    {
        public IEnumerable<AccountUserIndexData> Datas { get; set; }
    }
}