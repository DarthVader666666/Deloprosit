using Deloprosit.Data;

using JsonFlatFileDataStore;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;

var jsonFileCreated = false;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(logs =>
{
    logs.AddConsole();
});

builder.Services.AddControllers();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

builder.Services.AddCors(options => options.AddPolicy("AllowAll",
    new CorsPolicyBuilder().AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().Build()));

if (builder.Environment.IsDevelopment())
{
    var connectionString = builder.Configuration.GetConnectionString("DeloprositDb");
    builder.Services.AddDbContext<DeloprositDbContext>(options => options.UseSqlServer(connectionString));
}
else
{
    var path = $"{Directory.GetCurrentDirectory()}\\DeloprositDbJson\\";

    if (!Directory.Exists(path))
    {
        Directory.CreateDirectory(path);
    }

    path += "deloprositDb.json";

    if (!File.Exists(path))
    {
        var stream = File.Create(path);
        stream.Close();
        File.WriteAllText(path, "{}");
        jsonFileCreated = true;
    }

    builder.Services.AddScoped(serviceProvider => new DataStore(path, useLowerCamelCase: false));
}

var provider = builder?.Services?.BuildServiceProvider();
using var scope = provider?.CreateScope();
await MigrateSeedDatabase(scope, jsonFileCreated);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

async Task MigrateSeedDatabase(IServiceScope? scope, bool jsonFileCreated)
{
    if (builder!.Environment.IsDevelopment())
    {
        var dbContext = scope?.ServiceProvider.GetRequiredService<DeloprositDbContext>();
        dbContext?.Database.Migrate();
    }
    else if (jsonFileCreated)
    {
        var dataStore = scope?.ServiceProvider.GetRequiredService<DataStore>() ?? throw new ArgumentNullException("Could not get DataStore from DI");
        await SeedJsonDb(dataStore, builder.Configuration);
    }
}

async Task SeedJsonDb(DataStore dataStore, IConfiguration configuration)
{
    var adminEmail = configuration["AdminEmail"] ?? throw new ArgumentNullException("AdminEmail is null.");
    var adminPassword = configuration["AdminPassword"] ?? throw new ArgumentNullException("AdminPassword is null.");

}