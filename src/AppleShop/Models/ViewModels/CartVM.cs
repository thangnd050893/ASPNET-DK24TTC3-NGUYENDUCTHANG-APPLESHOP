using System;
using System.Collections.Generic;
using System.Linq;

namespace AppleShop.Models.ViewModels
{
    public class CartItemVM
    {
        public int ProductId { get; set; }
        public string Ten { get; set; } = "";
        public string? HinhAnh { get; set; }
        public decimal DonGia { get; set; }


        // Đơn giá của sản phẩm (map từ SanPham.GiaBan)
        public decimal GiaBan { get; set; }

        // Số lượng trong giỏ
        public int SoLuong { get; set; } = 1;

        // Thành tiền = Đơn giá * Số lượng
        public decimal ThanhTien => GiaBan * SoLuong;
    }

    public class CartVM
    {
        public List<CartItemVM> Items { get; set; } = new();

        public int TongSoLuong => Items.Sum(i => i.SoLuong);

        public decimal TongTien => Items.Sum(i => i.ThanhTien);
    }

}
