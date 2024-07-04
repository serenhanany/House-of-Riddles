using System.ComponentModel.DataAnnotations;

namespace MYSQLDB
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; } 
        public int Age { get; set; } 
        public string Study { get; set; } 
    }

}
