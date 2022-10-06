using BlogProject;
using BlogProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BlogDbContext>();

using (var db = new BlogDbContext())
{
   db.Database.EnsureCreated();
   db.Database.Migrate();
}

// Add services to the container.
builder.Services.AddControllersWithViews();

var assembly = Assembly.GetAssembly(typeof(MappingProfile));
builder.Services.AddAutoMapper(assembly);

builder.Services.AddAuthentication(opt => opt.DefaultScheme = "Cookies")
.AddCookie("Cookies", opt =>
{
   opt.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
   {
      OnRedirectToLogin = redirectContext =>
      {
         redirectContext.HttpContext.Response.StatusCode = 401;
         return Task.CompletedTask;
      }
   };
});

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
