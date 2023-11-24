using System.Data.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Product_MVC.Entities;

namespace Product_MVC.Data;

public class AppDbContext : IdentityDbContext<User>
{
	//private IServiceProvider services;
	public AppDbContext(DbContextOptions<AppDbContext> options, IServiceProvider services) : base(options)
	{
		this.Service = services;
	}
	public IServiceProvider Service { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<AuditLog> AuditLogs { get; set; }
	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder.Entity<Product>();
		base.OnModelCreating(builder);
		builder.ApplyConfiguration<IdentityRole>(new RoleConfiguration(Service));
	}
}