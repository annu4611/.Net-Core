using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [Required]
        public int ProductId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        // Navigation Properties
        public virtual Product Product { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
