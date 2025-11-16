using APP.Domain;
using APP.Models;
using APP.Services;
using CORE.APP.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the IoC container.
var connectionString = builder.Configuration.GetConnectionString(nameof(Db));
builder.Services.AddDbContext<DbContext, Db>(options => options.UseSqlite(connectionString));

//builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<IService<GroupRequest, GroupResponse>, GroupService>();
builder.Services.AddScoped<IService<RoleRequest, RoleResponse>, RoleService>();
builder.Services.AddScoped<IService<UserRequest, UserResponse>, UserService>();




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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
