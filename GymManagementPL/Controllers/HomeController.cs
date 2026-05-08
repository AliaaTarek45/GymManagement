using GymManagementBLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		private readonly IAnalyticsService _analyticsService;

		public HomeController(IAnalyticsService analyticsService)
		{
			_analyticsService = analyticsService;
		}
        public async Task<IActionResult> Index(CancellationToken ct)
          => View(await _analyticsService.GetAnalyticsDataAsync(ct));
    }
}
