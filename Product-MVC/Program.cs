using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Product_MVC.Data;
using Product_MVC.Entities;
using Product_MVC.Repositories;
using Product_MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Sozlashlar
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "TeamProjectMVC API",
		Version = "v1"
	});
});
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("Connection"))
);

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
	options.Password.RequireLowercase = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireDigit = false;
})
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<AppDbContext>();

// IdentityRole konfiguratsiyasini qo'shish
builder.Services.AddScoped<IEntityTypeConfiguration<IdentityRole>, RoleConfiguration>();


var app = builder.Build();

// Hammasi amalga oshirildi, endi konfiguratsiyalar qismini ko'rishimiz mumkin

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
//app.Use(async (context, next) =>
//{
//	if (context.Request.Path.StartsWithSegments("/swagger"))
//	{
//		await Console.Out.WriteLineAsync(context.User.Identity!.IsAuthenticated.ToString());
//		if (context.User.Identity.IsAuthenticated)
//		{
//			if (!context.User.IsInRole("ADMIN"))
//			{
//				context.Response.StatusCode = 403;
//				return;
//			}
//		}
//		else
//		{
//			context.Response.Redirect("/Auth/Login");
//			return;
//		}
//	}
//	await next.Invoke();
//});
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.UseAuthentication();
app.MapControllerRoute(
	name: "table",
	pattern: "Product/table",
	defaults: new { controller = "Product", action = "Table" });

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
