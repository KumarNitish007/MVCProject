using AuthenticationAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagementService.Services;
using Microsoft.Extensions.Options;
using UserManagementService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// For Entity Framework dbcontext
var Configuration = builder.Configuration;
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(Configuration.GetConnectionString("SQLDBConnection")));

// For Identity injecting the services
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
});


// Add Configuration for required Email
builder.Services.Configure<IdentityOptions>(options => options.SignIn.RequireConfirmedEmail = true);
// Adding EmailService and EmailConfiguration
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.Configure<EmailConfigration>(Configuration.GetSection("EmailConfigration"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<EmailConfigration>>().Value);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
