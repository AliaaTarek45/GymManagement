using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.AnalyticsViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;

namespace GymManagementBLL.Services.Classes
{
	public class AnalyticsService : IAnalyticsService
	{
		private readonly IUnitOfWork _unitOfWork;

		public AnalyticsService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

        public async Task<AnalyticsViewModel> GetAnalyticsDataAsync(CancellationToken ct = default)
        {
            var Sessions = await _unitOfWork.GetRepository<SessionEntity>().GetAllAsync();
        

            var totalMembers = await _unitOfWork.GetRepository<MemberEntity>().CountAsync(ct: ct);
            var totalTrainers = await _unitOfWork.GetRepository<TrainerEntity>().CountAsync(ct: ct);
            var activeMembers = await _unitOfWork.MembershipRepository.CountAsync(m => m.EndDate > DateTime.Now, ct);

            return new AnalyticsViewModel()
			{
                TotalMembers = totalMembers,
                TotalTrainers = totalTrainers,
                ActiveMembers = activeMembers,
				UpcomingSessions = Sessions.Count(X => X.StartDate > DateTime.Now),
				OngoingSessions = Sessions.Count(X => X.StartDate <= DateTime.Now && X.EndDate >= DateTime.Now),
				CompletedSessions = Sessions.Count(X => X.EndDate < DateTime.Now)
			};
		}
	}
}
