using FridgeApp.Data;
using FridgeApp.Interfaces;
using FridgeApp.Services;
using Microsoft.EntityFrameworkCore;

const string CorsPolicyName = "FlutterWebLocalhostPolicy";

var builder = WebApplication.CreateBuilder(args);

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
				if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
				{
					return false;
				}

				return uri.Scheme is "http" or "https" &&
					(uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
					 uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase));
			})
			  .AllowAnyMethod()
			  .AllowAnyHeader();
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
