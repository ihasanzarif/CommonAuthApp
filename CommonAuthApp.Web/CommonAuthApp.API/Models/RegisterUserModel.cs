using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonAuthApp.API.Models
{
    public class RegisterUserModel
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
    }
}
