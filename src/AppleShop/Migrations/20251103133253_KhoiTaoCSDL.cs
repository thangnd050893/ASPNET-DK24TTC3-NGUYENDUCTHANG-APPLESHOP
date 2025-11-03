using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppleShop.Migrations
{
    /// <inheritdoc />
    public partial class KhoiTaoCSDL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Đơn hàng",
                columns: table => new
                {
                    MaDonHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Đơn hàng", x => x.MaDonHang);
                });

            migrationBuilder.CreateTable(
                name: "Sản phẩm",
                columns: table => new
                {
                    MaSanPham = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenSanPham = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    LoaiSanPham = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GiaBan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sản phẩm", x => x.MaSanPham);
                });

            migrationBuilder.CreateTable(
                name: "Chi tiết đơn hàng",
                columns: table => new
                {
                    MaChiTiet = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSanPham = table.Column<int>(type: "int", nullable: false),
                    SanPhamMaSanPham = table.Column<int>(type: "int", nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    GiaBan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaDonHang = table.Column<int>(type: "int", nullable: false),
                    DonHangMaDonHang = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chi tiết đơn hàng", x => x.MaChiTiet);
                    table.ForeignKey(
                        name: "FK_Chi tiết đơn hàng_Sản phẩm_SanPhamMaSanPham",
                        column: x => x.SanPhamMaSanPham,
                        principalTable: "Sản phẩm",
                        principalColumn: "MaSanPham");
                    table.ForeignKey(
                        name: "FK_Chi tiết đơn hàng_Đơn hàng_DonHangMaDonHang",
                        column: x => x.DonHangMaDonHang,
                        principalTable: "Đơn hàng",
                        principalColumn: "MaDonHang");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chi tiết đơn hàng_DonHangMaDonHang",
                table: "Chi tiết đơn hàng",
                column: "DonHangMaDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_Chi tiết đơn hàng_SanPhamMaSanPham",
                table: "Chi tiết đơn hàng",
                column: "SanPhamMaSanPham");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chi tiết đơn hàng");

            migrationBuilder.DropTable(
                name: "Sản phẩm");

            migrationBuilder.DropTable(
                name: "Đơn hàng");
        }
    }
}
