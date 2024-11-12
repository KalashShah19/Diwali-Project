namespace WebAPI.Models;

public class TeacherwithStudent
{
    public int Id { get; set; }
    public required string Image { get; set; }
    public required string Name { get; set; }
    public required string Standard { get; set; }
    public required string MobileNumber { get; set; }
    public required string EmailAddress { get; set; }
    public required DateTime JoiningDate { get; set; }
    public string? ParentId { get; set; }
}