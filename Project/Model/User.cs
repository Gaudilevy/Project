using System.Collections.Generic;

namespace Project.Model
{
	public class User
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }

		public bool IsAdmin { get;  set; }

		private List<Tweet> posts = new List<Tweet>();


		public User(string username, string password, string email)
		{
			Username = username;
			Password = password;
			Email = email;
		}
	}
}
