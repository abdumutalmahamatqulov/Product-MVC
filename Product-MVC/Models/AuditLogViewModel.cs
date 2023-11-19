using Product_MVC.Entities;

namespace Product_MVC.Models;

public class AuditLogViewModel
{
	public DateTime FromDate { get; set; }
	public DateTime ToDate { get; set; }
	public string? Name { get; set; }
	public List<AuditLog>? FilteredLogs { get; set; }
}
