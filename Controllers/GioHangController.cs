using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project2.Models;

namespace Project2.Controllers
{
    public class GioHangController : Controller
    {
        private readonly WebBanDoAnNhanhContext _context;

        public GioHangController(WebBanDoAnNhanhContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
        }

        // GET: Xem giỏ hàng
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn())
                return RedirectToAction("DangNhap", "TaiKhoan");

            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var gioHangs = await _context.GioHangs
                .Include(g => g.IdsanPhamNavigation)
                .Where(g => g.IdtaiKhoan == userId)
                .ToListAsync();

            ViewBag.TongTien = gioHangs.Sum(g => g.IdsanPhamNavigation.GiaSanPham * g.SoLuongTrongGio);

            return View(gioHangs);
        }

        // POST: Cập nhật số lượng
        [HttpPost]
        public async Task<IActionResult> CapNhatSoLuong(int id, int soLuong)
        {
            if (!IsLoggedIn())
                return Json(new { success = false, message = "Vui lòng đăng nhập" });

            var gioHang = await _context.GioHangs.FindAsync(id);
            if (gioHang != null && soLuong > 0)
            {
                gioHang.SoLuongTrongGio = soLuong;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Không thể cập nhật" });
        }

        // POST: Xóa khỏi giỏ hàng
        [HttpPost]
        public async Task<IActionResult> XoaKhoiGio(int id)
        {
            if (!IsLoggedIn())
                return Json(new { success = false });

            var gioHang = await _context.GioHangs.FindAsync(id);
            if (gioHang != null)
            {
                _context.GioHangs.Remove(gioHang);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        // GET: Thanh toán
        public async Task<IActionResult> ThanhToan()
        {
            if (!IsLoggedIn())
                return RedirectToAction("DangNhap", "TaiKhoan");

            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var gioHangs = await _context.GioHangs
                .Include(g => g.IdsanPhamNavigation)
                .Where(g => g.IdtaiKhoan == userId)
                .ToListAsync();

            if (!gioHangs.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            ViewBag.TongTien = gioHangs.Sum(g => g.IdsanPhamNavigation.GiaSanPham * g.SoLuongTrongGio);
            ViewBag.GioHangs = gioHangs;

            return View();
        }

        // POST: Xác nhận đặt hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanDatHang(string diaChiGiaoHang)
        {
            if (!IsLoggedIn())
                return RedirectToAction("DangNhap", "TaiKhoan");

            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var gioHangs = await _context.GioHangs
                .Include(g => g.IdsanPhamNavigation)
                .Where(g => g.IdtaiKhoan == userId)
                .ToListAsync();

            if (!gioHangs.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            try
            {
                // Tạo đơn hàng
                var donHang = new DonHang
                {
                    IdtaiKhoan = userId,
                    NgayTaoDonHang = DateTime.Now,
                    TongTien = gioHangs.Sum(g => g.IdsanPhamNavigation.GiaSanPham * g.SoLuongTrongGio),
                    Status = "Chờ xác nhận",
                    DiaChiGiaoHang = diaChiGiaoHang
                };

                _context.DonHangs.Add(donHang);
                await _context.SaveChangesAsync();

                // Tạo chi tiết đơn hàng
                foreach (var item in gioHangs)
                {
                    var chiTiet = new ChiTietDonHang
                    {
                        IddonHang = donHang.IddonHang,
                        IdsanPham = item.IdsanPham,
                        SoLuong = item.SoLuongTrongGio,
                        Gia = item.IdsanPhamNavigation.GiaSanPham
                    };
                    _context.ChiTietDonHangs.Add(chiTiet);
                }

                // Xóa giỏ hàng
                _context.GioHangs.RemoveRange(gioHangs);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đặt hàng thành công! Đơn hàng đang chờ xác nhận.";
                return RedirectToAction("Index", "DonHang");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("ThanhToan");
            }
        }
    }
}