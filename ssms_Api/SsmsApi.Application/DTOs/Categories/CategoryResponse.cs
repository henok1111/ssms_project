namespace SsmsApi.Application.DTOs.Categories;

public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsServiceCategory { get; set; }
}