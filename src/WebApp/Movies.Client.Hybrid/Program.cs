using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Movies.Client.Hybrid.HttpHandlers;
using OpenAPIConsumer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

/*
	It's only necessary to use IHttpContextAccessor when you need access to the HttpContext inside a service.
*/
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<AuthenticationDelegatingHandler>();

builder.Services.AddHttpClient("IDClient", client =>
{
	client.DefaultRequestHeaders.Clear();
	client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");

	client.BaseAddress = new Uri("https://localhost:8888");
}).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

builder.Services.AddHttpClient("MovieApiClient", client =>
{
	client.DefaultRequestHeaders.Clear();
	client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
}).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

builder.Services.AddTransient<MoviesAPIClient>(x =>
{
	var httpClient = x.GetService<IHttpClientFactory>().CreateClient("MovieApiClient");

	var movieAPIClientAddress = builder.Configuration.GetValue<string>("ApiGateways");
	return new MoviesAPIClient(movieAPIClientAddress, httpClient);
});

/*
	First of all, the OIDC authentication scheme and the JWT bearer authentication scheme are independent of each other.
	OIDC is mostly used for server-side authentication and will pretty much never be used on its own but always with the cookie scheme.
	The reason for this is that the OIDC scheme will just be used for the authentication but is not able to persist the information on its own.

	As for JWT bearer, this authentication scheme will run on every request since it is completely 
	stateless and expects clients to authenticate themselves using the Authorization header all the time.
	This makes it primarily used for protecting APIs since browsers wouldn’t be able to provide a JWT for normal browser requests.

	So you should first ask yourself whether you are protecting your server-side web application (e.g. using Razor views or Razor pages) in which case 
	you want to use OIDC and the cookies authentication scheme, or if you are protecting your API. 
	Of course, the answer could be “both” in which case you want all of those three schemes but ASP.NET Core will not support this without further configuration.
*/

// Authentication
builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;  //TODO: ?
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
	options.Authority = "https://localhost:8888";

	options.ClientId = "movies_mvc_client_hybrid";
	options.ClientSecret = "secret";

	// OIDC Authentication Flows
	options.ResponseType = "code id_token";

	var predifniedScopes = options.Scope; //OpenID, Profile

	// IdentityResources
	options.Scope.Add("allowedservices");
	options.Scope.Add("roles");

	// ApiScopes
	options.Scope.Add("movies.getall");

	var predifinedClaimActions = options.ClaimActions;
	options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.WebSite, JwtClaimTypes.WebSite);
	options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.PreferredUserName, JwtClaimTypes.PreferredUserName);
	options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.Role, JwtClaimTypes.Role);
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
	options.TokenValidationParameters = new TokenValidationParameters()
	{
		NameClaimType = JwtClaimTypes.PreferredUserName,
		RoleClaimType = JwtClaimTypes.Role
	};

	options.Events =  new OpenIdConnectEvents
	{
		OnAccessDenied = context =>
		{
			return Task.CompletedTask;
		},

		OnAuthenticationFailed = context =>
		{
			context.Response.Redirect("/Home/Error");
			context.HandleResponse(); // Suppress the exception

			return Task.CompletedTask;
		},

		/*
			Steps are different based on chosen flow -> these steps are for hybrid flow.
		*/

		// Step (1)
		OnRedirectToIdentityProvider = context =>
		{
			return Task.CompletedTask;
		},

		// Step (2)
		OnMessageReceived = context =>
		{
			return Task.CompletedTask;
		},

		// Step (3)
		OnTokenValidated = context =>
		{
			var idToken = context.SecurityToken;

			string userIdentifier = idToken.Subject;

			return Task.CompletedTask;
		},

		// Step (4)
		OnAuthorizationCodeReceived = context =>
		{
			var authorizationCode = context.ProtocolMessage.Code;

			return Task.CompletedTask;
		},

		// Step (5)
		OnTokenResponseReceived = context =>
		{
			return Task.CompletedTask;
		},

		// Step (6)
		OnUserInformationReceived = context =>
		{
			return Task.CompletedTask;
		},

		// Step (7)
		OnTicketReceived = context =>
		{
			// If your authentication logic is based on users then add your logic here
			return Task.CompletedTask;
		},

		// Logout: Step (1)
		OnRedirectToIdentityProviderForSignOut = context =>
		{
			return Task.CompletedTask;
		},

		// Logout: Step (2)
		OnSignedOutCallbackRedirect = context =>
		{
			return Task.CompletedTask;
		},

		OnRemoteFailure = context =>
		{
			return Task.CompletedTask;
		},

		OnRemoteSignOut = context =>
		{
			return Task.CompletedTask;
		},
	};
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