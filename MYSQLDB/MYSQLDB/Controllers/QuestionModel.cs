using MYSQLDB.Controllers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

public class QuestionModel
{
    public int Id { get; set; } // Primary key
    public string QuestionText { get; set; }
    public string DifficultyLevel { get; set; }
    public string Category { get; set; }
    public string Hint { get; set; }
    public string CorrectAnswer { get; set; }
    public string AnswerOption1 { get; set; }
    public string AnswerOption2 { get; set; }
    public string AnswerOption3 { get; set; }
    public string AnswerOption4 { get; set; }

    [Column("recommended_level")]
    public int RecommendedLevel { get; set; }
    public List<HintModel> Hints { get; set; }  // Add this line
}
