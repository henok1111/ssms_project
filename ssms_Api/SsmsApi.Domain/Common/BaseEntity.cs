namespace SsmsApi.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public int PhoneNumber {get; set ;}
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;


}