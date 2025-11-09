using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleShop.Models
{
    [Table("DanhMuc")] // map đúng tên bảng trong SQL
    public class DanhMuc
    {
        [Key]
        public int DanhMucId { get; set; }

        [Required, StringLength(100)]
        public string Ten { get; set; } = "";

        // FK tự tham chiếu (có trong DB của bạn)
        public int? DanhMucChaId { get; set; }

        // ---- Navigation properties cần cho OnModelCreating ----
        [ForeignKey(nameof(DanhMucChaId))]
        public DanhMuc? DanhMucCha { get; set; }           // danh mục cha

        public ICollection<DanhMuc> DanhMucCon { get; set; } = new List<DanhMuc>(); // các danh mục con

        // nếu SanPham có FK DanhMucId
        public ICollection<SanPham>? SanPhams { get; set; }
    }
}
