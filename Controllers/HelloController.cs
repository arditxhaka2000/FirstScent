using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirstScent.Data;
using FirstScent.Models;

namespace FirstScent.Controllers
{
    public class HelloController : Controller
    {
        private readonly FirstScentContext _context;
        public HelloController(FirstScentContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            if (_context.Fragrances == null)
            {
                return Problem("Entity set 'MvcMovieContext.Movie'  is null.");
            }
            IQueryable<string> brandQuery = from m in _context.Fragrances
                                            orderby m.Brand
                                            select m.Brand;
            var fragranceCategoryVM = new FragranceTypeViewModel
            {
                Fragrances = await _context.Fragrances.ToListAsync(),
                Brand = new SelectList(await brandQuery.Distinct().ToListAsync())
            };
            return View(fragranceCategoryVM);
        }
        // 
        // GET: /HelloWorld/Welcome/ 
        public string Welcome()
        {
            return "This is the Welcome action method...";
        }
    }
}
