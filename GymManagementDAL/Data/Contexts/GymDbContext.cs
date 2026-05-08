using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace GymManagementDAL.Data.Contexts
{
	public class GymDbContext : IdentityDbContext<ApplicationUser>
	{
		public GymDbContext(DbContextOptions<GymDbContext> dbContextOptions) : base(dbContextOptions)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

			modelBuilder.Entity<ApplicationUser>(EB =>
			{
				EB.Property(X => X.FirstName)
				.HasColumnType("varchar")
				.HasMaxLength(50);

				EB.Property(X => X.LastName)
				.HasColumnType("varchar")
				.HasMaxLength(50);
			});
		}

		#region DbSets
		public DbSet<TrainerEntity> Trainers { get; set; }
		public DbSet<BookingEntity> Bookings { get; set; }
		public DbSet<CategoryEntity> Categories { get; set; }
		public DbSet<HealthRecordEntity> HealthRecords { get; set; }
		public DbSet<MemberEntity> Members { get; set; }
		public DbSet<MembershipEntity> Memberships { get; set; }
		public DbSet<PlanEntity> Plans { get; set; }
		public DbSet<SessionEntity> Sessions { get; set; }

		#endregion



	}
}
