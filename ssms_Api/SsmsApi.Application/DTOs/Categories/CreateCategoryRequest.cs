namespace SsmsApi.Application.DTOs.Categories;

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public bool IsServiceCategory { get; set; }
}