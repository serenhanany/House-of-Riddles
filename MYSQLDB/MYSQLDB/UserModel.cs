using System.ComponentModel.DataAnnotations;

namespace MYSQLDB
{
    /*Player Data*/
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Level { get; set; } = 1;  // Default level, if applicable
    }
}
