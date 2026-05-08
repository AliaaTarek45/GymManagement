using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.MembershipViewModels;
using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace GymManagementPL.Controllers
{
    [Authorize]
    public class MembershipController : Controller
    {
        private readonly IMembershipService _membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
            => View(await _membershipService.GetAllMembershipsAsync(ct));

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateDropdownsAsync(ct);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMemberShipViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(ct);
                return View(model);
            }

            var result = await _membershipService.CreateMembershipAsync(model, ct);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Membership created successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.Error;
            await PopulateDropdownsAsync(ct);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id, CancellationToken ct)
        {
            var result = await _membershipService.DeleteActiveMembershipAsync(id, ct);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
                result.Success ? "Membership cancelled." : result.Error;
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdownsAsync(CancellationToken ct)
        {
            ViewBag.Plans = new SelectList(await _membershipService.GetPlansForDropDownAsync(ct), "Id", "Name");
            ViewBag.Members = new SelectList(await _membershipService.GetMembersForDropDownAsync(ct), "Id", "Name");
        }
    }
}
