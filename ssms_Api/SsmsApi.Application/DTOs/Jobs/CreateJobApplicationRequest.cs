namespace SsmsApi.Application.DTOs.Jobs;

public class CreateJobApplicationRequest
{
    public decimal ProposedPrice { get; set; }
    public string? Message { get; set; }
}