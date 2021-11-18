using drift.Data;
using drift.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace drift.Controllers
{
	public class MedicalController : Controller
	{
		private readonly ApprovingService _approvingService;

		public MedicalController(ApprovingService approvingService, ApplicationDbContext context)
		{
			_approvingService = approvingService;
		}

		// GET: Medical
		public async Task<IActionResult> Index()
		{
			return View(_approvingService.GetPendingMedicsApplications());
		}

		[HttpGet]
		public IActionResult Approve(int? id)
		{
			if (id != null)
			{
				var application = _approvingService.GetApplicationById(id.Value);
				application.ApprovedByMedics = true;
				_approvingService.UpdateMedicalApplicationStatus(application);
			}
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Decline(int? id)
		{
			if (id != null)
			{
				var application = _approvingService.GetApplicationById(id.Value);
				application.Ignore = true;
				_approvingService.UpdateMedicalApplicationStatus(application);
			}
			return RedirectToAction(nameof(Index));
		}
	}
}
