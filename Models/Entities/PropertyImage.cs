namespace EmlakSitesi.Models.Entities;

public class PropertyImage
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public int SortOrder { get; set; }
}