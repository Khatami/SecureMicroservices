using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Movies.API.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //TODO: ?
	.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
	{
		options.Authority = "https://localhost:8888";
		options.TokenValidationParameters = new TokenValidationParameters()
		{
			ValidateAudience = false //TODO: ?
		};
	});

// Claim-based authorization
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "movieClient"));
	options.AddPolicy("ScopePolicy", policy => policy.RequireClaim("scope", "movieAPI"));
});

// EF
builder.Services.AddDbContext<MoviesDbContext>(options =>
{
	options.UseInMemoryDatabase("Movies");
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// EF
using var scope = app.Services.CreateScope();
using var context = scope.ServiceProvider.GetService<MoviesDbContext>();
context.Database.EnsureCreated();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();