using Microsoft.AspNetCore.Authentication;
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
	options.Authority = "https://localhost:8888";

	options.ClientId = "movies_mvc_client";
	options.ClientSecret = "secret";
	options.ResponseType = "code";

	var predifniedScopes = options.Scope; //OpenID, Profile
	options.Scope.Add("AllowedServices");

	var predifinedClaimActions = options.ClaimActions;
	options.ClaimActions.MapUniqueJsonKey("website", "website");
	options.ClaimActions.MapUniqueJsonKey("services", "services");

	options.SaveTokens = true; //TODO: ?

	// Boolean to set whether the handler should go to user info endpoint to retrieve
	// additional claims or not after creating an identity from id_token received from
	// token endpoint. The default is 'false'.
	options.GetClaimsFromUserInfoEndpoint = true;

	/*
		Microsoft and IdentityServer have different opinion on what the name of the claims should be,
		so you need to point out, which claim is the name claim, by using:
	*/
	options.TokenValidationParameters.NameClaimType = "given_name";
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