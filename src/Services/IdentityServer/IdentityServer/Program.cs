using IdentityServer;
using IdentityServerHost.Quickstart.UI;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

//Quickstart Options
AccountOptions.AutomaticRedirectAfterSignOut = true;

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