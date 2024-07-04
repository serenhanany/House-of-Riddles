using System.ComponentModel.DataAnnotations;

namespace MYSQLDB
{
    public class LoginModel
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
