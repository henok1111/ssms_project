using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SsmsApi.Domain.Entities;

namespace SsmsApi.Infrastructure.Persistence.Configurations;

public class WorkerSkillConfiguration : IEntityTypeConfiguration<WorkerSkill>
{
    public void Configure(EntityTypeBuilder<WorkerSkill> builder)
    {
        // Composite primary key — no single Id needed, the pair IS the identity.
        builder.HasKey(ws => new { ws.WorkerProfileId, ws.CategoryId });

        builder.HasOne(ws => ws.WorkerProfile)
            .WithMany(w => w.Skills)
            .HasForeignKey(ws => ws.WorkerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ws => ws.Category)
            .WithMany(c => c.WorkerSkills)
            .HasForeignKey(ws => ws.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}