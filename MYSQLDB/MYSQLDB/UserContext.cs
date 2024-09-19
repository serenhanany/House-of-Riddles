using Microsoft.EntityFrameworkCore;
using MYSQLDB;
using MYSQLDB.Controllers;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options) { }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<QuestionModel> Questions { get; set; }
    public DbSet<PlayerAnsweredQuestionModel> PlayerAnsweredQuestions { get; set; }
    public DbSet<HintModel> QuestionHints { get; set; }
    public DbSet<PlayerPerformance> PlayerPerformance { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the QuestionModel entity
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
        modelBuilder.Entity<QuestionModel>().Property(q => q.RecommendedLevel).HasColumnName("recommended_level");

        // Configure the HintModel entity
        modelBuilder.Entity<HintModel>().ToTable("question_hints");
        modelBuilder.Entity<HintModel>().HasKey(h => h.HintId);  // Set HintId as the primary key
        modelBuilder.Entity<HintModel>().Property(h => h.HintText).HasColumnName("hint_text");
        modelBuilder.Entity<HintModel>().Property(h => h.QuestionId).HasColumnName("question_id");
        modelBuilder.Entity<HintModel>().Property(h => h.HintLevel).HasColumnName("hint_level");
    }
}
