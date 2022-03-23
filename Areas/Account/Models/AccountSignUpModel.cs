using System.ComponentModel.DataAnnotations;

namespace AspNetIdentity.Areas.Account.Models
{
    public class AccountSignUpModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "請填寫電子郵件")]
        public string Email { get; set; }

        [Required(ErrorMessage = "請填寫帳號")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "請填寫名稱")]
        public string Name { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "未填寫密碼")]
        [StringLength(100, ErrorMessage = "密碼長度最少六個字", MinimumLength = 6)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "密碼輸入不相同")]
        public string ConfirmPassword { get; set; }

    }
}