using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Movies.API.OperationFilters;
using Movies.API.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
	{
		options.Authority = "https://localhost:8888";
		options.TokenValidationParameters = new TokenValidationParameters()
		{
			ValidateAudience = false //TODO: ?
		};
	});

// Claims-based authorization
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("ClientIdPolicy", policy => 
		policy.RequireClaim("client_id", "movieClient" , "movies_mvc_client_interactive", "movies_mvc_client_hybrid"));

	options.AddPolicy("ScopePolicy", policy => policy.RequireClaim("scope", "movieAPI"));

	// options.AddPolicy("ScopePolicy", policy => policy.RequireClaim("given_name", "Seyedhamed")); //TODO: ?
});

// EF
builder.Services.AddDbContext<MoviesDbContext>(options =>
{
	options.UseInMemoryDatabase("Movies");
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
	{
		Type = SecuritySchemeType.OAuth2,
		Flows = new OpenApiOAuthFlows
		{
			ClientCredentials = new OpenApiOAuthFlow
			{
				AuthorizationUrl = new Uri("https://localhost:8888/connect/authorize"),
				TokenUrl = new Uri("https://localhost:8888/connect/token"),
				Scopes = new Dictionary<string, string>
				{
					{"movieAPI", "Any descriptions"}
				}
			}
		}
	});

	options.OperationFilter<AuthorizeCheckOperationFilter>();
});

var app = builder.Build();

// EF
using var scope = app.Services.CreateScope();
using var context = scope.ServiceProvider.GetService<MoviesDbContext>();
context.Database.EnsureCreated();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(options => {
		options.OAuthClientId("movieClient");
		options.OAuthClientSecret("secret");
	});
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();