namespace MYSQLDB.Controllers
{
    public class QuestionModel
    {
        public int Id { get; set; }  // Assuming there's an Id column as a primary key
        public string QuestionText { get; set; }
        public string DifficultyLevel { get; set; }  // Mapping to `difficulty_level`
        public string Category { get; set; }
        public string Hint { get; set; }
        public string CorrectAnswer { get; set; }  // Mapping to `correct_answer`
        public string AnswerOption1 { get; set; }  // Mapping to `answer_option1`
        public string AnswerOption2 { get; set; }  // Mapping to `answer_option2`
        public string AnswerOption3 { get; set; }  // Mapping to `answer_option3`
        public string AnswerOption4 { get; set; }  // Mapping to `answer_option4`
    }
}
