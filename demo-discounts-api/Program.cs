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

if (connString != null)
{
    try
    {

        // Register DbContext
        builder.Services.AddDbContext<DemoDiscountsDbContext>(options =>
        {
            options.UseNpgsql(connString);
        });

        builder.Services.AddTransient<IDiscountCodeRepository, DiscountCodeDbRepository>();
    }
    catch
    {
        builder.Services.AddTransient<IDiscountCodeRepository, DiscountCodeMemoryRepository>();
    }
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
            .AllowCredentials(); ;
    });
});

builder.Services.AddTransient<IDiscountCodeService, DiscountCodeService>();

var app = builder.Build();

ApplicationLimits.MinCount = builder.Configuration.GetValue<int>("DiscountLimits:MinCount");
ApplicationLimits.MaxCount = builder.Configuration.GetValue<int>("DiscountLimits:MaxCount");
ApplicationLimits.MinLength = builder.Configuration.GetValue<int>("DiscountLimits:MinLength");
ApplicationLimits.MaxLength = builder.Configuration.GetValue<int>("DiscountLimits:MaxLength");


app.UseCors("AllowOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<DiscountHub>("/discountHub");

if (databaseDetected)
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<DemoDiscountsDbContext>();
        dbContext.Database.Migrate();
    }
}
app.Run();
