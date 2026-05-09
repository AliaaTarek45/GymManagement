namespace GymManagementBLL.ViewModels.MembershipViewModels
{
	public class MemberShipViewModel
	{
		public int MemberId { get; set; }
		public int PlanId { get; set; }
		public string MemberName { get; set; } = default!;
		public string PlanName { get; set; } = default!;
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}
}
