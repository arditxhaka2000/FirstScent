using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirstScent.Data;
using FirstScent.Models;

namespace FirstScent.Controllers
{
    public class WomensController : Controller
    {
        private readonly FirstScentContext _context;

        public WomensController(FirstScentContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? pageIndex, int pageSize = 6)
        {
            if (_context.Fragrances == null)
            {
                return Problem("Entity set 'MvcMovieContext.Movie'  is null.");
            }
            IQueryable<string> brandQuery = from m in _context.Fragrances
                                            orderby m.Brand
                                            select m.Brand;
            var fragrances = from m in _context.Fragrances
                             select m;

            var paginatedFragrances = Pagination<Fragrances>.Paginate(fragrances.Where(p => p.Category == "Female").ToList(), pageIndex ?? 1, pageSize);

            var fragranceType = new FragranceTypeViewModel
            {
                Brand = new SelectList(await brandQuery.Distinct().ToListAsync()),
                Fragrances = paginatedFragrances.Items.ToList(),
                PageIndex = paginatedFragrances.PageIndex,
                TotalPages = paginatedFragrances.TotalPages,
            };
            return View(fragranceType);
        }
        public async Task<IActionResult> NewIn(int? pageIndex, int pageSize = 6)
        {
            if (_context.Fragrances == null)
            {
                return Problem("Entity set 'MvcMovieContext.Movie'  is null.");
            }
            IQueryable<string> brandQuery = from m in _context.Fragrances
                                            orderby m.Brand
                                            select m.Brand;
            var fragrances = from m in _context.Fragrances
                             where m.NewIn == true
                             select m;


            var paginatedFragrances = Pagination<Fragrances>.Paginate(fragrances.Where(p => p.Category == "Female").ToList(), pageIndex ?? 1, pageSize);

            var fragranceType = new FragranceTypeViewModel
            {
                Brand = new SelectList(await brandQuery.Distinct().ToListAsync()),
                Fragrances = paginatedFragrances.Items.ToList(),
                PageIndex = paginatedFragrances.PageIndex,
                TotalPages = paginatedFragrances.TotalPages,
            };
            return View(fragranceType);
        }
        public async Task<IActionResult> BestSellers()
        {
            if (_context.Fragrances == null)
            {
                return Problem("Entity set 'MvcMovieContext.Movie'  is null.");
            }
            IQueryable<string> brandQuery = from m in _context.Fragrances
                                            orderby m.Brand
                                            select m.Brand;
            var fragrances = from m in _context.Fragrances
                             where m.IsBestSeller == true
                             select m;


            var fragranceType = new FragranceTypeViewModel
            {
                Brand = new SelectList(await brandQuery.Distinct().ToListAsync()),
                Fragrances = await fragrances.ToListAsync()

            };
            return View(fragranceType);
        }
        public async Task<IActionResult> Luxury()
        {
            if (_context.Fragrances == null)
            {
                return Problem("Entity set 'MvcMovieContext.Movie'  is null.");
            }
            IQueryable<string> brandQuery = from m in _context.Fragrances
                                            orderby m.Brand
                                            select m.Brand;
            var fragrances = from m in _context.Fragrances
                             where m.Price >= 120
                             select m;


            var fragranceType = new FragranceTypeViewModel
            {
                Brand = new SelectList(await brandQuery.Distinct().ToListAsync()),
                Fragrances = await fragrances.ToListAsync()

            };
            return View(fragranceType);
        }
        [HttpPost]
        public IActionResult FilterNewInFragrances(string[]? brand, string[]? category, string[]? price, string[]? size, string? sortingOption, int? pageIndex, int pageSize = 6)
        {
            // Implement your filtering logic here based on the selected options
            //var filteredFragrances = new Fragrance();
            var fragrances = _context.Fragrances.Where(p => p.Category == "Female" && p.NewIn == true).ToList();

            // Apply filters based on selected options
            if (brand.Length > 0)
            {
                fragrances = fragrances.Where(f => brand.Contains(f.Brand)).ToList();
            }


            if (category.Count() > 0)
            {
                fragrances = fragrances.Where(f => category.Contains(f.Category)).ToList();

            }

            if (price.Count() > 0)
            {
                List<Fragrances> filteredFragrances = new List<Fragrances>();
                foreach (var item in price)
                {
                    string[] priceValues = item.Split('-');

                    // Remove non-numeric characters
                    string fromValue = new string(priceValues[0].Where(char.IsDigit).ToArray());
                    string toValue = decimal.MaxValue.ToString();

                    if (priceValues.Length > 1)
                    {
                        toValue = new string(priceValues[1].Where(char.IsDigit).ToArray());
                    }
                    // Convert to decimal
                    decimal from = decimal.Parse(fromValue);
                    decimal to = decimal.Parse(toValue);

                    // Filter fragrances within the current price range
                    var fragrancesInRange = fragrances.Where(f => f.Price >= from && f.Price <= to);

                    // Add the filtered fragrances to the result
                    filteredFragrances.AddRange(fragrancesInRange);
                }
                fragrances = filteredFragrances;
            }

            if (size.Count() > 0)
            {
                var list = new List<int>();
                foreach (var item in size)
                {
                    string sizeValue = new string(item.Where(char.IsDigit).ToArray());
                    list.Add(Convert.ToInt32(sizeValue));

                }
                var newml = list.ToArray();
                fragrances = fragrances.Where(p => newml.Contains(p.SizeInMl)).ToList();

            }

            // Apply sorting based on the selected sorting option
            switch (sortingOption)
            {
                case "BestSeller":
                    fragrances = fragrances.OrderByDescending(f => f.IsBestSeller).ToList();

                    break;
                case "PriceHighToLow":
                    fragrances = fragrances.OrderByDescending(f => f.Price).ToList();
                    break;
                case "PriceLowToHigh":
                    fragrances = fragrances.OrderBy(f => f.Price).ToList();

                    break;
                case "NameAToZ":
                    fragrances = fragrances.OrderBy(f => f.Name).ToList();
                    break;
                    // Add more cases for additional sorting options
            }
            IQueryable<string> brandQuery = from m in _context.Fragrances
                                            orderby m.Brand
                                            select m.Brand;

            var paginatedFragrances = Pagination<Fragrances>.Paginate(fragrances.Where(p => p.Category == "Female").ToList(), pageIndex ?? 1, pageSize);

            var fragranceType = new FragranceTypeViewModel
            {
                Brand = new SelectList(brandQuery.Distinct().ToList()),
                Fragrances = paginatedFragrances.Items.ToList(),
                PageIndex = paginatedFragrances.PageIndex,
                TotalPages = paginatedFragrances.TotalPages,
                SelectedBrands = brand,
                SelectedCategory = category,
                SelectedPrice = price,
                SelectedSize = size,
                SortOption = sortingOption,
                FilterAction = "newIn",
            };

            return PartialView("_FilteredFragrances", fragranceType);


        }
        public IActionResult FilterBestSellerFragrances(string[] brand, string[] category, string[] price, string[] size, string sortingOption, int? pageIndex, int pageSize = 6)
        {
            // Implement your filtering logic here based on the selected options
            //var filteredFragrances = new Fragrance();
            var fragrances = _context.Fragrances.Where(p => p.Category == "Female" && p.IsBestSeller == true).ToList();

            // Apply filters based on selected options
            if (brand.Length > 0)
            {
                fragrances = fragrances.Where(f => brand.Contains(f.Brand)).ToList();
            }


            if (category.Count() > 0)
            {
                fragrances = fragrances.Where(f => category.Contains(f.Category)).ToList();

            }

            if (price.Count() > 0)
            {
                List<Fragrances> filteredFragrances = new List<Fragrances>();
                foreach (var item in price)
                {
                    string[] priceValues = item.Split('-');

                    // Remove non-numeric characters
                    string fromValue = new string(priceValues[0].Where(char.IsDigit).ToArray());
                    string toValue = decimal.MaxValue.ToString();

                    if (priceValues.Length > 1)
                    {
                        toValue = new string(priceValues[1].Where(char.IsDigit).ToArray());
                    }
                    // Convert to decimal
                    decimal from = decimal.Parse(fromValue);
                    decimal to = decimal.Parse(toValue);

                    // Filter fragrances within the current price range
                    var fragrancesInRange = fragrances.Where(f => f.Price >= from && f.Price <= to);

                    // Add the filtered fragrances to the result
                    filteredFragrances.AddRange(fragrancesInRange);
                }
                fragrances = filteredFragrances;
            }

            if (size.Count() > 0)
            {
                var list = new List<int>();
                foreach (var item in size)
                {
                    string sizeValue = new string(item.Where(char.IsDigit).ToArray());
                    list.Add(Convert.ToInt32(sizeValue));

                }
                var newml = list.ToArray();
                fragrances = fragrances.Where(p => newml.Contains(p.SizeInMl)).ToList();

            }

            // Apply sorting based on the selected sorting option
            switch (sortingOption)
            {
                case "BestSeller":
                    fragrances = fragrances.OrderByDescending(f => f.IsBestSeller).ToList();

                    break;
                case "PriceHighToLow":
                    fragrances = fragrances.OrderByDescending(f => f.Price).ToList();
                    break;
                case "PriceLowToHigh":
                    fragrances = fragrances.OrderBy(f => f.Price).ToList();

                    break;
                case "NameAToZ":
                    fragrances = fragrances.OrderBy(f => f.Name).ToList();
                    break;
                    // Add more cases for additional sorting options
            }
            IQueryable<string> brandQuery = from m in _context.Fragrances
                                            orderby m.Brand
                                            select m.Brand;
            var paginatedFragrances = Pagination<Fragrances>.Paginate(fragrances.Where(p => p.Category == "Female").ToList(), pageIndex ?? 1, pageSize);

            var fragranceType = new FragranceTypeViewModel
            {
                Brand = new SelectList(brandQuery.Distinct().ToList()),
                Fragrances = paginatedFragrances.Items.ToList(),
                PageIndex = paginatedFragrances.PageIndex,
                TotalPages = paginatedFragrances.TotalPages,
                SelectedBrands = brand,
                SelectedCategory = category,
                SelectedPrice = price,
                SelectedSize = size,
                SortOption = sortingOption,
                FilterAction = "bestSellers",

            };

            return PartialView("_FilteredFragrances", fragranceType);


        }
        [HttpPost]
        public IActionResult FilterFragrances(string[]? brand, string[]? category, string[]? price, string[]? size, string? sortingOption, int? pageIndex, int pageSize = 6)
        {
            // Implement your filtering logic here based on the selected options
            //var filteredFragrances = new Fragrance();
            var fragrances = _context.Fragrances.Where(p => p.Category == "Female").ToList();

            // Apply filters based on selected options
            if (brand.Length > 0)
            {
                fragrances = fragrances.Where(f => brand.Contains(f.Brand)).ToList();
            }


            if (category.Count() > 0)
            {
                fragrances = fragrances.Where(f => category.Contains(f.Category)).ToList();

            }

            if (price.Count() > 0)
            {
                List<Fragrances> filteredFragrances = new List<Fragrances>();
                foreach (var item in price)
                {
                    string[] priceValues = item.Split('-');

                    // Remove non-numeric characters
                    string fromValue = new string(priceValues[0].Where(char.IsDigit).ToArray());
                    string toValue = decimal.MaxValue.ToString();

                    if (priceValues.Length > 1)
                    {
                        toValue = new string(priceValues[1].Where(char.IsDigit).ToArray());
                    }
                    // Convert to decimal
                    decimal from = decimal.Parse(fromValue);
                    decimal to = decimal.Parse(toValue);

                    // Filter fragrances within the current price range
                    var fragrancesInRange = fragrances.Where(f => f.Price >= from && f.Price <= to);

                    // Add the filtered fragrances to the result
                    filteredFragrances.AddRange(fragrancesInRange);

                }
                fragrances = filteredFragrances;
            }

            if (size.Count() > 0)
            {
                var list = new List<int>();
                foreach (var item in size)
                {

                    string sizeValue = new string(item.Where(char.IsDigit).ToArray());
                    list.Add(Convert.ToInt32(sizeValue));
                }
                var newml = list.ToArray();
                fragrances = fragrances.Where(p => newml.Contains(p.SizeInMl)).ToList();

            }

            // Apply sorting based on the selected sorting option
            switch (sortingOption)
            {
                case "BestSeller":
                    fragrances = fragrances.OrderByDescending(f => f.IsBestSeller).ToList();

                    break;
                case "PriceHighToLow":
                    fragrances = fragrances.OrderByDescending(f => f.Price).ToList();
                    break;
                case "PriceLowToHigh":
                    fragrances = fragrances.OrderBy(f => f.Price).ToList();

                    break;
                case "NameAToZ":
                    fragrances = fragrances.OrderBy(f => f.Name).ToList();
                    break;
                    // Add more cases for additional sorting options
            }
            IQueryable<string> brandQuery = from m in _context.Fragrances
                                            orderby m.Brand
                                            select m.Brand;
            var paginatedFragrances = Pagination<Fragrances>.Paginate(fragrances.Where(p => p.Category == "Female").ToList(), pageIndex ?? 1, pageSize);

            var fragranceType = new FragranceTypeViewModel
            {
                Brand = new SelectList(brandQuery.Distinct().ToList()),
                Fragrances = paginatedFragrances.Items.ToList(),
                PageIndex = paginatedFragrances.PageIndex,
                TotalPages = paginatedFragrances.TotalPages,
                SelectedBrands = brand,
                SelectedCategory = category,
                SelectedPrice = price,
                SelectedSize = size,
                SortOption = sortingOption,
                FilterAction = "default",

            };

            return PartialView("_FilteredFragrances", fragranceType);

        }
        public async Task<IActionResult> Index1()
        {
            if (_context.Fragrances == null)
            {
                return Problem("Entity set 'MvcMovieContext.Movie'  is null.");
            }
            IQueryable<string> brandQuery = from m in _context.Fragrances
                                            orderby m.Brand
                                            select m.Brand;
            var fragrances = from m in _context.Fragrances
                             where m.Price >= 120
                             select m;


            var fragranceType = new FragranceTypeViewModel
            {
                Brand = new SelectList(await brandQuery.Distinct().ToListAsync()),
                Fragrances = await fragrances.ToListAsync()

            };
            return View(fragranceType);
        }
    }
}
