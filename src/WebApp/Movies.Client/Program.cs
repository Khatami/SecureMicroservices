using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using OpenAPIConsumer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<MoviesAPIClient>(x =>
{
	string baseAddress = builder.Configuration.GetValue<string>("OpenAPIConsumer:Movies.API");
	return new MoviesAPIClient(baseAddress, new HttpClient());
});

// Authentication
builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;  //TODO: ?
	options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;  //TODO: ?
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)  //TODO: ?
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>  //TODO: ?
{
	options.Authority = "https://localhost:6600";

	options.ClientId = "movies_mvc_client";
	options.ClientSecret = "secret";
	options.ResponseType = "code";

	options.Scope.Add("openid");
	options.Scope.Add("profile");

	options.SaveTokens = true; //TODO: ?

	options.GetClaimsFromUserInfoEndpoint = true; //TODO: ?
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();