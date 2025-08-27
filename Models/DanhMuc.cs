using System;
using System.Collections.Generic;

namespace Project2.Models;

public partial class DanhMuc
{
    public int IddanhMuc { get; set; }

    public string TenDanhMuc { get; set; } = null!;

    public string? MoTaDanhMuc { get; set; }

    public DateTime? NgayTaoDanhMuc { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
