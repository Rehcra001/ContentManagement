using ContentManagement.BlazorServer.Components;
using ContentManagement.BlazorServer.Components.Account;
using ContentManagement.BlazorServer.Data;
using ContentManagement.BlazorServer.Options;
using ContentManagement.BlazorServer.Services;
using ContentManagement.BlazorServer.Services.Contracts;
using ContentManagement.Repositories;
using ContentManagement.Repositories.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Serilog
var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

//Serilog
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Host.UseSerilog(logger);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("ContentManagementAccessDB") ?? throw new InvalidOperationException("Connection string 'ContentManagementAccessDB' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

//builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, EmailService>();

//The default inactivity timeout is 14 days. The following code sets the inactivity timeout to 2 hours with sliding expiration:
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
});


//The following code changes all data protection tokens timeout period to 3 hours:
//builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
//{
//    options.TokenLifespan = TimeSpan.FromHours(3);
//});

//Add email service
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));

//Data Connection
builder.Services.AddScoped<IRelationalDBConnection, RelationalDBConnection>();

//Repositories
builder.Services.AddScoped<IPersonRepository, PersonRepository>();

//Services
builder.Services.AddScoped<IPersonService, PersonService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    //Seed roles for development
    RoleManager<IdentityRole> adminRoleManager = builder.Services.BuildServiceProvider().GetService<RoleManager<IdentityRole>>()!;
    RoleManager<IdentityRole> authorRoleManager = builder.Services.BuildServiceProvider().GetService<RoleManager<IdentityRole>>()!;
    RoleManager<IdentityRole> userRoleManager = builder.Services.BuildServiceProvider().GetService<RoleManager<IdentityRole>>()!;
    UserManager<ApplicationUser> defaultAdminManager = builder.Services.BuildServiceProvider().GetService<UserManager<ApplicationUser>>()!;

    await SeedRoles.Seed(adminRoleManager, authorRoleManager, userRoleManager, defaultAdminManager);
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

//Not needed if hosted on the same server as the client
//Can be made more secure by only allowing certain origins
//app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSerilogRequestLogging();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();





