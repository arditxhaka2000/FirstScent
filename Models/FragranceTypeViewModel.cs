using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FirstScent.Models
{
    public class FragranceTypeViewModel
    {
        public List<Fragrances>? Fragrances { get; set; }
        public SelectList? Category { get; set; }
        public SelectList? Brand { get; set; }
        public string? FragranceCategory { get; set; }
        public string? FragranceBrand { get; set; }
        public string? SearchString { get; set; }
        public string? SortOption { get; set; }
        public string[]? SelectedBrands { get; set; }
        public string[]? SelectedCategory { get; set; }
        public string[]? SelectedPrice { get; set; }
        public string[]? SelectedSize { get; set; }
        public string FilterAction { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalPages { get; set; }


    }
}
