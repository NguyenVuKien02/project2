using Microsoft.AspNetCore.Mvc;
using Project2.Models;
using Microsoft.EntityFrameworkCore;

namespace Project2.Controllers
{
    public class AdminDonHangController : Controller
    {
        private readonly WebBanDoAnNhanhContext _context;

        public AdminDonHangController(WebBanDoAnNhanhContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            // Sửa: Kiểm tra case insensitive
            return string.Equals(userRole, "admin", StringComparison.OrdinalIgnoreCase);
        }

        private IActionResult CheckAdminAccess()
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "TaiKhoan");
            return null;
        }

        // GET: Danh sách đơn hàng
        public async Task<IActionResult> Index()
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            var donHangs = await _context.DonHangs
                .Include(d => d.IdtaiKhoanNavigation)
                .OrderByDescending(d => d.NgayTaoDonHang)
                .ToListAsync();

            return View(donHangs);
        }

        // GET: Chi tiết đơn hàng
        public async Task<IActionResult> Details(int? id)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            if (id == null) return NotFound();

            var donHang = await _context.DonHangs
                .Include(d => d.IdtaiKhoanNavigation)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.IdsanPhamNavigation)
                .FirstOrDefaultAsync(m => m.IddonHang == id);

            if (donHang == null) return NotFound();

            return View(donHang);
        }

        // POST: Cập nhật trạng thái đơn hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang != null)
            {
                donHang.Status = status;
                _context.Update(donHang);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }
    }
}
