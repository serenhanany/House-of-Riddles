using Microsoft.EntityFrameworkCore;
using MYSQLDB;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options) { }

    public DbSet<UserModel> Users { get; set; }

   /* protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>().ToTable("Users");
        modelBuilder.Entity<UserModel>().Property(u => u.Fullname).IsRequired().HasMaxLength(255);
        modelBuilder.Entity<UserModel>().Property(u => u.Age).IsRequired();
        modelBuilder.Entity<UserModel>().Property(u => u.Study).IsRequired().HasMaxLength(255);
    }*/
}



