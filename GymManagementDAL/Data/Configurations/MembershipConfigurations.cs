using GymManagementDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Data.Configurations
{
	internal class MembershipConfigurations : IEntityTypeConfiguration<MembershipEntity>
	{
		public void Configure(EntityTypeBuilder<MembershipEntity> builder)
		{
            builder.HasKey(m => m.Id);
            builder.Property(X => X.CreatedAt)
				   .HasColumnName("StartDate")
				   .HasDefaultValueSql("GETDATE()");

            builder.HasOne(m => m.Plan)
                          .WithMany(p => p.PlanMembers)
                          .HasForeignKey(m => m.PlanId)
                          .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Member)
                   .WithMany(me => me.MemberPlans)
                   .HasForeignKey(m => m.MemberId)
                   .OnDelete(DeleteBehavior.Cascade);
		}
	}
}
