using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project2.Models;
using System.Diagnostics;

namespace Project2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WebBanDoAnNhanhContext _context;

        public HomeController(ILogger<HomeController> logger, WebBanDoAnNhanhContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Lấy 6 sản phẩm còn hàng để hiển thị trên trang chủ
                var sanPhamNoiBat = await _context.SanPhams
                    .Include(s => s.IddanhMucNavigation)
                    .Where(s => s.Status == "Còn hàng")
                    .OrderByDescending(s => s.NgayTaoSanPham)
                    .Take(6)
                    .ToListAsync();

                // ✅ Thêm phần tính số lượng đã bán (chỉ tính đơn "Hoàn thành")
                var daBanDict = await _context.ChiTietDonHangs
                    .Where(ct => ct.IddonHangNavigation.Status == "Hoàn thành")
                    .GroupBy(ct => ct.IdsanPham)
                    .Select(g => new { IdsanPham = g.Key, SoLuong = g.Sum(x => x.SoLuong) })
                    .ToDictionaryAsync(x => x.IdsanPham, x => x.SoLuong);

                ViewBag.DaBanDict = daBanDict;

                return View(sanPhamNoiBat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải trang chủ");
                // Trả về view với danh sách rỗng nếu có lỗi
                return View(new List<SanPham>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
