using System.ComponentModel.DataAnnotations;

namespace Project2.Models.ViewModel
{
    public class DangKiViewModel
    {
        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Hãy nhập họ và tên")]   //Bắt buộc nhập
        [StringLength(30, ErrorMessage = "Họ và tên không được vượt quá 30 ký tự")]  //Không vượt quá 
        public string FullName { get; set; }


        [Display(Name = "Email")]
        [Required(ErrorMessage = "Hãy nhập email")]   //Bắt buộc nhập
        [EmailAddress(ErrorMessage = "Email không hợp lệ, vui lòng nhập đúng định dạng")]  //email không hợp lệ 
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]  //Không vượt quá
        public string Email { get; set; }


        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Hãy nhập số điện thoại")]   //Bắt buộc nhập
        [StringLength(100, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]  //Không vượt quá
        [RegularExpression(@"^(0\d{9}|(\+84)\d{9})$", ErrorMessage = "Số điện thoại không hợp lệ, phải bắt đầu bằng 0 hoặc +84 và có 9 số sau đó")]
        // Bắt đầu bằng 0 hoặc +84 sau đó là 9 số theo sau 
        public string Phone { get; set; }


        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Hãy nhập mật khẩu")]   //Bắt buộc nhập
        [StringLength(100, ErrorMessage = "Mật khẩu không được vượt quá 100 ký tự")]  //Không vượt quá
        public string Password { get; set; }


        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Hãy nhập địa chỉ")]   //Bắt buộc nhập
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]  //Không vượt quá
        public string Address { get; set; }

    }
}
