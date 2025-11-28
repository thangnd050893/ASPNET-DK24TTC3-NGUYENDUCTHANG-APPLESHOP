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

   
        public int? DanhMucChaId { get; set; }
        [ForeignKey(nameof(DanhMucChaId))]
        public DanhMuc? DanhMucCha { get; set; }         

        public ICollection<DanhMuc> DanhMucCon { get; set; } = new List<DanhMuc>(); 

  
        public ICollection<SanPham>? SanPhams { get; set; }
    }
}
