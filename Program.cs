using KKHCleanBus.MicroServices.Data;
using KKHCleanBus.MicroServices.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 支援 Render 的 PORT 環境變數
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://*:{port}");

// Add DbContext with SQLite
builder.Services.AddDbContext<CleanBusDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services
builder.Services.AddScoped<NewsService>();
builder.Services.AddScoped<ArrivalTimeService>();

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CleanBusDbContext>();
    dbContext.Database.EnsureCreated();
}

// 移除 HTTPS 重定向，Render 會在負載平衡層處理
// app.UseHttpsRedirection();

// Healthcheck 端點
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.MapControllers();
app.Run();
