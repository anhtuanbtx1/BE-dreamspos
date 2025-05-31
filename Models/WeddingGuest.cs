using System.ComponentModel.DataAnnotations;

namespace PosStore.Models
{
    public class WeddingGuest
    {
        public long Id { get; set; }
        
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(255)]
        public string? Unit { get; set; }
        
        [Range(1, int.MaxValue)]
        public int NumberOfPeople { get; set; } = 1;
        
        [Range(0, double.MaxValue)]
        public decimal GiftAmount { get; set; } = 0;
        
        [Required]
        public GuestStatus Status { get; set; } = GuestStatus.Pending;
        
        public RelationshipType? Relationship { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        
        [StringLength(255)]
        public string? CreatedBy { get; set; }
        
        [StringLength(255)]
        public string? UpdatedBy { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    public enum GuestStatus
    {
        Pending,
        Going,
        NotGoing
    }

    public enum RelationshipType
    {
        Family,
        Friend,
        Colleague,
        Other
    }
}
