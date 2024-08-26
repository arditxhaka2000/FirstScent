using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstScent.Models
{
    public class UserAdresses
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }

        public string? Country {  get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }

        public string? Phone { get; set; }
    }
}
