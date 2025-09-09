using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project2.Models;

namespace Project2.Controllers
{
    public class DonHangController : Controller
    {
        private readonly WebBanDoAnNhanhContext _context;

        public DonHangController(WebBanDoAnNhanhContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
        }

        // GET: Danh sách đơn hàng của khách
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn())
                return RedirectToAction("DangNhap", "TaiKhoan");

            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var donHangs = await _context.DonHangs
                .Where(d => d.IdtaiKhoan == userId)
                .OrderByDescending(d => d.NgayTaoDonHang)
                .ToListAsync();

            return View(donHangs);
        }

        // GET: Chi tiết đơn hàng của khách
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("DangNhap", "TaiKhoan");

            if (id == null) return NotFound();

            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.IdsanPhamNavigation)
                .FirstOrDefaultAsync(d => d.IddonHang == id && d.IdtaiKhoan == userId);

            if (donHang == null) return NotFound();

            return View(donHang);
        }
    }
}