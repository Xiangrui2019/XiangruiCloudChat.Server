using System.ComponentModel.DataAnnotations;

namespace XiangruiCloudChat.Server.Models.ApiAddressModels
{
    public class RegisterAddressModel
    {
        [Required]
        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "密码的长度在6-100之间.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "确认密码和密码不匹配!")]
        public string ConfirmPassword { get; set; }
    }
}
