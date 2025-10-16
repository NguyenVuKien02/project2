using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project2.Models;

namespace Project2.Controllers
{
    public class AdminTaiKhoanController : Controller
    {
        private readonly WebBanDoAnNhanhContext _context;

        public AdminTaiKhoanController(WebBanDoAnNhanhContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return string.Equals(userRole, "admin", StringComparison.OrdinalIgnoreCase);
        }

        private IActionResult CheckAdminAccess()
        {
            if (!IsAdmin())
                return RedirectToAction("DangNhap", "TaiKhoan");
            return null;
        }

        // GET: Danh sách tài khoản
        public async Task<IActionResult> Index()
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            var taiKhoans = await _context.TaiKhoans
                .OrderByDescending(t => t.NgayDangKi)
                .ToListAsync();

            return View(taiKhoans);
        }

        // GET: Chi tiết tài khoản
        public async Task<IActionResult> Details(int? id)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            if (id == null) return NotFound();

            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.DonHangs)
                .FirstOrDefaultAsync(t => t.IDTaiKhoan == id);

            if (taiKhoan == null) return NotFound();

            return View(taiKhoan);
        }

        // POST: Cập nhật trạng thái tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(int id, string role)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan != null)
            {
                taiKhoan.Role = role;
                _context.Update(taiKhoan);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật quyền thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ GET: Hiển thị xác nhận xóa
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            if (id == null) return NotFound();

            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.DonHangs)
                .FirstOrDefaultAsync(t => t.IDTaiKhoan == id);

            if (taiKhoan == null) return NotFound();

            return View(taiKhoan);
        }

        // ✅ POST: Xóa thật sự (sau khi xác nhận)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.DonHangs)
                .FirstOrDefaultAsync(t => t.IDTaiKhoan == id);

            if (taiKhoan == null)
            {
                TempData["ErrorMessage"] = "Tài khoản không tồn tại!";
                return RedirectToAction(nameof(Index));
            }

            if (taiKhoan.DonHangs.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa tài khoản vì đã có đơn hàng!";
                return RedirectToAction(nameof(Index));
            }

            if (taiKhoan.Role?.ToLower() == "admin")
            {
                TempData["ErrorMessage"] = "Không thể xóa tài khoản admin!";
                return RedirectToAction(nameof(Index));
            }

            _context.TaiKhoans.Remove(taiKhoan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa tài khoản thành công!";
            return RedirectToAction(nameof(Index));
        }


    }
}