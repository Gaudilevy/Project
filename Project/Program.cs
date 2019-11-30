using System;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Project.Controllers;

namespace Project
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
			JwtManager.CheckExpiredTokens();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
		    WebHost.CreateDefaultBuilder(args)
			  .UseStartup<Startup>();
	}
}
