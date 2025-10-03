using Microsoft.AspNetCore.Mvc;
using Project2.Models;

namespace Project2.Controllers
{
    public class AdminController : Controller
    {
        private readonly WebBanDoAnNhanhContext _context;

        public AdminController(WebBanDoAnNhanhContext context)
        {
            _context = context;
        }

        // Kiểm tra quyền admin
        private bool IsAdmin()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            // Sửa: Kiểm tra cả "Admin" và "admin" (case insensitive)
            return string.Equals(userRole, "admin", StringComparison.OrdinalIgnoreCase);
        }

        // Redirect nếu không phải admin
        private IActionResult CheckAdminAccess()
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "TaiKhoan");
            return null;
        }

        // Trang chủ admin
        public IActionResult Index()
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            // Thống kê cơ bản
            ViewBag.TongSanPham = _context.SanPhams.Count();
            ViewBag.TongKhachHang = _context.TaiKhoans.Where(t => t.Role == "customer").Count();
            ViewBag.TongDonHang = _context.DonHangs.Count();
       
            return View();
        }
    }
}
