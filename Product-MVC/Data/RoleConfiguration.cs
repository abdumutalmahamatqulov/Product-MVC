using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Product_MVC.Entities;
using Microsoft.AspNetCore.Builder;
using Product_MVC.Entities.Enum;

namespace Product_MVC.Data;

// RoleConfiguration klassi

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
	private readonly IServiceProvider _services;

	public RoleConfiguration(IServiceProvider services) => _services = services;

	public void Configure(EntityTypeBuilder<IdentityRole> builder)
	{
		using var scope = _services.CreateScope();
		var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

		var roles = Enum.GetNames<ERole>().Select(x => new IdentityRole(x.ToUpper()) { NormalizedName = roleManager.NormalizeKey(x.ToUpper()) });
		builder.HasData(roles);
	}
}

