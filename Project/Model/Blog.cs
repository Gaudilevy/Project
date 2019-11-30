using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Model
{
	public class Blog
	{

		public string Title { get; set; }
		public string Username { get; private set; }
		private List<Tweet> posts = new List<Tweet>();

		public void AddPost(Tweet post)
		{
			if (post == null || post.Content == null)
			{
				return;
			}
			posts.Add(post);
		}

		public void EditPost(int postID, string content)
		{
			foreach (Tweet p in posts)
			{
				if (p.TweetId == postID)
				{
					//p.Content = content;
				}
			}
		}

		public void DeletePost(int postID)
		{
			for (int i = 0; i < posts.Count; i++)
			{
				if (posts[i].TweetId == postID)
				{
					posts.RemoveAt(i);
					break;
				}
			}
		}

		public Tweet GetPost(int postID)
		{
			foreach (Tweet p in posts)
			{
				if (p.TweetId == postID)
				{
					return p;
				}
			}
				return null;
		}
	}
}