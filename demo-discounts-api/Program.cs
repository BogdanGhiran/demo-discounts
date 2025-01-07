using demo_discounts_api.Constants;
using demo_discounts_api.Data;
using demo_discounts_api.Hubs;
using demo_discounts_api.Repositories;
using demo_discounts_api.Repositories.Contracts;
using demo_discounts_api.Services;
using demo_discounts_api.Services.Contracts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
bool databaseDetected = false;
string connString = builder.Configuration.GetConnectionString("DemoDiscountsDb");

if (!string.IsNullOrEmpty(connString))
{
    try
    {
        // Temporarily create a DbContextOptionsBuilder to test the connection
        var optionsBuilder = new DbContextOptionsBuilder<DemoDiscountsDbContext>();
        optionsBuilder.UseNpgsql(connString);

        using (var dbContext = new DemoDiscountsDbContext(optionsBuilder.Options))
        {
            // Test the connection by opening it
            dbContext.Database.OpenConnection();
            dbContext.Database.CloseConnection();
        }

        // Register DbContext if the connection succeeds
        builder.Services.AddDbContext<DemoDiscountsDbContext>(options =>
        {
            options.UseNpgsql(connString);
        });

        builder.Services.AddTransient<IDiscountCodeRepository, DiscountCodeDbRepository>();
        databaseDetected = true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database connection failed: {ex.Message}");
        builder.Services.AddTransient<IDiscountCodeRepository, DiscountCodeFileRepository>();
    }
}
else
{
    builder.Services.AddTransient<IDiscountCodeRepository, DiscountCodeFileRepository>();
}

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigins", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "https://demo-discounts-client.gab16.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddTransient<IDiscountCodeService, DiscountCodeService>();

var app = builder.Build();

// Set application limits from configuration
ApplicationLimits.MinCount = builder.Configuration.GetValue<int>("DiscountLimits:MinCount");
ApplicationLimits.MaxCount = builder.Configuration.GetValue<int>("DiscountLimits:MaxCount");
ApplicationLimits.MinLength = builder.Configuration.GetValue<int>("DiscountLimits:MinLength");
ApplicationLimits.MaxLength = builder.Configuration.GetValue<int>("DiscountLimits:MaxLength");

app.UseCors("AllowOrigins");
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHub<DiscountHub>("/discountHub");

// Apply migrations if the database was successfully detected
if (databaseDetected)
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<DemoDiscountsDbContext>();
        dbContext.Database.Migrate();
    }
}

app.Run();
