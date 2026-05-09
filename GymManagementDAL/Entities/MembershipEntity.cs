using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagementDAL.Entities
{
    public class MembershipEntity : BaseEntity
    {
        public DateTime EndDate { get; set; }

        public int MemberId { get; set; }
        public MemberEntity Member { get; set; } = default!;

        public int PlanId { get; set; }
        public PlanEntity Plan { get; set; } = default!;

        [NotMapped]
        public string Status => EndDate > DateTime.Now ? "Active" : "Expired";

        [NotMapped]
        public bool IsActive => EndDate > DateTime.Now;
    }
}
