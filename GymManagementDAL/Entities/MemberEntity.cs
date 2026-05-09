namespace GymManagementDAL.Entities
{
    public class MemberEntity : GymUser
    {
        public string Photo { get; set; } = default!;
        public HealthRecordEntity HealthRecord { get; set; } = default!;
        public ICollection<BookingEntity> MemberSessions { get; set; } = default!;

        public ICollection<MembershipEntity> MemberPlans { get; set; } = default!;
    }
}
