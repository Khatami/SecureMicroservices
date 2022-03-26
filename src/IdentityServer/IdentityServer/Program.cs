using IdentityServer;

var builder = WebApplication.CreateBuilder(args);

// IdentityServer
builder.Services
	.AddIdentityServer()
	.AddInMemoryClients(Config.Clients)
	//.AddInMemoryIdentityResources(Config.IdentityResources)
	//.AddInMemoryApiResources(Config.ApiResources)
	.AddInMemoryApiScopes(Config.ApiScopes)
	//.AddTestUsers(Config.TestUsers)
	.AddDeveloperSigningCredential();

var app = builder.Build();

app.UseHttpsRedirection();

// IdentityServer
app.UseRouting();

app.UseIdentityServer();

app.MapGet("/", async handler =>
{
	await handler.Response.WriteAsync("IdentityServer");
});

app.Run();