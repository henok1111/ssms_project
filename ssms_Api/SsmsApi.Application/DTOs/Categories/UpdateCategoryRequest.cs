namespace SsmsApi.Application.DTOs.Categories;

public class UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public bool IsServiceCategory { get; set; }
}