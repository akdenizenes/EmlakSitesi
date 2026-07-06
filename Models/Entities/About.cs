namespace EmlakSitesi.Models.Entities;

public class About
{
    public int Id { get; set; }
    public string CompanyDescription { get; set; } = null!;
    public string Mission { get; set; } = null!;
    public string Vision { get; set; } = null!;
    public int FoundedYear { get; set; }
    public string? ProfileImagePath { get; set; }
}