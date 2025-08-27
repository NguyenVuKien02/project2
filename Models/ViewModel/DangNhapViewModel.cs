using System.ComponentModel.DataAnnotations;

namespace Project2.Models.ViewModel
{
    public class DangNhapViewModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Hãy nhập email")]   //Bắt buộc nhập
        [EmailAddress(ErrorMessage = "Email không hợp lệ, vui lòng nhập đúng định dạng")]  //email không hợp lệ 
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]  //Không vượt quá
        public string Email { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Hãy nhập mật khẩu")]   //Bắt buộc nhập
        [StringLength(100, ErrorMessage = "Mật khẩu không được vượt quá 100 ký tự")]  //Không vượt quá
        public string Password { get; set; }

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }
    }
}
