using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MYSQLDB
{
    [Table("player_performance")]
    public class PlayerPerformance
    {
        [Key]
        [Column("performance_id")]
        public int Id { get; set; }  // Map to the 'performance_id' in the database

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("question_id")]
        public int QuestionId { get; set; }

        [Column("attempts")]
        public int Attempts { get; set; }

        [Column("hints_given")]
        public bool HintsGiven { get; set; }

        [Column("time_taken")]
        public float TimeTaken { get; set; }

        [Column("attempt_date")]
        public DateTime AttemptDate { get; set; }

        [Column("answered_correctly")]
        public bool AnsweredCorrectly { get; set; }
    }
}
