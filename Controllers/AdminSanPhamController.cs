using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project2.Models;

namespace Project2.Controllers
{
    public class AdminSanPhamController : Controller
    {
        private readonly WebBanDoAnNhanhContext _context;

        public AdminSanPhamController(WebBanDoAnNhanhContext context)
        {
            _context = context;
        }

        // Kiểm tra quyền admin
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

        // GET: Danh sách sản phẩm
        public async Task<IActionResult> Index(string searchString, int? categoryId)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = categoryId;

            var sanPhams = _context.SanPhams
                .Include(s => s.IddanhMucNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                sanPhams = sanPhams.Where(s =>
                    s.TenSanPham.Contains(searchString) ||
                    s.MoTaSanPham.Contains(searchString) ||
                    s.IddanhMucNavigation.TenDanhMuc.Contains(searchString));
            }

            if (categoryId.HasValue && categoryId > 0)
            {
                sanPhams = sanPhams.Where(s => s.IddanhMuc == categoryId);
            }

            // Lấy danh mục để đổ vào dropdown
            ViewBag.DanhMucs = await _context.DanhMucs.ToListAsync();



            return View(await sanPhams.ToListAsync());
        }


        // GET: Chi tiết sản phẩm
        public async Task<IActionResult> Details(int? id)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            if (id == null) return NotFound();

            var sanPham = await _context.SanPhams
                .Include(s => s.IddanhMucNavigation)
                .FirstOrDefaultAsync(m => m.IdsanPham == id);

            if (sanPham == null) return NotFound();

            // Chỉ tính số lượng đã bán trong các đơn "Hoàn thành"
            var soLuongDaBan = await _context.ChiTietDonHangs
                .Include(c => c.IddonHangNavigation)
                .Where(c => c.IdsanPham == id && c.IddonHangNavigation.Status == "Hoàn thành")
                .SumAsync(c => (int?)c.SoLuong) ?? 0;

            ViewBag.SoLuongDaBan = soLuongDaBan;

            return View(sanPham);
        }



        // GET: Tạo sản phẩm mới
        public IActionResult Create()
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            // Lấy danh mục
            ViewBag.DanhMucs = _context.DanhMucs.ToList();

            // Lấy danh sách ảnh trong wwwroot/images/
            var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (Directory.Exists(imageFolder))
            {
                var imageFiles = Directory.GetFiles(imageFolder)
                                          .Select(Path.GetFileName)
                                          .ToList();
                ViewBag.ImageFiles = imageFiles;
            }
            else
            {
                ViewBag.ImageFiles = new List<string>(); // nếu chưa có thư mục images
            }

            return View();
        }

        // POST: Tạo sản phẩm mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPham sanPham)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            if (ModelState.IsValid)
            {
                sanPham.NgayTaoSanPham = DateTime.Now;
                sanPham.Status = "Còn hàng";

                _context.Add(sanPham);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            return View(sanPham);
        }

        // GET: Sửa sản phẩm
        public async Task<IActionResult> Edit(int? id)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            if (id == null) return NotFound();

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null) return NotFound();

            ViewBag.DanhMucs = _context.DanhMucs.ToList();

            // 👉 Lấy danh sách ảnh có trong thư mục /images/
            var imageDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            var imageFiles = Directory.Exists(imageDir)
                ? Directory.GetFiles(imageDir).Select(Path.GetFileName).ToList()
                : new List<string>();
            ViewBag.ImageFiles = imageFiles;

            return View(sanPham);
        }


        // POST: Sửa sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SanPham sanPham)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            if (id != sanPham.IdsanPham) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.IdsanPham))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            return View(sanPham);
        }

        // GET: Xóa sản phẩm
        public async Task<IActionResult> Delete(int? id)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            if (id == null) return NotFound();

            var sanPham = await _context.SanPhams
                .Include(s => s.IddanhMucNavigation)
                .FirstOrDefaultAsync(m => m.IdsanPham == id);

            if (sanPham == null) return NotFound();

            return View(sanPham);
        }

        // POST: Xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại!";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra xem sản phẩm có trong đơn hàng không
            bool coTrongDonHang = await _context.ChiTietDonHangs
                .AnyAsync(c => c.IdsanPham == id);

            if (coTrongDonHang)
            {
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm này vì đã có trong đơn hàng!";
                return RedirectToAction(nameof(Index));
            }

            _context.SanPhams.Remove(sanPham);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }



        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.IdsanPham == id);
        }
    }
}
