using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MYSQLDB
{
    /*This table will help the RL agent to give a hint base on player performance*/
    [Table("player_performance")]
    public class PlayerPerformance
    {
        [Key]
        [Column("performance_id")]
        public int Id { get; set; }  

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
