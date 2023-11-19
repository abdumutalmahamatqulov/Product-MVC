using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Product_MVC.Entities;
using Product_MVC.Repositories;

namespace Product_MVC.Controllers;
[Authorize(Roles = "ADMIN")]
[Route("api/[controller]")]
public class AuditViewController : ControllerBase
{
	private readonly IAuditRepository _auditRepository;

	public AuditViewController(IAuditRepository context) => _auditRepository = context;
	//public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, string Name) => View(await _auditRepository.Index(fromDate, toDate, Name));
	[HttpGet("List")]
	[ProducesResponseType(typeof(List<AuditLog>),StatusCodes.Status200OK)]
	public async Task <IActionResult >GetAllAudits() =>Ok(await _auditRepository.GetAllAudits());
	[HttpGet]
	[ProducesResponseType(typeof(List<AuditLog>), 200)]
	[ProducesResponseType(typeof(List<AuditLog>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
	public async Task <IActionResult>GetFiltered(string fromDate,string toDate)
	{
		try
		{
			return Ok(await _auditRepository.GetFiltered(fromDate, toDate));
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}
	}
	[HttpGet("Name")]
	[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> SortByUserName(string name)
	{
		try
		{
			return Ok(await _auditRepository.SortByUserName(name));
		}
		catch (Exception ex)
		{
			return BadRequest(ex.Message);
		}

	}
}
