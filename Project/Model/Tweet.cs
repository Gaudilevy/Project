using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Project.Model
{
	public class Tweet
	{
		public string SentBy { get; }
		public int TweetId { get; private set; }
		public string Content { get; }
		public DateTime SendTime { get; private set; }
		public bool IsComment { get; private set; }
		private List<Tweet> comments;

		public Tweet CommentedTo { get; private set; }
		public bool IsMainPost => (CommentedTo == null);

		[JsonConstructor]
		public Tweet(string sentBy,string content, DateTime sendTime)
		{
			SentBy = sentBy;
			Content = content;
			SendTime = sendTime == null ?  sendTime : DateTime.Now;
			comments = new List<Tweet>();
		}

		public Tweet(SqlDataReader dr)
		{
			TweetId = dr.GetInt32(0);
			Content = dr.GetString(1);
			SendTime = dr.GetDateTime(2);
			SentBy = dr.GetString(3);
		}
	}
}
