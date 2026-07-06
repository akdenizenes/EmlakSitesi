namespace EmlakSitesi.Models.Entities;

public class Property
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }

    public int CityId { get; set; }
    public City City { get; set; } = null!;
    public int DistrictId { get; set; }
    public District District { get; set; } = null!;
    public string Neighborhood { get; set; } = null!;
    public string Address { get; set; } = null!;

    public int RoomCount { get; set; }
    public int LivingRoomCount { get; set; }
    public int BathroomCount { get; set; }
    public int BalconyCount { get; set; }
    public int Floor { get; set; }
    public int TotalFloors { get; set; }
    public int GrossArea { get; set; }
    public int NetArea { get; set; }
    public int BuildYear { get; set; }
    public string HeatingType { get; set; } = null!;
    public decimal? Dues { get; set; }
    public bool HasParking { get; set; }
    public bool HasElevator { get; set; }
    public bool IsFurnished { get; set; }

    public ListingType ListingType { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; } = true;
    public int ViewCount { get; set; }
    public string? MapEmbedUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
}

public enum ListingType
{
    Satilik = 1,
    Kiralik = 2
}