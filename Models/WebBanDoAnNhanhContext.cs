using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Project2.Models;

public partial class WebBanDoAnNhanhContext : DbContext
{
    public WebBanDoAnNhanhContext()
    {
    }

    public WebBanDoAnNhanhContext(DbContextOptions<WebBanDoAnNhanhContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<DanhGia> DanhGias{ get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-KNT8DC45;Database=Web_ban_do_an_nhanh;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.IdchiTietDonHang).HasName("PK__ChiTietD__EB5BBDC0A9188672");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.IdchiTietDonHang).HasColumnName("IDChiTietDonHang");
            entity.Property(e => e.Gia).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.IddonHang).HasColumnName("IDDonHang");
            entity.Property(e => e.IdsanPham).HasColumnName("IDSanPham");

            entity.HasOne(d => d.IddonHangNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.IddonHang)
                .HasConstraintName("FK__ChiTietDo__IDDon__48CFD27E");

            entity.HasOne(d => d.IdsanPhamNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.IdsanPham)
                .HasConstraintName("FK__ChiTietDo__IDSan__49C3F6B7");
        });

        modelBuilder.Entity<DanhGia>(entity =>
        {
            entity.HasKey(e => e.IddanhGia).HasName("PK__DanhGia__C216E48D0CEEC01F");

            entity.Property(e => e.IddanhGia).HasColumnName("IDDanhGia");
            entity.Property(e => e.IdsanPham).HasColumnName("IDSanPham");
            entity.Property(e => e.IdtaiKhoan).HasColumnName("IDTaiKhoan");
            entity.Property(e => e.NgayDanhGia)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NoiDungDanhGia).HasMaxLength(500);

            entity.HasOne(d => d.IdsanPhamNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.IdsanPham)
                .HasConstraintName("FK__DanhGia__IDSanPh__4E88ABD4");

            entity.HasOne(d => d.IdtaiKhoanNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.IdtaiKhoan)
                .HasConstraintName("FK__DanhGia__IDTaiKh__4F7CD00D");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.IddanhMuc).HasName("PK__DanhMuc__DF6C0BD2C7AC91B9");

            entity.ToTable("DanhMuc");

            entity.Property(e => e.IddanhMuc).HasColumnName("IDDanhMuc");
            entity.Property(e => e.MoTaDanhMuc).HasMaxLength(255);
            entity.Property(e => e.NgayTaoDanhMuc)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenDanhMuc).HasMaxLength(100);
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.IddonHang).HasName("PK__DonHang__9CA232F701DA209E");

            entity.ToTable("DonHang");

            entity.Property(e => e.IddonHang).HasColumnName("IDDonHang");
            entity.Property(e => e.DiaChiGiaoHang).HasMaxLength(255);
            entity.Property(e => e.IdtaiKhoan).HasColumnName("IDTaiKhoan");
            entity.Property(e => e.NgayTaoDonHang)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Chờ xác nhận");
            entity.Property(e => e.TongTien).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.IdtaiKhoanNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.IdtaiKhoan)
                .HasConstraintName("FK__DonHang__IDTaiKh__45F365D3");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.IdGioHang).HasName("PK__GioHang__CCE77A1F1A0E9A99");

            entity.ToTable("GioHang");

            entity.Property(e => e.IdsanPham).HasColumnName("IDSanPham");
            entity.Property(e => e.IdtaiKhoan).HasColumnName("IDTaiKhoan");
            entity.Property(e => e.SoLuongTrongGio).HasDefaultValue(1);

            entity.HasOne(d => d.IdsanPhamNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.IdsanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHang__IDSanPh__5441852A");

            entity.HasOne(d => d.IdtaiKhoanNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.IdtaiKhoan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHang__IDTaiKh__534D60F1");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.IdsanPham).HasName("PK__SanPham__9D45E58A2FEE6B56");

            entity.ToTable("SanPham");

            entity.Property(e => e.IdsanPham).HasColumnName("IDSanPham");
            entity.Property(e => e.AnhSanPham)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.GiaSanPham).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.IddanhMuc).HasColumnName("IDDanhMuc");
            entity.Property(e => e.MoTaSanPham).HasMaxLength(500);
            entity.Property(e => e.NgayTaoSanPham)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Còn hàng");
            entity.Property(e => e.TenSanPham).HasMaxLength(150);

            entity.HasOne(d => d.IddanhMucNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.IddanhMuc)
                .HasConstraintName("FK__SanPham__IDDanhM__412EB0B6");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.IDTaiKhoan).HasName("PK__TaiKhoan__BC5F907CDA8B115D");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.Email, "UQ__TaiKhoan__A9D10534B27F174F").IsUnique();

            entity.Property(e => e.IDTaiKhoan).HasColumnName("IDTaiKhoan");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.NgayDangKi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("customer");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
