using System;
using System.Collections.Generic;

namespace Project2.Models;

public partial class GioHang
{
    public int IdGioHang { get; set; }

    public int IdtaiKhoan { get; set; }

    public int IdsanPham { get; set; }

    public int SoLuongTrongGio { get; set; }

    public virtual SanPham IdsanPhamNavigation { get; set; } = null!;

    public virtual TaiKhoan IdtaiKhoanNavigation { get; set; } = null!;
}
