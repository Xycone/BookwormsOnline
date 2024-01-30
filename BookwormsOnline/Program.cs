using BookwormsOnline.Extensions;
using BookwormsOnline.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddDbContext<ActivityLogsDbContext>();
builder.Services.AddIdentity<BookwormsUser, IdentityRole>( options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;

    // Account lockout
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
}).AddEntityFrameworkStores<AuthDbContext>();


builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";
    options.AccessDeniedPath = "/Account/AccessDenied";
});
builder.Services.AddAuthorization(options =>
{
});

builder.Services.ConfigureApplicationCookie(Config =>
{
	Config.Cookie.HttpOnly = true;
    Config.ExpireTimeSpan = TimeSpan.FromMinutes(10);
	Config.LoginPath = "/Login";
	Config.AccessDeniedPath = "/Login";
	Config.SlidingExpiration = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
