using System.ComponentModel.DataAnnotations;

namespace ECommerceWebApp.ViewModels
{
    public class ReviewViewModel
    {
        public int ProductId { get; set; }
        
        [Required]
        [Range(1, 5, ErrorMessage = "Please select a rating between 1 and 5 stars")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        [Display(Name = "Your Review")]
        public string? Comment { get; set; }

        // For display purposes
        public string ProductName { get; set; } = string.Empty;
    }
}
