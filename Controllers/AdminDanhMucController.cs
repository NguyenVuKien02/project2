using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project2.Models;

namespace Project2.Controllers
{
    public class AdminDanhMucController : Controller
    {
        private readonly WebBanDoAnNhanhContext _context;

        public AdminDanhMucController(WebBanDoAnNhanhContext context)
        {
            _context = context;
        }

        // Kiểm tra quyền admin
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

        // GET: Danh sách danh mục
        public async Task<IActionResult> Index(string searchString)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            ViewData["CurrentFilter"] = searchString;

            var danhMucs = _context.DanhMucs
                .Include(d => d.SanPhams)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                danhMucs = danhMucs.Where(d =>
                    d.TenDanhMuc.Contains(searchString) ||
                    d.MoTaDanhMuc.Contains(searchString));
            }


            return View(await danhMucs.OrderByDescending(d => d.NgayTaoDanhMuc).ToListAsync());
        }

        // GET: Chi tiết danh mục
        public async Task<IActionResult> Details(int? id)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            if (id == null) return NotFound();

            var danhMuc = await _context.DanhMucs
                .Include(d => d.SanPhams)
                .FirstOrDefaultAsync(m => m.IddanhMuc == id);

            if (danhMuc == null) return NotFound();

            return View(danhMuc);
        }

        // GET: Tạo danh mục mới
        public IActionResult Create()
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            return View();
        }

        // POST: Tạo danh mục mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DanhMuc danhMuc)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            if (ModelState.IsValid)
            {
                danhMuc.NgayTaoDanhMuc = DateTime.Now;

                _context.Add(danhMuc);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(danhMuc);
        }

        // GET: Sửa danh mục
        public async Task<IActionResult> Edit(int? id)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            if (id == null) return NotFound();

            var danhMuc = await _context.DanhMucs.FindAsync(id);
            if (danhMuc == null) return NotFound();

            return View(danhMuc);
        }

        // POST: Sửa danh mục
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DanhMuc danhMuc)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            if (id != danhMuc.IddanhMuc) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(danhMuc);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DanhMucExists(danhMuc.IddanhMuc))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(danhMuc);
        }

        // GET: Xóa danh mục
        public async Task<IActionResult> Delete(int? id)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            if (id == null) return NotFound();

            var danhMuc = await _context.DanhMucs
                .Include(d => d.SanPhams)
                .FirstOrDefaultAsync(m => m.IddanhMuc == id);

            if (danhMuc == null) return NotFound();

            return View(danhMuc);
        }

        // POST: Xóa danh mục
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var redirectResult = CheckAdminAccess();
            if (redirectResult != null) return redirectResult;

            var danhMuc = await _context.DanhMucs
                .Include(d => d.SanPhams)
                .FirstOrDefaultAsync(d => d.IddanhMuc == id);

            if (danhMuc != null)
            {
                // Kiểm tra xem danh mục có sản phẩm không
                if (danhMuc.SanPhams.Any())
                {
                    TempData["ErrorMessage"] = "Không thể xóa danh mục này vì có sản phẩm đang sử dụng!";
                    return RedirectToAction(nameof(Index));
                }

                _context.DanhMucs.Remove(danhMuc);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa danh mục thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DanhMucExists(int id)
        {
            return _context.DanhMucs.Any(e => e.IddanhMuc == id);
        }
    }
}