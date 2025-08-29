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
        public async Task<IActionResult> Index()
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            var sanPhams = await _context.SanPhams
                .Include(s => s.IddanhMucNavigation)
                .ToListAsync();

            return View(sanPhams);
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

            return View(sanPham);
        }

        // GET: Tạo sản phẩm mới
        public IActionResult Create()
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            ViewBag.DanhMucs = _context.DanhMucs.ToList();
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
            if (sanPham != null)
            {
                _context.SanPhams.Remove(sanPham);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.IdsanPham == id);
        }
    }
}
