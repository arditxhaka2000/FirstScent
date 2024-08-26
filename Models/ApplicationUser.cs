using Microsoft.AspNetCore.Identity;

namespace FirstScent.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<UserAdresses> Adresses { get; set; }
        public List<UserOrders> Orders { get; set; }
        public List<OrderItems> OrderItems { get; set; }
        public List<UserFavoriteFragrance> favoriteFragrances { get; set; }
    }
}
