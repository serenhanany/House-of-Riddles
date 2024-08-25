/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;

public class Question
{
    public int Id { get; set; }
    public string QuestionText { get; set; }
    public string[] AnswerOptions { get; set; }
    public string CorrectAnswer { get; set; }
    public string Hint { get; set; }
    public string Category { get; set; }
}

public class textControl : MonoBehaviour
{
    private MySqlConnection connection;

    // Constructor or method to set the existing connection
    public void SetConnection(MySqlConnection existingConnection)
    {
        connection = existingConnection;
    }

    public List<Question> GetQuestions()
    {
        List<Question> questions = new List<Question>();

        if (connection != null && connection.State == System.Data.ConnectionState.Open)
        {
            string query = "SELECT id, question_text, answer_option1, answer_option2, answer_option3, answer_option4, correct_answer, hint, category FROM questions";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Question question = new Question
                {
                    Id = reader.GetInt32("id"),
                    QuestionText = reader.GetString("question_text"),
                    AnswerOptions = new string[]
                    {
                        reader.GetString("answer_option1"),
                        reader.GetString("answer_option2"),
                        reader.GetString("answer_option3"),
                        reader.GetString("answer_option4")
                    },
                    CorrectAnswer = reader.GetString("correct_answer"),
                    Hint = reader.GetString("hint"),
                    Category = reader.GetString("category")
                };

                questions.Add(question);
            }

            reader.Close();
        }
        else
        {
            Debug.LogError("Database connection is not established.");
        }

        return questions;
    }
}*/


