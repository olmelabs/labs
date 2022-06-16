using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using AuthApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<CognitoUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly CognitoUserManager<CognitoUser> userManager;
        private readonly CognitoUserPool pool;
        public AuthController(
            SignInManager<CognitoUser> signInManager,
            CognitoUserManager<CognitoUser> userManager,
            CognitoUserPool pool,
            IConfiguration configuration)
        {
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.userManager = userManager;
            this.pool = pool;
        }

        [HttpGet]
        [Route("login")]
        public async Task<IActionResult> Login([FromQuery] LoginModel model)
        {
            //var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, false, lockoutOnFailure: false);
            var user = await userManager.FindByEmailAsync(model.Username);
            
            var response = await userManager.CheckPasswordAsync(user, model.Password);

            if (user != null)
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Email, user.Attributes["email"]),
                    new(ClaimTypes.Name, user.Attributes["name"]),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                claims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

                //role claims duplicated because of json parsing
                //https://github.com/ThreeMammals/Ocelot/issues/679
                claims.AddRange(userRoles.Select(userRole => new Claim("Role", userRole)));

                claims.Add(new Claim("WebsiteId", "1216"));

                var token = GetToken(claims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            //var userExists = await userManager.FindByNameAsync(model.Username);
            //if (userExists != null)
            //    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            //IdentityUser user = new()
            //{
            //    Email = model.Email,
            //    SecurityStamp = Guid.NewGuid().ToString(),
            //    UserName = model.Username
            //};

            //var result = await userManager.CreateAsync(user, model.Password);

            //if (!result.Succeeded)
            //    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            //if (!await roleManager.RoleExistsAsync(UserRoles.User))
            //    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            //await userManager.AddToRoleAsync(user, UserRoles.User);

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }
        
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:ValidIssuer"],
                audience: configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(24),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
