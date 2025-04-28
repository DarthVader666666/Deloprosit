using Deloprosit.Bll;
using Deloprosit.Bll.Interfaces;
using Deloprosit.Bll.Services;
using Deloprosit.Data;
using Deloprosit.Data.Entities;
using Deloprosit.Server.Configurations;

using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;

using System.Text.Json.Serialization;

using static Google.Apis.Drive.v3.DriveService;

var builder = WebApplication.CreateBuilder(args);
var usePostgres = false;

ConfigurationHelper.Initialize(builder.Configuration, builder.Environment.WebRootPath);

builder.Services.AddLogging(logs =>
{
    logs.AddConsole();
});

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddAuthentication(o =>
{
    o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
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

var origins = builder.Configuration.GetSection("CorsOrigins")?.AsEnumerable()?.Select(x => x.Value ?? string.Empty).ToArray();

builder.Services.AddCors(options => options.AddPolicy("AllowClient",
    new CorsPolicyBuilder().WithOrigins(origins ?? [])
    .AllowAnyHeader().AllowAnyMethod().AllowCredentials().Build()));

var connectionString = builder.Configuration.GetConnectionString("MssqlDeloprositDb");

if (connectionString == null)
{
    usePostgres = true;
    connectionString = builder.Configuration.GetConnectionString("PostgresDeloprositDb");
    builder.Services.AddDbContext<PostgresDeloprositDbContext>(optionsBuilder => optionsBuilder.UseNpgsql(connectionString));
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
}
else
{
    builder.Services.AddDbContext<MssqlDeloprositDbContext>(optionsBuilder => optionsBuilder.UseSqlServer(connectionString));
}

if (!usePostgres)
{
    builder.Services.AddScoped<IRepository<User>, UserRepository>(ConfigureRepository<MssqlDeloprositDbContext, UserRepository>);
    builder.Services.AddScoped<IRepository<Role>, RoleRepository>(ConfigureRepository<MssqlDeloprositDbContext, RoleRepository>);
    builder.Services.AddScoped<IRepository<Chapter>, ChapterRepository>(ConfigureRepository<MssqlDeloprositDbContext, ChapterRepository>);
    builder.Services.AddScoped<IRepository<Theme>, ThemeRepository>(ConfigureRepository<MssqlDeloprositDbContext, ThemeRepository>);
    builder.Services.AddScoped<IRepository<Captcha>, CaptchaRepository>(ConfigureRepository<MssqlDeloprositDbContext, CaptchaRepository>);
    builder.Services.AddScoped<IRepository<Message>, MessageRepository>(ConfigureRepository<MssqlDeloprositDbContext, MessageRepository>);
}
else
{
    builder.Services.AddScoped<IRepository<User>, UserRepository>(ConfigureRepository<PostgresDeloprositDbContext, UserRepository>);
    builder.Services.AddScoped<IRepository<Role>, RoleRepository>(ConfigureRepository<PostgresDeloprositDbContext, RoleRepository>);
    builder.Services.AddScoped<IRepository<Chapter>, ChapterRepository>(ConfigureRepository<PostgresDeloprositDbContext, ChapterRepository>);
    builder.Services.AddScoped<IRepository<Theme>, ThemeRepository>(ConfigureRepository<PostgresDeloprositDbContext, ThemeRepository>);
    builder.Services.AddScoped<IRepository<Captcha>, CaptchaRepository>(ConfigureRepository<PostgresDeloprositDbContext, CaptchaRepository>);
    builder.Services.AddScoped<IRepository<Message>, MessageRepository>(ConfigureRepository<PostgresDeloprositDbContext, MessageRepository>);
}

builder.Services.AddSingleton<CryptoService>();
builder.Services.AddScoped<EmailSender>();
builder.Services.AddScoped<UserManager>();
builder.Services.AddSingleton<DriveService>(provider =>
{
    var cryptoService = provider.GetService<CryptoService>();
    var secrets = builder.Configuration["GoogleDrive:Secrets"];
    var decryptedContent = cryptoService?.Decrypt(secrets);
    var credential = GoogleCredential.FromJson(decryptedContent);

    if (credential.IsCreateScopedRequired)
    {
        credential = credential.CreateScoped(ScopeConstants.DriveFile);
    }

    var driveService = new DriveService(new BaseClientService.Initializer()
    {
        HttpClientInitializer = credential,
        ApplicationName = builder.Configuration["GoogleDrive:ApplicationName"] ?? string.Empty
    });

    return driveService;
});

builder.Services.AddSingleton<GoogleDriveService>();

builder.Services.ConfigureAutomapper();

var provider = builder?.Services?.BuildServiceProvider();
using var scope = provider?.CreateScope();
MigrateSeedDatabase(scope);
UploadDocuments(scope);

var app = builder.Build();

app.UseStatusCodePagesWithReExecute("/error/{0}");

app.UseDefaultFiles();
app.UseStaticFiles();

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

if (app.Environment.IsProduction())
{
    app.MapWhen(httpContext => !httpContext.Request.Path.Value.StartsWith("/api"), appBuilder =>
    {
        appBuilder.UseRouting();

        appBuilder.UseEndpoints(endpoints =>
        {
            endpoints.MapFallbackToFile("index.html");
        });
    });
}

app.Run();

TRepository ConfigureRepository<TDbContext, TRepository>(IServiceProvider provider) where TDbContext : DeloprositDbContext where TRepository : class
{
    return Activator.CreateInstance(typeof(TRepository), provider.GetRequiredService<TDbContext>()) as TRepository ?? throw new NullReferenceException();
}

void MigrateSeedDatabase(IServiceScope? scope)
{
    DeloprositDbContext? dbContext = null;

    if (!usePostgres)
    {
        dbContext = scope?.ServiceProvider.GetRequiredService<MssqlDeloprositDbContext>();
    }
    else
    {
        dbContext = scope?.ServiceProvider.GetRequiredService<PostgresDeloprositDbContext>();     
    }

    if (dbContext != null)
    {
        try
        { 
            dbContext?.Database.Migrate();
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

void UploadDocuments(IServiceScope? scope)
{
    var driveService = scope?.ServiceProvider.GetRequiredService<GoogleDriveService>();
    Task.Run(() => driveService?.RestoreAllDocuments());
}
