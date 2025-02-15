using Deloprosit.Bll.Interfaces;
using Deloprosit.Bll.Services;
using Deloprosit.Data;
using Deloprosit.Data.Entities;
using Deloprosit.Server.Configurations;

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

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
        options.Cookie.Name = "Deloprosit_Cookies";
        //options.Cookie.SameSite = SameSiteMode.None;
        options.Events.OnRedirectToLogin = (context) =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Cookie.HttpOnly = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options => options.AddPolicy("AllowClient",
    new CorsPolicyBuilder().WithOrigins("http://localhost:5173", "https://localhost:5173")
    .AllowAnyHeader().AllowAnyMethod().AllowCredentials().Build()));

if (builder.Environment.IsDevelopment())
{
    var connectionString = builder.Configuration.GetConnectionString("DeloprositDb");
    builder.Services.AddDbContext<DeloprositDbContext>(options => options.UseSqlServer(connectionString));
    builder.Services.AddScoped<IRepository<User>, UserRepository>();
    builder.Services.AddScoped<IRepository<Role>, RoleRepository>();
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

builder.Services.AddScoped<CryptoService>();
builder.Services.AddScoped<EmailSender>();
builder.Services.AddScoped<UserManager>();
builder.Services.ConfigureAutomapper();

var provider = builder?.Services?.BuildServiceProvider();
using var scope = provider?.CreateScope();
await MigrateSeedDatabase(scope, jsonFileCreated);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseCors("AllowClient");
app.UseCookiePolicy(
    new CookiePolicyOptions
    {
        Secure = CookieSecurePolicy.Always
    });
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
}