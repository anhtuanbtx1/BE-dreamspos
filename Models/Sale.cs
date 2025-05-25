using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PosStore.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.UtcNow;

        public int? CustomerId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty; // Cash, Card, Digital

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Completed"; // Pending, Completed, Cancelled, Refunded

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string? CashierName { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}
