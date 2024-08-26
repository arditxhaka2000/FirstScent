using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstScent.Models
{
    public class OrderItems
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int OrderId { get; set; }
        public string? PictureUrl { get; set; }
        public string? Name { get; set; }

    }
}
