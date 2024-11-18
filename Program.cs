using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RedisSqlDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddSingleton<DataService>(); // Register DataService
builder.Services.AddRazorPages(); // Add Razor Pages support
builder.Services.AddControllers(); // Add support for API controllers

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseRouting();
app.UseAuthorization();

app.MapControllers(); // Map API controllers
app.MapRazorPages(); // Map Razor Pages

// Pre-load Redis data at startup
var dataService = app.Services.GetRequiredService<DataService>();
await dataService.PreLoadRedisDataAsync();


app.Run();