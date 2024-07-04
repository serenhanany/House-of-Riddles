using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MYSQLDB;
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

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginModel.Username);

           if (user == null)
            {
                return Unauthorized(new { Message = "User does not exist" });
            }

            if (user.Password != loginModel.Password)
            {
                return Unauthorized(new { Message = "Invalid password" });
            }

            return Ok(new { Message = "Login successful" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }
}
