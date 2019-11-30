using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Model;

namespace Project.Controllers
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class TweetsController : ControllerBase
	{
		[HttpPost("post")]
		public ActionResult PostTweet([FromBody] Tweet tweet)
		{
			if (tweet == null || string.IsNullOrWhiteSpace(tweet.Content))
			{
				return BadRequest();
			}

			if (!JwtManager.IsTokenActive(Request))
			{
				return Unauthorized();
			}

			int tweetId = AddTweet(tweet);
			if (tweetId > 0)
			{
				return Ok(tweetId);
			}

			return BadRequest();
		}


		[HttpGet("get")]
		public ActionResult GetTweets()
		{
			if (!JwtManager.IsTokenActive(Request))
			{
				return Unauthorized();
			}
			string query = "select * from Tweets order by SendTime desc";
			List<Tweet> tweets = new List<Tweet>();

			Database.PullFromDatabase(
				query: query,
				configureCommand: null,
				doWithEachRow: dr => tweets.Add(new Tweet(dr)));
			return Ok(tweets);

			#region nonsense

			//"SELECT * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY SendTime) AS RowNum FROM Tweets) AS MyDerivedTable WHERE MyDerivedTable.RowNum BETWEEN @From AND @To";
			//"select top 5 * from Tweets order by SendTime desc";

			#endregion
		}

		[HttpPost("edit")]
		public ActionResult EditTweet([FromBody] Tweet tweet)
		{
			if (tweet == null || string.IsNullOrWhiteSpace(tweet.Content) || tweet.TweetId < 0)
			{
				return BadRequest();
			}

			
			if (!JwtManager.IsTokenActive(Request))
			{
				return Unauthorized();
			}

			string query = "update Tweets set Content = @Content where TweetId = @TweetId";

			int rowsEffected = Database.ExecuteCommand(query,
				configureCommand: cmd =>
					{
						cmd.Parameters.AddWithValueCheckNull("@Content", tweet.Content);
						cmd.Parameters.AddWithValueCheckNull("@TweetId", tweet.TweetId);

					});

			if (rowsEffected > 0)
			{
				return Ok(rowsEffected);
			}

			return BadRequest();
		}

		[HttpGet("delete")]
		public ActionResult DeleteTweet(string username, int tweetId)
		{
			if (tweetId <= 0)
			{
				return BadRequest();
			}

			if (!JwtManager.IsTokenActive(Request))
			{
				return Unauthorized();
			}

			string query = "delete from Tweets where TweetId = @TweetId";

			int rowsEffected = Database.ExecuteCommand(
				query,
				configureCommand: cmd =>
					{
						cmd.Parameters.AddWithValueCheckNull("@TweetId", tweetId);

					});

			if (rowsEffected > 0)
			{
				return Ok();
			}

			return BadRequest();
		}

		private static int AddTweet(Tweet tweet)
		{
			string query = "insert into Tweets (Content,SendTime,SentBy) output Inserted.TweetId values (@Content,@SendTime,@SentBy)";

			int tweetId = 0;
			tweetId = Database.ExecuteCommand(
				query: query,
				configureCommand: cmd =>
					{
						cmd.Parameters.AddWithValueCheckNull("@Content", tweet.Content);
						cmd.Parameters.AddWithValueCheckNull("@SendTime", tweet.SendTime);
						cmd.Parameters.AddWithValueCheckNull("@SentBy", tweet.SentBy);
					},
				shouldExecuteScalar: true);

			return tweetId;
		}
	}
}