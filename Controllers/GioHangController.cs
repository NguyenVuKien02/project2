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
        public async Task<IActionResult> ThanhToan(int? sanPhamId, int? soLuong)
        {
            if (!IsLoggedIn())
                return RedirectToAction("DangNhap", "TaiKhoan");

            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            // ✅ Trường hợp MUA NGAY
            if (sanPhamId.HasValue && soLuong.HasValue)
            {
                var sanPham = await _context.SanPhams.FindAsync(sanPhamId.Value);
                if (sanPham == null)
                {
                    TempData["ErrorMessage"] = "Sản phẩm không tồn tại!";
                    return RedirectToAction("Index", "SanPham");
                }

                ViewBag.MuaNgay = true;
                ViewBag.SanPham = sanPham;
                ViewBag.SoLuong = soLuong.Value;
                ViewBag.TongTien = sanPham.GiaSanPham * soLuong.Value;

                return View();
            }

            // ✅ Trường hợp thanh toán từ GIỎ HÀNG
            var gioHangs = await _context.GioHangs
                .Include(g => g.IdsanPhamNavigation)
                .Where(g => g.IdtaiKhoan == userId)
                .ToListAsync();

            if (!gioHangs.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            ViewBag.MuaNgay = false;
            ViewBag.TongTien = gioHangs.Sum(g => g.IdsanPhamNavigation.GiaSanPham * g.SoLuongTrongGio);
            ViewBag.GioHangs = gioHangs;

            return View();
        }

        // POST: Xác nhận đặt hàng
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> XacNhanDatHang(string diaChiGiaoHang, int? sanPhamId, int? soLuong)
        {
            if (!IsLoggedIn())
                return RedirectToAction("DangNhap", "TaiKhoan");

            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            try
            {
                // ✅ Nếu là MUA NGAY
                if (sanPhamId.HasValue && soLuong.HasValue)
                {
                    var sanPham = await _context.SanPhams.FindAsync(sanPhamId.Value);
                    if (sanPham == null)
                    {
                        TempData["ErrorMessage"] = "Sản phẩm không tồn tại!";
                        return RedirectToAction("Index", "SanPham");
                    }

                    // --- Tạo đơn hàng ---
                    var donHang = new DonHang
                    {
                        IdtaiKhoan = userId,
                        NgayTaoDonHang = DateTime.Now,
                        TongTien = sanPham.GiaSanPham * soLuong.Value,
                        Status = "Chờ xác nhận",
                        DiaChiGiaoHang = diaChiGiaoHang
                    };

                    _context.DonHangs.Add(donHang);
                    await _context.SaveChangesAsync();

                    // --- Chi tiết đơn hàng ---
                    var chiTiet = new ChiTietDonHang
                    {
                        IddonHang = donHang.IddonHang,
                        IdsanPham = sanPham.IdsanPham,
                        SoLuong = soLuong.Value,
                        Gia = sanPham.GiaSanPham
                    };
                    _context.ChiTietDonHangs.Add(chiTiet);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đặt hàng thành công!";
                    return RedirectToAction("Index", "DonHang");
                }

                // ✅ Nếu là thanh toán từ GIỎ HÀNG
                var gioHangs = await _context.GioHangs
                    .Include(g => g.IdsanPhamNavigation)
                    .Where(g => g.IdtaiKhoan == userId)
                    .ToListAsync();

                if (!gioHangs.Any())
                {
                    TempData["ErrorMessage"] = "Giỏ hàng trống!";
                    return RedirectToAction("Index");
                }

                var donHangGH = new DonHang
                {
                    IdtaiKhoan = userId,
                    NgayTaoDonHang = DateTime.Now,
                    TongTien = gioHangs.Sum(g => g.IdsanPhamNavigation.GiaSanPham * g.SoLuongTrongGio),
                    Status = "Chờ xác nhận",
                    DiaChiGiaoHang = diaChiGiaoHang
                };

                _context.DonHangs.Add(donHangGH);
                await _context.SaveChangesAsync();

                foreach (var item in gioHangs)
                {
                    var chiTiet = new ChiTietDonHang
                    {
                        IddonHang = donHangGH.IddonHang,
                        IdsanPham = item.IdsanPham,
                        SoLuong = item.SoLuongTrongGio,
                        Gia = item.IdsanPhamNavigation.GiaSanPham
                    };
                    _context.ChiTietDonHangs.Add(chiTiet);
                }

                _context.GioHangs.RemoveRange(gioHangs);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đặt hàng thành công!";
                return RedirectToAction("Index", "DonHang");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
                return RedirectToAction("ThanhToan");
            }
        }

    }
}