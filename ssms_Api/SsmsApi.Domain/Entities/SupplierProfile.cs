using SsmsApi.Domain.Common;
using SsmsApi.Domain.Enums;
namespace SsmsApi.Domain.Entities;
public class SupplierProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public string ShopName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    public ICollection<MaterialItem> MaterialItems { get; set; } = new List<MaterialItem>();
}