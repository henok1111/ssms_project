namespace SsmsApi.Domain.Entities;

public class WorkerSkill
{
    public Guid WorkerProfileId { get; set; }
    public WorkerProfile WorkerProfile { get; set; } = null!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}