using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstScent.Models
{
    public class Fragrances
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string? Name { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$")]
        [Required]
        [StringLength(30)]
        public string? Category { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$")]
        [Required]
        [StringLength(30)]
        public string? Brand { get; set; }

        [Range(1, 1000)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required]
        public int SizeInMl { get; set; }

        [NotMapped]
        [Display(Name = "Upload Picture")]
        public IFormFile? Picture { get; set; }

        public string? PictureUrl { get; set; }
        public string? Description { get; set; }
        public bool IsBestSeller { get; set; }
        public bool NewIn { get; set; }

    }
}
