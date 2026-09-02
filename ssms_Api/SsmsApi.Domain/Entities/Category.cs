using SsmsApi.Domain.Common;

namespace SsmsApi.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsServiceCategory { get; set; }

    public ICollection<WorkerSkill> WorkerSkills { get; set; } = new List<WorkerSkill>();
}