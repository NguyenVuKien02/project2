using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project2.Models;

namespace Project2.Controllers
{
    public class DanhGiaController : Controller
    {
        private readonly WebBanDoAnNhanhContext _context;

        public DanhGiaController(WebBanDoAnNhanhContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
        }

        // Kiểm tra đã mua sản phẩm chưa
        private async Task<bool> DaMuaSanPham(int userId, int sanPhamId)
        {
            return await _context.ChiTietDonHangs
                .Include(ct => ct.IddonHangNavigation)
                .AnyAsync(ct => ct.IdsanPham == sanPhamId &&
                               ct.IddonHangNavigation.IdtaiKhoan == userId &&
                               ct.IddonHangNavigation.Status == "Hoàn thành");
        }

        // POST: Thêm đánh giá
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemDanhGia(int sanPhamId, int rating, string noiDung)
        {
            if (!IsLoggedIn())
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập" });
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            // Kiểm tra đã mua sản phẩm chưa
            if (!await DaMuaSanPham(userId, sanPhamId))
            {
                return Json(new { success = false, message = "Bạn chỉ có thể đánh giá sản phẩm đã mua" });
            }

            // Kiểm tra đã đánh giá chưa
            var daDanhGia = await _context.DanhGias
                .AnyAsync(d => d.IdtaiKhoan == userId && d.IdsanPham == sanPhamId);

            if (daDanhGia)
            {
                return Json(new { success = false, message = "Bạn đã đánh giá sản phẩm này rồi" });
            }

            try
            {
                var danhGia = new DanhGia
                {
                    IdsanPham = sanPhamId,
                    IdtaiKhoan = userId,
                    Rating = rating,
                    NoiDungDanhGia = noiDung,
                    NgayDanhGia = DateTime.Now
                };

                _context.DanhGias.Add(danhGia);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đánh giá thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}