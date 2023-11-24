using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using NToastNotify;
using Product_MVC.Data;
using Product_MVC.Entities;
using Product_MVC.Funtion;
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
		Title = "Producr_MVC API",
		Version = "v1"
	});
});
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<VatCalculator>();
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

builder.Services.AddScoped<IEntityTypeConfiguration<IdentityRole>, RoleConfiguration>();
builder.Services.AddRazorPages().AddNToastNotifyNoty(new NotyOptions
{
	ProgressBar = true,
	Timeout = 1000
});

var app = builder.Build();
using (var serviceScope = app.Services.CreateScope())
{
	await Seed.SeedUsersAndRolesAsync(serviceScope.ServiceProvider);
}
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("Home/Error");
	app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseNToastNotify();

app.UseAuthorization();
app.UseAuthentication();
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
