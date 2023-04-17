using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase {
    private readonly MyContext context;

    public AuthController(MyContext context) {
        this.context = context;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> Login(LoginModel model) {
        model.Email = model.Email.Trim().ToLower();

        var account = await context.Accounts
            .SingleOrDefaultAsync(x =>
                x.Email.Equals(model.Email) &&
                x.Pwd.Equals(model.Pwd.Hash())
            );
        if(account != null) {

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Email, account.Email),
             };

            var key = Encoding.ASCII.GetBytes("the-secret-key-that-i-chose-to-get-rid-of-u");
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(5),
                Issuer = "liron",
                //Audience = "webapp",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            var stringToken = tokenHandler.WriteToken(token);

            return stringToken;
        }

        return BadRequest();
    }
}

public class LoginModel {
    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, PasswordPropertyText]
    public string Pwd { get; set; }
}
