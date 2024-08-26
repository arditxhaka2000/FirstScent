using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirstScent.Data;
using FirstScent.Models;

namespace FirstScent.Controllers
{
    public class BrandsController : Controller
    {
        private readonly FirstScentContext _context;
        public BrandsController(FirstScentContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string brandName, string fName)
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

            if (!string.IsNullOrEmpty(fName))
            {
                fragrances = fragrances.Where(s => s.Name!.Contains(fName));
            }

            if (!string.IsNullOrEmpty(brandName))
            {
                fragrances = fragrances.Where(x => x.Brand == brandName);
            }

            var fragranceType = new FragranceTypeViewModel
            {
                Brand = new SelectList(await brandQuery.Distinct().ToListAsync()),
                FragranceBrand = brandName,
                Fragrances = await fragrances.ToListAsync()

            };

            return View(fragranceType);
        }

        public async Task<IActionResult> Male(string brandName, string fName)
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

            if (!string.IsNullOrEmpty(fName))
            {
                fragrances = fragrances.Where(s => s.Name!.Contains(fName));
            }

            if (!string.IsNullOrEmpty(brandName))
            {
                fragrances = fragrances.Where(x => x.Brand == brandName);
            }

            var fragranceType = new FragranceTypeViewModel
            {
                Brand = new SelectList(await brandQuery.Distinct().ToListAsync()),
                FragranceBrand = brandName,
                Fragrances = await fragrances.ToListAsync()

            };

            return View("MaleBrand",fragranceType);
        }
        public async Task<IActionResult> Female(string brandName, string fName)
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

            if (!string.IsNullOrEmpty(fName))
            {
                fragrances = fragrances.Where(s => s.Name!.Contains(fName));
            }

            if (!string.IsNullOrEmpty(brandName))
            {
                fragrances = fragrances.Where(x => x.Brand == brandName);
            }

            var fragranceType = new FragranceTypeViewModel
            {
                Brand = new SelectList(await brandQuery.Distinct().ToListAsync()),
                FragranceBrand = brandName,
                Fragrances = await fragrances.ToListAsync()

            };

            return View("FemaleBrand",fragranceType);
        }
        public ActionResult SortFragrances(string brandName, string sortingOption, string category)
        {

            var sortedFragrances = GetSortedFragrances(brandName, sortingOption, category);
            ViewBag.SortingOption = sortingOption;
            IQueryable<string> brandQuery = from m in _context.Fragrances
                                            orderby m.Brand
                                            select m.Brand;

            var fragranceType = new FragranceTypeViewModel
            {
                Brand = new SelectList(brandQuery.Distinct().ToList()),
                FragranceBrand = brandName,
                Fragrances = sortedFragrances.ToList(),
                SortOption = sortingOption

            };
            return View("MaleBrand", fragranceType);
        }
        public List<Fragrances> GetSortedFragrances(string brandName, string sortingOption,string category)
        {
            // Replace this with your actual data retrieval and sorting logic.
            var fragrances = _context.Fragrances.Where(p=>p.Brand==brandName && p.Category==category).ToList();

            switch (sortingOption)
            {
                case "BestSeller":
                    return fragrances.OrderByDescending(f => f.IsBestSeller).ToList();
                case "PriceHighToLow":
                    return fragrances.OrderByDescending(f => f.Price).ToList();
                case "PriceLowToHigh":
                    return fragrances.OrderBy(f => f.Price).ToList();
                case "NameAToZ":
                    return fragrances.OrderBy(f => f.Name).ToList();
                default:
                    return fragrances;
            }
        }
        public JsonResult Sort(string brandName, string sortingOption, string category)
        {
            var sortedFragrances = GetSortedFragrances(brandName, sortingOption,category);
            return Json(sortedFragrances);
        }

        public async Task<IActionResult> Brand()
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


            var fragranceType = new FragranceTypeViewModel
            {
                Brand = new SelectList(await brandQuery.Distinct().ToListAsync()),
                Fragrances = await fragrances.ToListAsync()

            };
            return View(fragranceType);
        } 
    }
}
