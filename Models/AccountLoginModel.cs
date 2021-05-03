using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace AspNetIdentity.Models
{
    public class AccountLoginModel
    {
        [Required(ErrorMessage = "請輸入帳號")]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "未填寫密碼")]
        public string Password { get; set; }
        public bool Remember { get; set; }
    }
}