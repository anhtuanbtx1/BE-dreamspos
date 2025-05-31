using System.ComponentModel.DataAnnotations;

namespace PosStore.DTOs
{
    public class WeddingGuestDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public int NumberOfPeople { get; set; }
        public decimal GiftAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Relationship { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateWeddingGuestDto
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(255)]
        public string? Unit { get; set; }
        
        [Range(1, int.MaxValue)]
        public int NumberOfPeople { get; set; } = 1;
        
        [Range(0, double.MaxValue)]
        public decimal GiftAmount { get; set; } = 0;
        
        public string Status { get; set; } = "Pending";

        public string? Relationship { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
        
        [StringLength(255)]
        public string? CreatedBy { get; set; }
    }

    public class UpdateWeddingGuestDto
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(255)]
        public string? Unit { get; set; }
        
        [Range(1, int.MaxValue)]
        public int NumberOfPeople { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal GiftAmount { get; set; }
        
        public string Status { get; set; } = string.Empty;

        public string? Relationship { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
        
        [StringLength(255)]
        public string? UpdatedBy { get; set; }
    }

    public class WeddingGuestQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public string? Unit { get; set; }
        public string? Relationship { get; set; }
        public string SortBy { get; set; } = "name";
        public string SortOrder { get; set; } = "asc";
    }

    public class WeddingGuestStatisticsDto
    {
        public int TotalGuests { get; set; }
        public int ConfirmedGuests { get; set; }
        public int PendingGuests { get; set; }
        public int DeclinedGuests { get; set; }
        public int TotalPeople { get; set; }
        public decimal TotalGiftAmount { get; set; }
        public decimal AverageGiftAmount { get; set; }
        public Dictionary<string, int> GuestsByRelationship { get; set; } = new();
        public List<UnitStatisticsDto> GuestsByUnit { get; set; } = new();
    }

    public class UnitStatisticsDto
    {
        public string Unit { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalGiftAmount { get; set; }
    }

    public class WeddingGuestSummaryDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public int NumberOfPeople { get; set; }
        public decimal GiftAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Relationship { get; set; }
    }
}
