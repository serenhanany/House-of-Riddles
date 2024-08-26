using Microsoft.EntityFrameworkCore;
using MYSQLDB;
using MYSQLDB.Controllers;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options) { }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<QuestionModel> Questions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<QuestionModel>().ToTable("temp_questions");
        modelBuilder.Entity<QuestionModel>().HasKey(q => q.Id);
        modelBuilder.Entity<QuestionModel>().Property(q => q.QuestionText).HasColumnName("question_text");
        modelBuilder.Entity<QuestionModel>().Property(q => q.DifficultyLevel).HasColumnName("difficulty_level");
        modelBuilder.Entity<QuestionModel>().Property(q => q.Category).HasColumnName("category");
        modelBuilder.Entity<QuestionModel>().Property(q => q.Hint).HasColumnName("hint");
        modelBuilder.Entity<QuestionModel>().Property(q => q.CorrectAnswer).HasColumnName("correct_answer");
        modelBuilder.Entity<QuestionModel>().Property(q => q.AnswerOption1).HasColumnName("answer_option1");
        modelBuilder.Entity<QuestionModel>().Property(q => q.AnswerOption2).HasColumnName("answer_option2");
        modelBuilder.Entity<QuestionModel>().Property(q => q.AnswerOption3).HasColumnName("answer_option3");
        modelBuilder.Entity<QuestionModel>().Property(q => q.AnswerOption4).HasColumnName("answer_option4");
    }
}



