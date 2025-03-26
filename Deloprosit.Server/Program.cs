using Deloprosit.Bll.Interfaces;
using Deloprosit.Bll.Services;
using Deloprosit.Data;
using Deloprosit.Data.Entities;
using Deloprosit.Server;
using Deloprosit.Server.Configurations;

using JsonFlatFileDataStore;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text.Json.Serialization;

const string azureEnvironment = "Production";
var jsonFileCreated = false;
var builder = WebApplication.CreateBuilder(args);

ConfigurationHelper.Initialize(builder.Configuration, builder.Environment);

builder.Services.AddLogging(logs =>
{
    logs.AddConsole();
});

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
        options.Cookie.Name = "Deloprosit_Cookies";
        //options.Cookie.MaxAge = TimeSpan.FromDays(1);
        //options.Cookie.SameSite = SameSiteMode.None;
        options.Events.OnRedirectToLogin = (context) =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Cookie.HttpOnly = false;
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options => options.AddPolicy("AllowClient",
    new CorsPolicyBuilder().WithOrigins("http://localhost:5173", "https://localhost:5173", "https://deloprosit.azurewebsites.net")
    .AllowAnyHeader().AllowAnyMethod().AllowCredentials().Build()));

string? connectionString = null;

if (builder.Environment.IsDevelopment())
{
    connectionString = builder.Configuration.GetConnectionString("MssqlDeloprositDb");
}
else if (builder.Environment.EnvironmentName.Equals(azureEnvironment, StringComparison.OrdinalIgnoreCase))
{
    connectionString = builder.Configuration.GetConnectionString("PostgresDeloprositDb");
}

if (connectionString == null)
{
    throw new NullReferenceException();
}

builder.Services.AddDbContext<MssqlDeloprositDbContext>(optionsBuilder => optionsBuilder.UseSqlServer(connectionString));
builder.Services.AddDbContext<PostgresDeloprositDbContext>(optionsBuilder => optionsBuilder.UseNpgsql(connectionString));
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

if (builder.Environment.IsDevelopment()){

    builder.Services.AddScoped<IRepository<User>, UserRepository>(ConfigureRepository<MssqlDeloprositDbContext, UserRepository>);
    builder.Services.AddScoped<IRepository<Role>, RoleRepository>(ConfigureRepository<MssqlDeloprositDbContext, RoleRepository>);
    builder.Services.AddScoped<IRepository<Chapter>, ChapterRepository>(ConfigureRepository<MssqlDeloprositDbContext, ChapterRepository>);
    builder.Services.AddScoped<IRepository<Theme>, ThemeRepository>(ConfigureRepository<MssqlDeloprositDbContext, ThemeRepository>);
    builder.Services.AddScoped<IRepository<Message>, MessageRepository>(ConfigureRepository<MssqlDeloprositDbContext, MessageRepository>);
}
else if(builder.Environment.EnvironmentName.Equals(azureEnvironment, StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<IRepository<User>, UserRepository>(ConfigureRepository<PostgresDeloprositDbContext, UserRepository>);
    builder.Services.AddScoped<IRepository<Role>, RoleRepository>(ConfigureRepository<PostgresDeloprositDbContext, RoleRepository>);
    builder.Services.AddScoped<IRepository<Chapter>, ChapterRepository>(ConfigureRepository<PostgresDeloprositDbContext, ChapterRepository>);
    builder.Services.AddScoped<IRepository<Theme>, ThemeRepository>(ConfigureRepository<PostgresDeloprositDbContext, ThemeRepository>);
    builder.Services.AddScoped<IRepository<Message>, MessageRepository>(ConfigureRepository<PostgresDeloprositDbContext, MessageRepository>);
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
CreateFolders();

var app = builder.Build();

app.UseStatusCodePagesWithReExecute("/error", "?status={0}");

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

TRepository ConfigureRepository<TDbContext, TRepository>(IServiceProvider provider) where TDbContext: DeloprositDbContext where TRepository: class
{
    return Activator.CreateInstance(typeof(TRepository), provider.GetRequiredService<TDbContext>()) as TRepository ?? throw new NullReferenceException();
}

async Task MigrateSeedDatabase(IServiceScope? scope, bool jsonFileCreated)
{
    if (builder!.Environment.IsDevelopment())
    {
        var dbContext = scope?.ServiceProvider.GetRequiredService<MssqlDeloprositDbContext>();
        dbContext?.Database.Migrate();
    }
    else if (builder!.Environment.EnvironmentName.Equals(azureEnvironment, StringComparison.OrdinalIgnoreCase))
    {
        var dbContext = scope?.ServiceProvider.GetRequiredService<PostgresDeloprositDbContext>();

        try
        {
            if (dbContext != null && dbContext.Database.EnsureCreated())
            {
                dbContext?.Database.Migrate();
            }
        }
        catch (NpgsqlException ex)
        { 
            Console.WriteLine(ex.Message);
        }        
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

void CreateFolders()
{
    var path = $"{Directory.GetCurrentDirectory()}\\wwwroot\\docs";

    if (!Directory.Exists(path))
    {
        Directory.CreateDirectory(path);
    }
}