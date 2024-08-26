using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirstScent.Data;
using FirstScent.Models;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace FirstScent.Controllers
{
    public class FragranceController : Controller
    {
        private readonly FirstScentContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly string _imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        public FragranceController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, FirstScentContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: Fragrances
        // GET: Fragrances

        public async Task<IActionResult> Index(string fragranceCategory, string searchString)
        {
            var id = _userManager.GetUserId(User);
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {


                if (_context.Fragrances == null)
                {
                    return Problem("Entity set 'MvcMovieContext.Movie'  is null.");
                }

                // Use LINQ to get list of category.
                IQueryable<string> categoryQuery = from m in _context.Fragrances
                                                   orderby m.Category
                                                   select m.Category;
                var fragrance = from m in _context.Fragrances
                                select m;

                if (!string.IsNullOrEmpty(searchString))
                {
                    fragrance = fragrance.Where(s => s.Name!.Contains(searchString));
                }

                if (!string.IsNullOrEmpty(fragranceCategory))
                {
                    fragrance = fragrance.Where(x => x.Category == fragranceCategory);
                }

                var fragranceCategoryVM = new FragranceTypeViewModel
                {
                    Category = new SelectList(await categoryQuery.Distinct().ToListAsync()),
                    Fragrances = await fragrance.ToListAsync()
                };

                return View(fragranceCategoryVM);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Fragrances == null)
            {
                return NotFound();
            }

            var fragrance = await _context.Fragrances
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fragrance == null)
            {
                return NotFound();
            }

            return View(fragrance);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {

                return View();
            }
            return View("ErrorV");

        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Brand,Category,Price,SizeInMl,Picture,PictureUrl,Description,NewIn,isBestSeller")] Fragrances fragrance)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    if (fragrance.Picture != null && fragrance.Picture.Length > 0)
                    {
                        var fileName = Path.GetFileName(fragrance.Picture.FileName);
                        var filePath = Path.Combine($"wwwroot\\Images\\{fragrance.Brand}", fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await fragrance.Picture.CopyToAsync(fileStream);
                        }

                        fragrance.PictureUrl = $"/Images/{fragrance.Brand}/{fileName}";
                    }
                    _context.Add(fragrance);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(fragrance);
            }
            return RedirectToAction("Index", "Home");

        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                if (id == null || _context.Fragrances == null)
                {
                    return NotFound();
                }

                var fragrance = await _context.Fragrances.FindAsync(id);
                if (fragrance == null)
                {
                    return NotFound();
                }
                return View(fragrance);
            }
            return RedirectToAction("Index", "Home");

        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Brand,Category,Price,PictureUrl,SizeInMl,Description,NewIn,IsBestSeller")] Fragrances fragrance, IFormFile Picture)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                if (id != fragrance.Id)
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (Picture != null)
                        {

                             var fileName = Path.GetFileName(Picture.FileName);
                            var filePath = Path.Combine($"wwwroot\\Images\\{fragrance.Brand}", fileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await Picture.CopyToAsync(fileStream);
                            }

                            fragrance.PictureUrl = $"/Images/{fragrance.Brand}/{fileName}";

                        }

                        _context.Update(fragrance);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!MoviesExists(fragrance.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var key in ModelState.Keys)
                    {
                        foreach (var error in ModelState[key].Errors)
                        {
                            var err = error;
                        }
                    }
                }
                return View(fragrance);
            }
            return RedirectToAction("Index", "Home");

        }


        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                if (id == null || _context.Fragrances == null)
                {
                    return NotFound();
                }

                var movies = await _context.Fragrances
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (movies == null)
                {
                    return NotFound();
                }

                return View(movies);
            }
            return RedirectToAction("Index", "Home");

        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                if (_context.Fragrances == null)
                {
                    return Problem("Entity set 'WebApplication3Context.Movies'  is null.");
                }
                var movies = await _context.Fragrances.FindAsync(id);
                if (movies != null)
                {
                    _context.Fragrances.Remove(movies);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index", "Home");

        }

        private bool MoviesExists(int id)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return (_context.Fragrances?.Any(e => e.Id == id)).GetValueOrDefault();
            }
            return false;

        }

        public async Task<IActionResult> Stock(string fragranceT, string searchString)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                if (_context.Fragrances == null)
                {
                    return Problem("Entity set 'MvcMovieContext.Movie'  is null.");
                }

                // Use LINQ to get list of genres.
                IQueryable<string> genreQuery = from m in _context.Fragrances
                                                orderby m.Category
                                                select m.Category;
                var fragrances = from m in _context.Fragrances
                                 select m;

                if (!string.IsNullOrEmpty(searchString))
                {
                    fragrances = fragrances.Where(s => s.Name!.Contains(searchString));
                }

                if (!string.IsNullOrEmpty(fragranceT))
                {
                    fragrances = fragrances.Where(x => x.Category == fragranceT);
                }

                var fragranceType = new FragranceTypeViewModel
                {
                    Category = new SelectList(await genreQuery.Distinct().ToListAsync()),
                    Fragrances = await fragrances.ToListAsync()
                };

                return View(fragranceType);
            }
            return RedirectToAction("Index", "Home");

        }
        public IActionResult AddToCart(int fragranceId)
        {
            // Check if the user is authenticated (logged in)
            if (!User.Identity.IsAuthenticated)
            {
                // Handle the case where the user is not authenticated, e.g., redirect to login page
                //return RedirectToAction("Login", "Users"); // Modify the action and controller names as needed
                return Json(new { success = false });

            }

            // Retrieve the user's cart items from the database (assuming you have a logged-in user)
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the user's ID
            List<CartItems> cartList = _context.CartItems
                .Where(item => item.UserId == userId)
                .ToList();

            // Check if the selected fragrance is already in the user's cart
            CartItems existingItem = cartList.FirstOrDefault(item => item.FragranceId == fragranceId);
            var fragrance = _context.Fragrances.Where(p => p.Id == fragranceId).First();
            if (existingItem != null)
            {
                // If the fragrance is already in the cart, update its quantity
                existingItem.Quantity++;
            }
            else
            {
                // If the fragrance is not in the cart, add it as a new item
                cartList.Add(new CartItems { UserId = userId, FragranceId = fragranceId, Quantity = 1, Name = fragrance.Name, PictureUrl = fragrance.PictureUrl, Description = fragrance.Description, Price = fragrance.Price });
            }

            // Update the cart items in the database
            foreach (var cartItem in cartList)
            {
                if (cartItem.Id == 0)
                {
                    // If the cart item is new (not in the database), add it
                    _context.CartItems.Add(cartItem);
                }
                else
                {
                    // If the cart item exists in the database, update it
                    _context.CartItems.Update(cartItem);
                }
            }

            // Save changes to the database
            _context.SaveChanges();

            return Json(new { success = true, message = "Fragrance added to cart" });
        }

        public IActionResult CartCheckOut()
        {
            // Check if the user is authenticated (logged in)
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); 
            }

            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the user's ID
            List<CartItems> cartList = _context.CartItems
                .Where(item => item.UserId == userId)
                .ToList();

            // Retrieve fragrance details based on the cart items
            var fragranceIdsInCart = cartList.Select(item => item.FragranceId).ToList();
            var fragrances = _context.Fragrances
                .Where(fragrance => fragranceIdsInCart.Contains(fragrance.Id))
                .ToList();

            // Create a list of CartViewModel instances by joining cart items with fragrance data
            List<CartItems> cartViewModel = (
                from cartItem in cartList
                join fragrance in fragrances on cartItem.FragranceId equals fragrance.Id
                select new CartItems
                {
                    FragranceId = cartItem.FragranceId,
                    Name = fragrance.Name,
                    Price = fragrance.Price,
                    Quantity = cartItem.Quantity,
                    ItemCount = cartItem.ItemCount + 1,
                    PictureUrl = fragrance.PictureUrl,
                    Description = fragrance.Description,
                }
            ).ToList();

            return View(cartViewModel);
        }
        public IActionResult CartCheckOutDirectBuy(string cartList)
        {
            // Check if the user is authenticated (logged in)
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); 
            }
            List<CartItems> cartListObject = JsonConvert.DeserializeObject<List<CartItems>>(cartList);

            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           
        

            return View("CartCheckOut", cartListObject);
        }
        public IActionResult ShoppingCart()
        {
            // Check if the user is authenticated (logged in)
            if (!User.Identity.IsAuthenticated)
            {
                // Handle the case where the user is not authenticated, e.g., redirect to login page
                return RedirectToAction("Login", "Users");
            }

            // Retrieve the user's cart items from the database based on their user ID
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the user's ID
            List<CartItems> cartList = _context.CartItems
                .Where(item => item.UserId == userId)
                .ToList();

            // Retrieve fragrance details based on the cart items
            var fragranceIdsInCart = cartList.Select(item => item.FragranceId).ToList();
            var fragrances = _context.Fragrances
                .Where(fragrance => fragranceIdsInCart.Contains(fragrance.Id))
                .ToList();

            // Create a list of CartViewModel instances by joining cart items with fragrance data
            List<CartItems> cartViewModel = (
                from cartItem in cartList
                join fragrance in fragrances on cartItem.FragranceId equals fragrance.Id
                select new CartItems
                {
                    Id = cartItem.Id,
                    FragranceId = cartItem.FragranceId,
                    Name = fragrance.Name,
                    Price = fragrance.Price,
                    Quantity = cartItem.Quantity,
                    ItemCount = cartItem.ItemCount + 1,
                    PictureUrl = fragrance.PictureUrl,
                    Description = fragrance.Description,
                }
            ).ToList();

            return View(cartViewModel);
        }


        public IActionResult GetCartItemCount()
        {
            if (_context.CartItems == null)
            {
                return Problem("Entity set 'CartItems'  is null.");
            }
            var userId = _userManager.GetUserId(User);


            // Calculate the item count
            int itemCount = _context.CartItems
       .Where(cartItem => cartItem.UserId == userId)
       .Count();
            return Json(itemCount);
        }

        public IActionResult UpdateFavorite(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Handle the case where the user is not authenticated, e.g., redirect to login page
                return RedirectToAction("Login", "Users");
            }

            if (_context.CartItems == null)
            {
                return Problem("Entity set 'CartItems'  is null.");
            }

            var userId = _userManager.GetUserId(User);

            if (!string.IsNullOrEmpty(userId))
            {
                var item = _context.favoriteFragrances.FirstOrDefault(x => x.FragranceId == id);

                if (item == null)
                {
                    var usrFragrance = new UserFavoriteFragrance
                    {
                        ApplicationUserId = userId,
                        FragranceId = id,
                        PictureUrl = _context.Fragrances.Find(id).PictureUrl
                    };

                    // Add and save the new address to the database
                    _context.favoriteFragrances.Add(usrFragrance);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }

            }
            return Json(new { success = false });

        }
        public IActionResult RemoveFromFavorite(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Handle the case where the user is not authenticated, e.g., redirect to login page
                return RedirectToAction("Login", "Users");
            }

            if (_context.CartItems == null)
            {
                return Problem("Entity set 'CartItems'  is null.");
            }

            var userId = _userManager.GetUserId(User);

            if (!string.IsNullOrEmpty(userId))
            {
                var item = _context.favoriteFragrances.FirstOrDefault(x => x.FragranceId == id);

                if (item != null)
                {
                    _context.favoriteFragrances.Remove(item);
                    _context.SaveChanges();

                    return Json(new { success = true });
                }

            }
            return Json(new { success = false });

        }

    }
}
