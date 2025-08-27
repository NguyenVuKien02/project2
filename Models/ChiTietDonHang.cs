using System;
using System.Collections.Generic;

namespace Project2.Models;

public partial class ChiTietDonHang
{
    public int IdchiTietDonHang { get; set; }

    public int? IddonHang { get; set; }

    public int? IdsanPham { get; set; }

    public int SoLuong { get; set; }

    public decimal Gia { get; set; }

    public virtual DonHang? IddonHangNavigation { get; set; }

    public virtual SanPham? IdsanPhamNavigation { get; set; }
}
