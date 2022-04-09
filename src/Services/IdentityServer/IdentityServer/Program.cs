using IdentityServer;
using IdentityServer4.Services;
using IdentityServerHost.Quickstart.UI;

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
builder.Services
	.AddIdentityServer()
	.AddInMemoryClients(Config.Clients)
	.AddInMemoryIdentityResources(Config.IdentityResources)
	.AddInMemoryApiResources(Config.ApiResources)
	.AddInMemoryApiScopes(Config.ApiScopes)
	.AddTestUsers(Config.TestUsers)
	.AddDeveloperSigningCredential();

var app = builder.Build();

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