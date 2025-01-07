using demo_discounts_api.Hubs;

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

var app = builder.Build();

app.UseCors("AllowOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<DiscountHub>("/discountHub");

app.Run();
