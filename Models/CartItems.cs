using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstScent.Models
{
    public class CartItems
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("FragranceId")]
        public int FragranceId { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int ItemCount { get; set; }
        public string? PictureUrl { get; set; }
        public string? Description { get; set; }
    }
}
