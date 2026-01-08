using KKHCleanBus.MicroServices.Data;
using KKHCleanBus.MicroServices.Services;
using Microsoft.EntityFrameworkCore;
using KKHCleanBus.MicroServices.Extensions;

var builder = WebApplication.CreateBuilder(args);


// Add DbContext with SQLite
builder.Services.AddDbContext<CleanBusDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services
builder.Services.AddScoped<NewsService>();
builder.Services.AddScoped<ArrivalTimeService>();

// Add controllers
builder.Services.AddControllers();

// Configure CORS from appsettings via extension
builder.Services.AddCorsSetting(builder.Configuration);

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CleanBusDbContext>();
    dbContext.Database.EnsureCreated();
}

// 移除 HTTPS 重定向，Render 會在負載平衡層處理
// app.UseHttpsRedirection();

// Use CORS via extension
app.UseCorsSetting();

// Healthcheck 端點
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.MapControllers();
app.Run();
