using System;
using System.Collections.Generic;

namespace Project2.Models;

public partial class SanPham
{
    public int IdsanPham { get; set; }

    public int? IddanhMuc { get; set; }

    public string TenSanPham { get; set; } = null!;

    public string? MoTaSanPham { get; set; }

    public decimal GiaSanPham { get; set; }

    public string? AnhSanPham { get; set; }

    public string? Status { get; set; }

    public DateTime? NgayTaoSanPham { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<DanhGia> DanhGia { get; set; } = new List<DanhGia>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual DanhMuc? IddanhMucNavigation { get; set; }
}
