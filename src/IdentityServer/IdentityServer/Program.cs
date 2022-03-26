using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// IdentityServer
builder.Services
	.AddIdentityServer()
	.AddInMemoryClients(new List<Client>())
	.AddInMemoryIdentityResources(new List<IdentityResource>())
	.AddInMemoryApiResources(new List<ApiResource>())
	.AddInMemoryApiScopes(new List<ApiScope>())
	.AddTestUsers(new List<TestUser>())
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