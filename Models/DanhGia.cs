using System;
using System.Collections.Generic;

namespace Project2.Models;

public partial class DanhGia
{
    public int IddanhGia { get; set; }

    public int? IdsanPham { get; set; }

    public int? IdtaiKhoan { get; set; }

    public int? Rating { get; set; }

    public string? NoiDungDanhGia { get; set; }

    public DateTime? NgayDanhGia { get; set; }

    public virtual SanPham? IdsanPhamNavigation { get; set; }

    public virtual TaiKhoan? IdtaiKhoanNavigation { get; set; }
}
