using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PosStore.Models
{
    public class Project
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("ProjectName")]
        public string ProjectName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string ClientName { get; set; } = string.Empty;

        [Required]
        public long CategoryId { get; set; }

        [Required]
        [StringLength(10)]
        public string Priority { get; set; } = "medium"; // low, medium, high

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "planning"; // planning, in-progress, review, completed, on-hold

        [Required]
        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal Budget { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal ActualCost { get; set; } = 0.00m;

        [Required]
        [Range(0, 100)]
        public byte ProgressPercentage { get; set; } = 0;

        [Required]
        public long CreatedBy { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // [Column(TypeName = "datetime2")]
        // public DateTime? DeletedAt { get; set; }  // Temporarily disabled until DB is updated

        // Navigation properties
        public virtual User? CreatedByUser { get; set; }
        public virtual ProjectCategory? Category { get; set; }
    }
}
