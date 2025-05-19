using System.ComponentModel.DataAnnotations; // Optional

namespace Service.DTOs
{
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Product title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Unit price must be non-negative.")]
        public int UnitPrice { get; set; } // Or decimal

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be non-negative.")]
        public int Quantity { get; set; }
    }
}