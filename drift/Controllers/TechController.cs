using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using drift.Data;
using drift.Data.Entity;
using drift.Models.Dto;
using drift.Service;

namespace drift.Controllers
{
    public class TechController : Controller
    {
		private readonly ApprovingService _approvingService;

		public TechController(ApprovingService approvingService)
		{
			_approvingService = approvingService;
		}

		public async Task<IActionResult> Index()
		{
			return View(_approvingService.GetPendingTechApplications());
		}

		[HttpGet]
		public IActionResult Approve(int? id)
		{
			if (id != null)
			{
				var application = _approvingService.GetApplicationById(id.Value);
				application.ApprovedByTech= true;
				_approvingService.UpdateTechApplicationStatus(application);
			}
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
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
