using AutoMapper;
using GymManagementBLL.Common;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.PlanViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;

namespace GymManagementBLL.Services.Classes
{
	public class PlanService : IPlanService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public PlanService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

        public async Task<IReadOnlyList<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<PlanEntity>().GetAllAsync(ct: ct);
            return _mapper.Map<IReadOnlyList<PlanViewModel>>(plans);
        }
        public async Task<PlanViewModel?> GetPlanByIdAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<PlanEntity>().GetByIdAsync(planId, ct);
            return plan is null ? null : _mapper.Map<PlanViewModel>(plan);
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<PlanEntity>().GetByIdAsync(planId, ct);
            if (plan is null || !plan.IsActive) return null;
            if (await HasActiveMembershipsAsync(planId, ct)) return null;
            return _mapper.Map<UpdatePlanViewModel>(plan);
        }

        public async Task<Result> ToggleActivationAsync(int planId, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetRepository<PlanEntity>();
            var plan = await repo.GetByIdAsync(planId, ct);
            if (plan is null) return Result.NotFound("Plan not found.");

            if (plan.IsActive && await HasActiveMembershipsAsync(planId, ct))
                return Result.Fail("Cannot deactivate a plan that has active memberships.");

            plan.IsActive = !plan.IsActive;
			plan.UpdatedAt = DateTime.Now;
            repo.Update(plan);
           var result =  await _unitOfWork.SaveChangesAsync(ct);
            return result>0 ? Result.Ok(): Result.Fail("Failed to Toggle Plan Status");
        }
        public async Task<Result> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetRepository<PlanEntity>();
            var plan = await repo.GetByIdAsync(id, ct);
            if (plan is null) return Result.NotFound("Plan not found.");
            if (await HasActiveMembershipsAsync(id, ct))
                return Result.Fail("Cannot edit a plan that has active memberships.");

            _mapper.Map(model, plan);
            plan.UpdatedAt = DateTime.Now;
            repo.Update(plan);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Ok();
        }

        #region Helper Methods

        private async Task<bool> HasActiveMembershipsAsync(int planId, CancellationToken ct)
        {
            return await _unitOfWork.MembershipRepository.AnyAsync(m => m.PlanId == planId && m.EndDate > DateTime.Now, ct);
        }
 
		#endregion
	}
}
