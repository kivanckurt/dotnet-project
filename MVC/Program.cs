using APP.Domain;
using APP.Models;
using APP.Services;
using CORE.APP.Services;
using CORE.APP.Services.Authentication.MVC;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the IoC container.
var connectionString = builder.Configuration.GetConnectionString(nameof(Db));
builder.Services.AddDbContext<DbContext, Db>(options => options.UseSqlite(connectionString));

//builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<IService<GroupRequest, GroupResponse>, GroupService>();
builder.Services.AddScoped<IService<RoleRequest, RoleResponse>, RoleService>();
builder.Services.AddScoped<IService<UserRequest, UserResponse>, UserService>();
builder.Services.AddScoped<IService<BlogRequest, BlogResponse>, BlogService>();
builder.Services.AddScoped<IService<TagRequest, TagResponse>, TagService>();


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICookieAuthService, CookieAuthService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // changed from /Users/Login to /Login since route was changed for the action
        options.AccessDeniedPath = "/Login"; // changed from /Users/Login to /Login since route was changed for the action
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });





builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
