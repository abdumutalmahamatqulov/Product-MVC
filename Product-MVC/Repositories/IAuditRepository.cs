using Product_MVC.Entities;
using Product_MVC.Models;

namespace Product_MVC.Repositories;

public interface IAuditRepository
{
	Task<AuditLogViewModel> Index(DateTime? fromDate, DateTime? toDate, string name);
	Task<List<AuditLog>> SortByUserName(string name);

	Task<List<AuditLog>> GetFiltered(string? fromDate, string? toDate);
	Task<List<AuditLog>> GetAllAudits();
}
