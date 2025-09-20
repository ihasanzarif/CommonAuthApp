using System.ComponentModel.DataAnnotations;

namespace CommonAuthApp.API.Models
{
    public class SchoolSystemDetails
    {
        [Key]
        public int SystemDetailsId { get; set; }
        public int SchoolId { get; set; }
        public string SchoolServerIP { get; set; }
        public int SchoolServerPort { get; set; }
        public string SchoolDBName { get; set; }
        public string SchoolDBServer { get; set; }
        public int SchoolDBPort { get; set; }
        public string SchoolDBUsername { get; set; }
        public string SchoolDBPass { get; set; }
        public string SchoolDBSystemName { get; set; }
        public string SchoolDBConnectionString { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
