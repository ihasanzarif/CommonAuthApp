using System.ComponentModel.DataAnnotations;

namespace CommonAuthApp.API.Models
{
    public class SchoolMenu
    {
        [Key]
        public int MenuId { get; set; }
        public int MenuSerial { get; set; }
        public string MenuName { get; set; }
        public string MenuUrlPath { get; set; }
        public int MenuIsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }
    }
}
