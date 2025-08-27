using System;
using System.Collections.Generic;

namespace Project2.Models;

public partial class DonHang
{
    public int IddonHang { get; set; }

    public int? IdtaiKhoan { get; set; }

    public DateTime? NgayTaoDonHang { get; set; }

    public decimal? TongTien { get; set; }

    public string? Status { get; set; }

    public string? DiaChiGiaoHang { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual TaiKhoan? IdtaiKhoanNavigation { get; set; }
}
