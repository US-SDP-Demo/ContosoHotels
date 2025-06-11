using ContosoHotels.Data;
using ContosoHotels.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext with appropriate connection string
builder.Services.AddDbContext<ContosoHotelsContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    // If in development and SQL_PASSWORD environment variable exists, use it to replace placeholder
    if (builder.Environment.IsDevelopment())
    {
        var sqlPassword = Environment.GetEnvironmentVariable("SQL_PASSWORD");
        if (!string.IsNullOrEmpty(sqlPassword))
        {
            connectionString = connectionString?.Replace("${SQL_PASSWORD}", sqlPassword);
        }
    }
    
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });
});

// Add MVC services
builder.Services.AddControllersWithViews();

// Register custom services
builder.Services.AddScoped<DataSeedingService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
    // Seed the database in development
    using (var scope = app.Services.CreateScope())
    {
        var dataSeedingService = scope.ServiceProvider.GetRequiredService<DataSeedingService>();
        try
        {
            await dataSeedingService.SeedDataAsync();
        }
        catch (Exception ex)
        {
            // Log error but don't prevent startup
            Console.WriteLine($"Error seeding data: {ex.Message}");
        }
    }
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
