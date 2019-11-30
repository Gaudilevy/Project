using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Model;

namespace Project.Controllers
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : Controller
	{
		[HttpPost("login")]
		public ActionResult Login([FromBody] User user)
		{

			if (CheckUser(user, RegistrationType.Login))
			{
				return Ok(JwtManager.GenerateToken(user.Username));
			}

			return Unauthorized();
		}

		private bool CheckUser(User user, RegistrationType type)
		{
			string query = "SELECT password FROM users WHERE Username=@Username";
			bool usernameExists = false;
			bool passwordMatches = false;

			Database.PullFromDatabase(query,
				(cmd) => cmd.Parameters.AddWithValue("@Username", user.Username),
				(dr) =>
					{
						usernameExists = true;
						passwordMatches = (dr.GetString(0) == user.Password);
					}
				);

			return type == RegistrationType.Login ? passwordMatches : usernameExists;
		}

		[HttpPost("signup")]
		public ActionResult Signup([FromBody] User user)
		{

			if (CheckUser(user, RegistrationType.Signup))
			{
				return Conflict();
			}

			if (AddNewUser(user))
			{
				return Ok(JwtManager.GenerateToken(user.Username));
			}

			return BadRequest();
		}

		private bool AddNewUser(User user)
		{
			string query = "insert into Users (Username, Password, Email,IsAdmin) values (@Username,@Password,@Email,@IsAdmin)";

			int rowsAffected = Database.ExecuteCommand(query, (cmd) =>
			{
				cmd.Parameters.AddWithValueCheckNull("@Username", user.Username);
				cmd.Parameters.AddWithValueCheckNull("@Password", user.Password);
				cmd.Parameters.AddWithValueCheckNull("@Email", user.Email);
				cmd.Parameters.AddWithValueCheckNull("@IsAdmin", false);
			});

			return rowsAffected == 1;
		}

		[HttpGet("istokenactive")]
		public ActionResult IsTokenActive(string username)
		{
			return Ok(JwtManager.IsTokenActive(username));
		}

		[HttpGet("logout")]
		[Authorize]
		public ActionResult Logout()
		{
			JwtSecurityToken jwt= JwtManager.GetTokenFromRequest(Request);
			return Ok(JwtManager.InvalidateToken(jwt));
		}
	}
	public enum RegistrationType
	{
		Login,
		Signup
	}

}