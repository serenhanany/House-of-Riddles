using Microsoft.EntityFrameworkCore;
using MYSQLDB;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options) { }

    public DbSet<UserModel> Users { get; set; }
}


