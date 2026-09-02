using SsmsApi.Domain.Common;

namespace SsmsApi.Domain.Entities;

public class SupplierProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public string ShopName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsVerified { get; set; } = false;

    public ICollection<MaterialItem> MaterialItems { get; set; } = new List<MaterialItem>();
}