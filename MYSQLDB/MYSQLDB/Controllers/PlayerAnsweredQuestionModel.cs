namespace MYSQLDB.Controllers
{
    public class PlayerAnsweredQuestionModel
    {
        public int Id { get; set; }  // Primary key

        public int UserId { get; set; }
        public UserModel User { get; set; }

        public int QuestionId { get; set; }
        public QuestionModel Question { get; set; }
    }
}
