using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SsmsApi.Domain.Entities;

namespace SsmsApi.Infrastructure.Persistence;

public class SsmsDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public SsmsDbContext(DbContextOptions<SsmsDbContext> options) : base(options) { }

    // World 1 — People & Identity
    public DbSet<WorkerProfile> WorkerProfiles => Set<WorkerProfile>();
    public DbSet<ClientProfile> ClientProfiles => Set<ClientProfile>();
    public DbSet<SupplierProfile> SupplierProfiles => Set<SupplierProfile>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<WorkerSkill> WorkerSkills => Set<WorkerSkill>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // World 2 — Work Lifecycle
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<JobAttachment> JobAttachments => Set<JobAttachment>();

    // World 3 — Supply Chain
    public DbSet<MaterialItem> MaterialItems => Set<MaterialItem>();
    public DbSet<JobMaterialRequest> JobMaterialRequests => Set<JobMaterialRequest>();
    public DbSet<MaterialOrder> MaterialOrders => Set<MaterialOrder>();
    public DbSet<Quote> Quotes => Set<Quote>();

    // World 4 — Money, Trust & Communication
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Dispute> Disputes => Set<Dispute>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // must come first — sets up Identity's own tables

        builder.ApplyConfigurationsFromAssembly(typeof(SsmsDbContext).Assembly);

        // Soft-delete filter — same pattern as TMS M5's HasQueryFilter,
        // applied globally to every entity inheriting BaseEntity.
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(Domain.Common.BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, nameof(Domain.Common.BaseEntity.IsDeleted));
                var condition = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
                var lambda = System.Linq.Expressions.Expression.Lambda(condition, parameter);
                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}