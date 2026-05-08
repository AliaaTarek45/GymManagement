using GymManagementBLL.Services.AttachmentService;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.MemberViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
	[Authorize(Roles = "SuperAdmin")]
	public class MemberController : Controller
	{
		private readonly IMemberService _memberService;
        private readonly IAttachmentService _attachmentService;

        public MemberController(IMemberService memberService , IAttachmentService attachmentService)
		{
			_memberService = memberService;
            _attachmentService = attachmentService;
        }
		#region Get All Members
        public async Task<IActionResult> Index(CancellationToken ct)
           => View(await _memberService.GetAllMembersAsync(ct));
        #endregion

        #region Create Member 

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);

            var result = await _memberService.CreateMemberAsync(model, ct);
            if (result.Success)
                TempData["SuccessMessage"] = "Member created successfully.";
            else
                TempData["ErrorMessage"] = result.Error;

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Show Member Data 
        public async Task<IActionResult> Picture(int id)
        {
            var member = await _memberService.GetMemberDetailsAsync(id);
            if (member is null || string.IsNullOrEmpty(member.Photo))
                return NotFound();

           
            var result = _attachmentService.GetFile(member.Photo, "MembersPictures");
            if (result is null) return NotFound();

            return File(result.Value.Stream, result.Value.ContentType);
        }
        [HttpGet]
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsAsync(id, ct);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [HttpGet]
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var record = await _memberService.GetMemberHealthRecordAsync(id, ct);
            if (record is null)
            {
                TempData["ErrorMessage"] = "Health record not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(record);
        }

        #endregion

        #region Member Data Edit 
        [HttpGet]
        public async Task<IActionResult> MemberEdit(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberToUpdateAsync(id, ct);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [HttpPost]
        public async Task<IActionResult> MemberEdit(int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _memberService.UpdateMemberDetailsAsync(id, model, ct);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Member updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result.Error;
            return View(model);
        }
        #endregion

        #region Delete Member 

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsAsync(id, ct);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MemberId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _memberService.RemoveMemberAsync(id, ct);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
                result.Success ? "Member deleted successfully." : result.Error;
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
