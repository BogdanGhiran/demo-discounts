using demo_discounts_api.Constants;
using demo_discounts_api.Hubs;
using demo_discounts_api.Repositories;
using demo_discounts_api.Repositories.Contracts;
using demo_discounts_api.Services;
using demo_discounts_api.Services.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

builder.Services.AddTransient<IDiscountCodeRepository, DiscountCodeMemoryRepository>();
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

app.Run();
