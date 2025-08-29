using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project2.Models;
using Project2.Models.ViewModel;

namespace Project2.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly WebBanDoAnNhanhContext _context;

        public TaiKhoanController(WebBanDoAnNhanhContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult DangKi()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangKi(DangKiViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra email đã tồn tại
                    if (_context.TaiKhoans.Any(t => t.Email == model.Email))
                    {
                        ModelState.AddModelError("Email", "Email này đã được sử dụng");
                        return View(model);
                    }

                    // Tạo tài khoản mới
                    var taiKhoan = new TaiKhoan
                    {
                        FullName = model.FullName,
                        Email = model.Email,
                        Phone = model.Phone ?? "",
                        Password = model.Password,
                        Address = model.Address ?? "",
                        Role = "customer",
                        NgayDangKi = DateTime.Now
                    };

                    _context.TaiKhoans.Add(taiKhoan);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("DangNhap");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }
            return View(model);
        }

        // GET: /Account/Login
        public IActionResult DangNhap()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangNhap(DangNhapViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = _context.TaiKhoans
                    .FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

                    if (user == null)
                    {
                        ModelState.AddModelError("", "Sai email hoặc mật khẩu");
                        return View(model);
                    }

                    // Lưu thông tin vào Session
                    HttpContext.Session.SetString("UserId", user.IDTaiKhoan.ToString());
                    HttpContext.Session.SetString("UserName", user.FullName ?? "");
                    HttpContext.Session.SetString("UserRole", user.Role ?? "customer");
                    HttpContext.Session.SetString("UserEmail", user.Email ?? "");

                    // Debug: Log thông tin để kiểm tra
                    System.Diagnostics.Debug.WriteLine($"User Role: {user.Role}");
                    System.Diagnostics.Debug.WriteLine($"Session Role: {HttpContext.Session.GetString("UserRole")}");

                    // Nếu là Admin → về trang quản trị
                    if (string.Equals(user.Role, "admin", StringComparison.OrdinalIgnoreCase))
                        return RedirectToAction("Index", "Admin");
                    else
                        return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }
            return View(model);
        }

        // Đăng xuất
        public IActionResult DangXuat()
        {
            try
            {
                HttpContext.Session.Clear();
            }
            catch
            {
                // Ignore session errors
            }

            return RedirectToAction("DangNhap", "TaiKhoan");
        }
    }
}
