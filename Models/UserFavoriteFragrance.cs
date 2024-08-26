using System.ComponentModel.DataAnnotations;

namespace FirstScent.Models
{
    public class UserFavoriteFragrance
    {
        [Key]
        public int Id { get; set; }
        public string? ApplicationUserId { get; set; }
        public int FragranceId {  get; set; }
        public string? PictureUrl { get; set; }


    }
}
