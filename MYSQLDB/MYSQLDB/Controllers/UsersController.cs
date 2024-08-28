using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYSQLDB;
using MYSQLDB.Controllers;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserContext _context;

    public UsersController(UserContext context)
    {
        _context = context;
    }

    // POST: api/Users/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserModel user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok(new { message = "User registered successfully!" });
    }

    // POST: api/Users/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        try
        {
            if (loginModel == null || string.IsNullOrEmpty(loginModel.Username) || string.IsNullOrEmpty(loginModel.Password))
            {
                return BadRequest("Invalid client request");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginModel.Username);

            if (user == null)
            {
                return Unauthorized(new { Message = "User does not exist" });
            }

            if (user.Password != loginModel.Password)
            {
                return Unauthorized(new { Message = "Invalid password" });
            }

            // Return the user data along with the success message
            return Ok(new
            {
                UserId = user.Id,
                Username = user.Username,
                Level = user.Level,
                Message = "Login successful"
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    [HttpGet("getQuestion")]
    public async Task<IActionResult> GetQuestion(int playerLevel)
    {
        // Get questions based on the player's level
        var questions = await _context.Questions
                                      .Where(q => q.DifficultyLevel == GetDifficultyLevel(playerLevel))
                                      .ToListAsync();

        if (questions == null || questions.Count == 0)
        {
            return NotFound(new { Message = "No questions found for this level" });
        }

        // Return the list of questions
        return Ok(questions);
    }


    private string GetDifficultyLevel(int playerLevel)
    {
        if (playerLevel == 1) return "easy";
        else if (playerLevel == 2) return "medium";
        else if (playerLevel == 3) return "hard";
        else return "easy";  // Default to easy if level is unrecognized
    }


    // POST: api/Users/recordAnswer
    [HttpPost("recordAnswer")]
    public async Task<IActionResult> RecordAnswer(int userId, int questionId, bool isCorrect)
    {
        try
        {
            var record = new PlayerAnsweredQuestionModel
            {
                UserId = userId,
                QuestionId = questionId
            };

            _context.PlayerAnsweredQuestions.Add(record);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Answer recorded successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }

    // GET: api/Users/getHint
    [HttpGet("getHint")]
    public async Task<IActionResult> GetHint(int questionId)
    {
        var question = await _context.Questions.FirstOrDefaultAsync(q => q.Id == questionId);

        if (question == null)
        {
            return NotFound(new { Message = "Question not found" });
        }

        return Ok(question.Hint);  // Return the hint for the question
    }

    // PUT: api/Users/updatePerformance
    [HttpPut("updatePerformance")]
    public async Task<IActionResult> UpdatePlayerPerformance(int userId, int questionId, bool answeredCorrectly, int attempts, bool hintGiven, float timeTaken)
    {
        try
        {
            var record = await _context.PlayerPerformance
                                       .FirstOrDefaultAsync(p => p.UserId == userId && p.QuestionId == questionId);

            if (record == null)
            {
                record = new PlayerPerformance
                {
                    UserId = userId,
                    QuestionId = questionId,
                    Attempts = attempts,
                    HintsGiven = hintGiven,
                    TimeTaken = timeTaken,
                    AttemptDate = DateTime.Now,
                    AnsweredCorrectly = answeredCorrectly
                };
                _context.PlayerPerformance.Add(record);
                //Console.WriteLine("Hello word"+record);

            }
            else
            {
                record.Attempts = attempts;
                record.HintsGiven = hintGiven;
                record.TimeTaken = timeTaken;
                record.AttemptDate = DateTime.Now;
                record.AnsweredCorrectly = answeredCorrectly;
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Player performance updated successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating player performance: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }

    }

    // PUT: api/Users/updateLevel
    [HttpPut("updateLevel")]
    public async Task<IActionResult> UpdateLevel(int userId, int newLevel)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return NotFound(new { Message = "User not found" });
        }

        user.Level = newLevel;
        await _context.SaveChangesAsync();

        return Ok(new { Message = "User level updated successfully" });
    }

 
}
