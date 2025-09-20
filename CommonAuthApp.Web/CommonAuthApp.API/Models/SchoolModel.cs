using System.ComponentModel.DataAnnotations;

namespace CommonAuthApp.API.Models
{
    public class SchoolModel
    {
        [Key]
        public int SchoolId { get; set; }
        public string SchoolCode { get; set; }
        public string SchoolName { get; set; }
        public string SchoolEiin { get; set; }
        public string SchoolType { get; set; }
        public string SchoolDivision { get; set; }
        public string SchoolDistrict { get; set; }
        public string SchoolThana { get; set; }
        public string SchoolPostalCode { get; set; }
        public string SchoolArea { get; set; }
        public string SchoolAddress { get; set; }
        public string SchoolContactPerson { get; set; }
        public string SchoolContactPersonPosition { get; set; }
        public string SchoolConactPersonMobileNo { get; set; }
        public int SchoolStatus { get; set; }
        public bool IsActive { get; set; }
        public string SchoolLogoPath { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
