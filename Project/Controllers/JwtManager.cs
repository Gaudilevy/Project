using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace Project.Controllers
{
	public static class JwtManager
	{
		private static Dictionary<string, JwtSecurityToken> activeTokens = new Dictionary<string, JwtSecurityToken>();
		private static Timer tokenCheckingTimer;
		public static SymmetricSecurityKey tokenSecurityKey;

		static JwtManager()
		{
			tokenCheckingTimer = new Timer(obj => CheckExpiredTokens(), null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
		}

		public static void CheckExpiredTokens()
		{
			List<string> expired = new List<string>();
			foreach (KeyValuePair<string, JwtSecurityToken> pair in activeTokens)
			{
				if (pair.Value.ValidTo < DateTime.Now)
				{
					expired.Add(pair.Key);
				}
			}

			foreach (string username in expired)
			{
				activeTokens.Remove(username);
			}
		}


		public static string GenerateToken(string username)
		{
			// Signing credentials using security key
			SigningCredentials signingCredentials = new SigningCredentials(tokenSecurityKey, SecurityAlgorithms.HmacSha256);

			// Add claims
			List<Claim> claims = new List<Claim> {
				new Claim("sub",username)
			};

			// Create token
			JwtSecurityToken token = new JwtSecurityToken(
				issuer: "Audilevy",
				expires: DateTime.Now.AddHours(1),
				claims: claims,
				signingCredentials: signingCredentials
				);


			if (activeTokens.ContainsKey(username))
			{

				activeTokens[username] = token;
			}
			else
			{
				activeTokens.Add(username, token);
			}

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public static bool IsTokenActive(string username)
		{
			return activeTokens.ContainsKey(username);
		}

		public static bool IsTokenActive(HttpRequest request)
		{
			string username = GetTokenFromRequest(request).Subject;
			return IsTokenActive(username);
		}

		public static bool InvalidateToken(JwtSecurityToken jwt)
		{
			string username = jwt.Subject;
			if (IsTokenActive(username))
			{
				return activeTokens.Remove(username);
			}
			return true;
		}

		public static JwtSecurityToken GetTokenFromRequest(HttpRequest request)
		{
			// TODO - this line probably breaks if there is no Authorization header 
			string token = request.Headers["Authorization"][0].Split(" ")[1];
			return new JwtSecurityToken(token);
		}
	}
}
