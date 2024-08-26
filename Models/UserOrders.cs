using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstScent.Models
{
    public class UserOrders
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }

        public string? OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }
        public int OrderItemsId { get; set; }
        public int isPayed { get; set; }
        public int Status { get; set; }
        public decimal TotalSum { get; set; }

        [NotMapped]
        public List<OrderItems> items { get; set; }

    }
}
