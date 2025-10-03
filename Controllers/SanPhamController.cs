using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project2.Models;


namespace Project2.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly WebBanDoAnNhanhContext _context;

        public SanPhamController(WebBanDoAnNhanhContext context)
        {
            _context = context;
        }

        // GET: Danh sách sản phẩm với phân trang
        public async Task<IActionResult> Index(int? page, int? danhMucId, string searchString)
        {
            int pageSize = 9;
            int pageNumber = page ?? 1;

            // Lưu giá trị tìm kiếm để hiển thị lại trong form
            ViewData["CurrentFilter"] = searchString;
            ViewBag.DanhMucId = danhMucId;

            var sanPhamsQuery = _context.SanPhams
                .Include(s => s.IddanhMucNavigation)
                .Where(s => s.Status == "Còn hàng");

            // Lọc theo danh mục 
            if (danhMucId.HasValue && danhMucId > 0)
            {
                sanPhamsQuery = sanPhamsQuery.Where(s => s.IddanhMuc == danhMucId);
            }

            // Tìm kiếm theo tên sản phẩm 
            if (!String.IsNullOrEmpty(searchString))
            {
                sanPhamsQuery = sanPhamsQuery.Where(s => s.TenSanPham.Contains(searchString));
            }

            var sanPhams = await sanPhamsQuery
                .OrderByDescending(s => s.NgayTaoSanPham)
                .ToListAsync();

            // Tạo PagedList thủ công
            var pagedList = sanPhams
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Tính toán thông tin phân trang
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)sanPhams.Count / pageSize);
            ViewBag.TotalItems = sanPhams.Count;
            ViewBag.PageSize = pageSize;

            // Danh sách danh mục cho filter
            ViewBag.DanhMucs = await _context.DanhMucs.ToListAsync();

            return View(pagedList);
        }

        // GET: Chi tiết sản phẩm
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sanPham = await _context.SanPhams
                .Include(s => s.IddanhMucNavigation)
                .Include(s => s.DanhGia)
                    .ThenInclude(d => d.IdtaiKhoanNavigation)
                .FirstOrDefaultAsync(s => s.IdsanPham == id);

            if (sanPham == null) return NotFound();

            // Kiểm tra xem user đã mua sản phẩm này chưa
            var userId = HttpContext.Session.GetString("UserId");
            ViewBag.DaMuaSanPham = false;

            if (!string.IsNullOrEmpty(userId))
            {
                var daMua = await _context.ChiTietDonHangs
                    .Include(ct => ct.IddonHangNavigation)
                    .AnyAsync(ct => ct.IdsanPham == id &&
                                   ct.IddonHangNavigation.IdtaiKhoan == int.Parse(userId) &&
                                   ct.IddonHangNavigation.Status == "Hoàn thành");
                ViewBag.DaMuaSanPham = daMua;
            }

            return View(sanPham);
        }

        // POST: Thêm vào giỏ hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemGioHang(int sanPhamId, int soLuong = 1)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            try
            {
                var gioHangExist = await _context.GioHangs
                    .FirstOrDefaultAsync(g => g.IdtaiKhoan == int.Parse(userId) && g.IdsanPham == sanPhamId);

                if (gioHangExist != null)
                {
                    gioHangExist.SoLuongTrongGio += soLuong;
                    _context.Update(gioHangExist);
                }
                else
                {
                    var gioHang = new GioHang
                    {
                        IdtaiKhoan = int.Parse(userId),
                        IdsanPham = sanPhamId,
                        SoLuongTrongGio = soLuong
                    };
                    _context.GioHangs.Add(gioHang);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
            }

            return RedirectToAction("Index", new { id = sanPhamId });
        }

       
    }
}