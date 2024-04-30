using ClinicWebApi.Data;
using ClinicWebApi.Extensions;
using ClinicWebApi.Models;
using ClinicWebApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get Connection String 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Add Database Service
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


//AUTH (Custom User)
builder.Services.AddAuthorization();


/*builder.Services.AddIdentity<BaseUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true; // Ensure emails are unique
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();*/

builder.Services.AddIdentityApiEndpoints<BaseUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
// Email configuration
builder.Services.AddFluentEmail(builder.Configuration);
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
// Token lifecycle
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(30);
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.MapIdentityApi<BaseUser>();
app.MapPost("/logout", async (SignInManager<BaseUser> signInManager) =>
{
    await signInManager.SignOutAsync().ConfigureAwait(false);
}).RequireAuthorization();
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
