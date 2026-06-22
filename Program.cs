using FridgeApp.Data;
using FridgeApp.Interfaces;
using FridgeApp.Services;
using Microsoft.EntityFrameworkCore;

const string CorsPolicyName = "FlutterWebCorsPolicy";
const string AllowedCorsOriginsConfigKey = "ALLOWED_CORS_ORIGINS";

var builder = WebApplication.CreateBuilder(args);
var allowedCorsOrigins = ParseAllowedCorsOrigins(
	builder.Configuration[AllowedCorsOriginsConfigKey]);

if (!builder.Environment.IsDevelopment())
{
	var port = Environment.GetEnvironmentVariable("PORT");
	if (string.IsNullOrWhiteSpace(port))
	{
		port = "8080";
	}

	builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
	options.AddPolicy(CorsPolicyName, policy =>
	{
		policy.SetIsOriginAllowed(origin =>
			{
				// Localhost origins are for Flutter web development on changing ports.
				if (IsLocalFlutterWebOrigin(origin))
				{
					return true;
				}

				// ALLOWED_CORS_ORIGINS is for hosted web demos such as Netlify.
				var normalizedOrigin = NormalizeOrigin(origin);
				return normalizedOrigin != null &&
					allowedCorsOrigins.Contains(normalizedOrigin);
			})
		  .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
		  .WithHeaders("Content-Type", "Authorization");
	});
});

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IFridgeService, FridgeService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductCatalogService, ProductCatalogService>();
builder.Services.AddScoped<IFridgeQuickAddService, FridgeQuickAddService>();
builder.Services.AddScoped<IRecipeSuggestionService, RecipeSuggestionService>();
builder.Services.AddScoped<IShoppingListService, ShoppingListService>();
builder.Services.AddScoped<IDemoBootstrapService, DemoBootstrapService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

//if (!app.Environment.IsDevelopment())
//{
//	app.UseHttpsRedirection();
//}

app.UseCors(CorsPolicyName);

app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapMethods("/{*path}", ["OPTIONS"], () => Results.Ok()).RequireCors(CorsPolicyName);

app.MapControllers();

app.Run();

static HashSet<string> ParseAllowedCorsOrigins(string? rawOrigins)
{
	return (rawOrigins ?? string.Empty)
		.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
		.Select(NormalizeOrigin)
		.Where(origin => origin != null)
		.Select(origin => origin!)
		.ToHashSet(StringComparer.OrdinalIgnoreCase);
}

static bool IsLocalFlutterWebOrigin(string origin)
{
	if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
	{
		return false;
	}

	return uri.Scheme is "http" or "https" &&
		(uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
		 uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase));
}

static string? NormalizeOrigin(string origin)
{
	if (!Uri.TryCreate(origin.Trim().TrimEnd('/'), UriKind.Absolute, out var uri))
	{
		return null;
	}

	if (uri.Scheme is not "http" and not "https" || string.IsNullOrWhiteSpace(uri.Host))
	{
		return null;
	}

	return uri.GetLeftPart(UriPartial.Authority);
}
