using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Project.Controllers;

namespace Project
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
			Database.connectionString = Configuration["ConnectionStrings:conn1"];
			JwtManager.tokenSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["securityKey"]));

	}


	// This method gets called by the runtime. Use this method to add services to the container.
	public void ConfigureServices(IServiceCollection services)
		{
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						// What claims to validate
						ValidateIssuer = true,
						ValidateAudience = false ,
						
						ValidateIssuerSigningKey = true,

						// Valid values
						ValidIssuer = "Audilevy",
						IssuerSigningKey = JwtManager.tokenSecurityKey
					};
				});
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseDefaultFiles();
			app.UseStaticFiles();
			app.UseCors((options) => {
				options.WithHeaders("Content-type", "Authorization");
				options.WithMethods("Get", "Post");
				options.WithOrigins("http://localhost:3000");
			});
			app.UseAuthentication();
			app.UseMvc();
		}
	}
}
