using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MYSQLDB.Controllers
{
    public class HintModel
    {
        [Key]
        [Column("hint_id")]
        public int HintId { get; set; }  // Primary key for the `question_hints` table

        [Column("question_id")]
        public int QuestionId { get; set; }  // Foreign key linking to `questions` table

        [Column("hint_text")]
        public string HintText { get; set; }  // The hint text

        [Column("hint_level")]
        public int HintLevel { get; set; }  // Level of detail or effectiveness of the hint

        [JsonIgnore]
        public QuestionModel Question { get; set; }
    }
}
