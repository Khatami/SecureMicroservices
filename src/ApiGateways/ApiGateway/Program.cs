using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ocelot
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
	config.AddJsonFile($"ocelot.json", false, true);
});

var authenticationProviderKey = "IdentityApiKey";

builder.Services.AddAuthentication()
	.AddJwtBearer(authenticationProviderKey, options => {
		options.Authority = "https://localhost:8888";

		options.TokenValidationParameters = new TokenValidationParameters()
		{
			ValidateAudience = false, //TODO: ?
		};
	});

builder.Services
	.AddOcelot();

var app = builder.Build();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
	endpoints.MapGet("/", async context =>
	{
		await context.Response.WriteAsync("Ocelot API Gateway");
	});
});

app.UseOcelot();

app.Run();