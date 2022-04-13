using IdentityServer;
using IdentityServer.Extensions;
using IdentityServer4.Services;
using IdentityServerHost.Quickstart.UI;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

//Quickstart Options
AccountOptions.AutomaticRedirectAfterSignOut = true;

// CORS
// We should enable CORS for Swagger UI authorization
if (builder.Environment.IsDevelopment())
{
	builder.Services.AddSingleton<ICorsPolicyService>((container) =>
	{
		var logger = container.GetRequiredService<ILogger<DefaultCorsPolicyService>>();
		return new DefaultCorsPolicyService(logger)
		{
			AllowedOrigins = { "https://localhost:6500" }
		};
	});
}

// IdentityServer
var identityServerBuilder = builder.Services
	.AddIdentityServer()
	.AddDeveloperSigningCredential();

// In Memory
//identityServerBuilder
//	.AddInMemoryClients(Config.Clients)
//	.AddInMemoryIdentityResources(Config.IdentityResources)
//	.AddInMemoryApiResources(Config.ApiResources)
//	.AddInMemoryApiScopes(Config.ApiScopes)
//	.AddTestUsers(Config.TestUsers);

// IdentityServer4 EF Integration
string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
string connectionString = builder.Configuration.GetConnectionString("IdentityServerDbContext");

identityServerBuilder
	.AddTestUsers(Config.TestUsers)
	.AddConfigurationStore(options =>
	{
		options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
			sql => sql.MigrationsAssembly(assemblyName));
	})
	.AddOperationalStore(options =>
	{
		options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
			sql => sql.MigrationsAssembly(assemblyName));
	});

var app = builder.Build();

app.MigrateDatabase();

app.UseHttpsRedirection();

app.UseStaticFiles();

// IdentityServer
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

// MVC
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();