using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using FirstScent.Data;
using FirstScent.Models;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FirstScent.Controllers
{
    public class UsersController : Controller
    {
        private readonly FirstScentContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsersController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, FirstScentContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationViewModel user)
        {
            if (ModelState.IsValid)
            {
                // Check if the user already exists by email
                var existingUser = await _userManager.FindByEmailAsync(user.Email);

                if (existingUser != null)
                {
                    // Email is already in use, display an error message
                    ModelState.AddModelError("SignupErrors", "Email is already in use.");
                    ViewBag.ActiveTab = "SignUp";

                }

                // Create a new ApplicationUser
                var newUser = new ApplicationUser { UserName = user.Email, Email = user.Email, FirstName = user.FirstName, LastName = user.LastName };

                // Create the user using UserManager
                var result = await _userManager.CreateAsync(newUser, user.Password);

                if (result.Succeeded)
                {
                    // Redirect to a success page or login page
                    var r = await _signInManager.PasswordSignInAsync(newUser, user.Password, false, false);
                    await _userManager.AddToRoleAsync(newUser, "Admin");
                    return RedirectToAction("Index", "Hello");
                }
                else
                {
                    // If user creation failed, add errors to ModelState
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("SignupErrors", error.Description);
                        ViewBag.ActiveTab = "SignUp";
                    }
                }
            }

            // If ModelState is not valid or user creation failed, return the registration view
            return View("Login");
        }


        [HttpPost]
        public IActionResult CheckEmailExistence(string email)
        {
            var existingUser = _context.Users.Any(u => u.Email == email);
            if (existingUser == true)
            {
                // Email already exists, return a JSON error response
                return Json(new { success = false, message = "Email is already in use." });
            }

            // Email doesn't exist, return a JSON success response
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckPassword(string email, string password)
        {
            if (password != null && email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

                    if (result.Succeeded)
                    {
                        if (user != null)
                        {
                            var claimResult = await _userManager.AddClaimAsync(user, new Claim("Admin", "True"));

                        }
                        return RedirectToAction("Index", "Hello");

                    }
                    else
                    {
                        ModelState.AddModelError("LoginError", "Invalid password.");

                    }
                }
                else
                {
                    ModelState.AddModelError("LoginError", "Invalid email.");

                }

            }
            return View("Login");



        }
        [HttpGet]
        public async Task<IActionResult> CheckUserCheckout()
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'Users' is null.");
            }

            var userEmail = User.Identity.Name;

            var existingUser = await _userManager.FindByEmailAsync(userEmail);
            List<CartItems> cartList = _context.CartItems
                .Where(item => item.UserId == existingUser.Id)
                .ToList();

            if (existingUser == null)
            {
                return View("CheckOutLogin");

            }
            else
            {
                return View("../Fragrance/CartCheckOut", cartList);

            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckUserCheckoutDirectBuy(string email, string password, int itemId)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'Users' is null.");
            }


            var existingUser = await _userManager.FindByEmailAsync(email);
            //var check = Convert.ToBoolean(_userManager.CheckPasswordAsync(existingUser, password));
            var result = await _signInManager.PasswordSignInAsync(existingUser, password, false, false);

            if (existingUser != null && result.Succeeded)
            {

                List<CartItems> cartList = new List<CartItems>();
                cartList.Add(new CartItems
                {
                    FragranceId = itemId,
                    UserId = existingUser.Id,
                    Price = _context.Fragrances.Find(itemId).Price,
                    PictureUrl = _context.Fragrances.Find(itemId).PictureUrl,
                    Description = _context.Fragrances.Find(itemId).Description,
                    Name = _context.Fragrances.Find(itemId).Name,
                    Quantity = 1,
                });

                return Json(new { success = true, cartList = cartList });
            }
            else
            {
                return View("CheckOutLogin");
            }

        }
        [HttpPost]
        public async Task<IActionResult> BuyNow(int itemId)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'Users' is null.");
            }

            var userEmail = User.Identity.Name;
            List<CartItems> cartList = new List<CartItems>();

            if (userEmail != null)
            {
                var existingUser = await _userManager.FindByEmailAsync(userEmail);
                {
                    cartList.Add(new CartItems
                    {
                        FragranceId = itemId,
                        UserId = existingUser.Id,
                        Price = _context.Fragrances.Find(itemId).Price,
                        PictureUrl = _context.Fragrances.Find(itemId).PictureUrl,
                        Description = _context.Fragrances.Find(itemId).Description,
                        Name = _context.Fragrances.Find(itemId).Name,
                        Quantity = 1,
                    });
                };
                return View("../Fragrance/CartCheckOut", cartList);

            }
            else
            {
                var fragranceType = new FragranceTypeViewModel
                {
                    Fragrances = _context.Fragrances.Where(p=>p.Id==itemId).ToList()
                };
                return View("CheckOutLogin", fragranceType);

            }

        }
        [HttpPost]
        public IActionResult UpdateCartItemQuantity(int itemId, int quantity)
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var cart = _context.CartItems.Where(p => p.UserId == userId && p.FragranceId == itemId).First();

                cart.Quantity = quantity;
                _context.SaveChanges();

            }
            return Json(new { success = true });
        }
        public IActionResult CheckoutLogin()
        {
            return View();
        }

        public IActionResult UserDashboard(string section)
        {
            var userId = _userManager.GetUserId(User);

            if (!string.IsNullOrEmpty(userId))
            {
                var user = _userManager.FindByIdAsync(userId).Result;
                user.Adresses = _context.UserAdresses.Where(p => p.UserId == user.Id).ToList();
                user.Orders = _context.UserOrders.Where(p => p.UserId == user.Id).ToList();
                user.favoriteFragrances = _context.favoriteFragrances.Where(p => p.ApplicationUserId == user.Id).ToList();
                foreach (var order in user.Orders)
                {
                    order.items = _context.OrderItems
                        .Where(oi => oi.OrderId == order.Id)
                        .ToList();
                }
                if (user != null)
                {
                    ViewBag.ActiveSection = section;
                    return View(user);
                }
            }
            return View("Login");
        }
        public ActionResult IndexPagination(int page)
        {

            var userId = _userManager.GetUserId(User); 

            if (!string.IsNullOrEmpty(userId))
            {
                
                var user = _userManager.FindByIdAsync(userId).Result;
                user.Adresses = _context.UserAdresses.Where(p => p.UserId == user.Id).ToList();
                user.Orders = _context.UserOrders.Where(p => p.UserId == user.Id).ToList();
                user.favoriteFragrances = _context.favoriteFragrances.Where(p => p.ApplicationUserId == user.Id).ToList();
                foreach (var order in user.Orders)
                {
                    order.items = _context.OrderItems
                        .Where(oi => oi.OrderId == order.Id).Take(4)
                        .ToList();
                }
                //if (!string.IsNullOrEmpty(search))
                //{
                //    // Modify your query or filter logic based on the search parameter
                //    // For example, if you're filtering by OrderId:
                //    user.Orders = user.Orders.Where(o => o.OrderNumber.Contains(search)).ToList();
                //}
                // Calculate the start index for the current page
                int startIndex = (page - 1) * 2;
                var pageItems = user.Orders.Skip(startIndex).Take(2).ToList();

                return PartialView("_OrderItemsPartial", pageItems);
            }
            return NotFound();

        }

        [HttpPost]
        public async Task<IActionResult> AddAddress(UserAdresses address)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var userId = _userManager.GetUserId(User);
            address.UserId = userId;

            if (ModelState.IsValid)
            {

                // Create a new UserAdresses
                var newAddress = new UserAdresses
                {
                    Name = address.Name,
                    LastName = address.LastName,
                    Country = address.Country,
                    City = address.City,
                    Address = address.Address,
                    Email = address.Email,
                    Phone = address.Phone,
                    UserId = userId
                };

                // Add and save the new address to the database
                _context.UserAdresses.Add(newAddress);
                await _context.SaveChangesAsync();
            }

            // If ModelState is not valid, return to the UserDashboard action with validation errors
            return RedirectToAction("UserDashboard");
        }
        [HttpPost]
        public IActionResult UpdateAddress(UserAdresses updatedAddress)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var userId = _userManager.GetUserId(User);
            updatedAddress.UserId = userId;

            if (ModelState.IsValid)
            {
                // Check if the user already exists by address (excluding the current address being edited)


                // Find the existing address by its ID
                var existingAddressEntry = _context.UserAdresses.Find(updatedAddress.Id);

                if (existingAddressEntry != null)
                {
                    // Update the existing address with the new data
                    existingAddressEntry.Name = updatedAddress.Name;
                    existingAddressEntry.LastName = updatedAddress.LastName;
                    existingAddressEntry.Country = updatedAddress.Country;
                    existingAddressEntry.City = updatedAddress.City;
                    existingAddressEntry.Address = updatedAddress.Address;
                    existingAddressEntry.Email = updatedAddress.Email;
                    existingAddressEntry.Phone = updatedAddress.Phone;

                    // Save the changes to the database
                    _context.SaveChanges();
                }
                else
                {
                    // Handle the case where the address to update is not found
                    return NotFound();
                }
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


            // If ModelState is not valid, return to the UserDashboard action with validation errors
            return RedirectToAction("UserDashboard");
        }

        public IActionResult DeleteAddress(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            // Find the existing address by its ID
            var existingAddressEntry = _context.UserAdresses.Find(id);

            if (existingAddressEntry != null)
            {
                // Optionally, you can check if the address belongs to the currently authenticated user
                var userId = _userManager.GetUserId(User);
                if (existingAddressEntry.UserId != userId)
                {
                    // Handle the case where the address does not belong to the authenticated user
                    return Forbid(); // Or return an appropriate response
                }

                // Remove the address from the database context and save the changes
                _context.UserAdresses.Remove(existingAddressEntry);
                _context.SaveChanges();

                // Optionally, redirect to an appropriate action
                return RedirectToAction("UserDashboard"); // Redirect or return appropriate view
            }
            else
            {
                // Handle the case where the address to delete is not found
                return NotFound();
            }
        }

        public async Task<IActionResult> UpdateUserInfo(string name, string lastName)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var userId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {

                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser != null)
                {
                    // Update the existing address with the new data
                    currentUser.FirstName = name;
                    currentUser.LastName = lastName;


                    var result = await _userManager.UpdateAsync(currentUser);

                    if (!result.Succeeded)
                    {
                        // Handle errors if the update fails
                        foreach (var error in result.Errors)
                        {
                            // Handle each error message
                        }
                    }
                }
                else
                {
                    // Handle the case where the address to update is not found
                    return NotFound();
                }
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


            // If ModelState is not valid, return to the UserDashboard action with validation errors
            return RedirectToAction("UserDashboard");
        }
       
        public IActionResult ProceedPayment(List<int> items, decimal sum)
        {
            var userId = _userManager.GetUserId(User);

            if (!string.IsNullOrEmpty(userId))
            {
                string orderNumber = Guid.NewGuid().ToString("N").Substring(0, 10);

                var userOrder = new UserOrders
                {
                    UserId = userId,
                    Email = _userManager.Users.Where(p => p.Id == userId).First().Email,
                    OrderNumber = orderNumber,
                    OrderDate = DateTime.Now,
                    isPayed = 0,
                    TotalSum = sum
                };

                _context.UserOrders.Add(userOrder);
                _context.SaveChanges();

                int userOrderId = userOrder.Id;

                // Insert OrderItems for each item with the UserOrder Id
                foreach (var item in items)
                {
                    var orderItems = new OrderItems
                    {
                        UserId = userId,
                        PictureUrl = _context.Fragrances.Find(item).PictureUrl,
                        Name = _context.Fragrances.Find(item).Name,
                        OrderId = userOrderId,
                    };

                    // Add the order item to the database
                    _context.OrderItems.Add(orderItems);
                }

                // Save changes to the database
                _context.SaveChanges();
                NotifyOwner(orderNumber, sum, items);
                // After successful insertion, you can delete cart items for the user
                DeleteCartItemsForUser(userId);

                // Redirect to a success page or display a success message
                return RedirectToAction("Index", "Hello");
            }

            return View();
        }
        private async void NotifyOwner(string orderNumber, decimal sum, List<int> items)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var cUserEmail = currentUser.Email;
            var cUserName = currentUser.FirstName;
            var senderEmail = new MailAddress("arditxhaka2000@gmail.com", "Ardit");
            var receiverEmail = new MailAddress(cUserEmail, cUserName);
            var password = "kjix tyfr mmyb bayp";
            var sub = "New Order Notification";
            var body = $@"
        <p>A new order with order number {orderNumber} has been placed. Total: {sum:C}.</p>
        <div>
            <h3>Ordered Items:</h3>
            <ul>";

            // Add each item to the HTML string
            foreach (var itemId in items)
            {
                var fragrance = _context.Fragrances.Find(itemId);

                // Check if the fragrance exists and has a picture URL
                if (fragrance != null && !string.IsNullOrEmpty(fragrance.PictureUrl))
                {
                    body += $@"
                <li>
                    <img src='https://www.chanel.com/images//t_one//w_0.51,h_0.51,c_crop/q_auto:good,f_auto,fl_lossy,dpr_1.1/w_1020/bleu-de-chanel-eau-de-parfum-spray-3-4fl-oz--packshot-default-107360-9536314081310.jpg' alt='{fragrance.Name}' style='max-width: 100px; max-height: 100px;' />
                    {fragrance.Name}
                </li>";
                }
            }

            // Close the HTML string
            body += @"
            </ul>
        </div>";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = sub,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(mess);
            }
        }
        private void DeleteCartItemsForUser(string userId)
        {

            // Find and remove cart items associated with the given userId
            var cartItemsToRemove = _context.CartItems.Where(item => item.UserId == userId).ToList();

            if (cartItemsToRemove.Any())
            {
                _context.CartItems.RemoveRange(cartItemsToRemove);
                _context.SaveChanges();
            }
        }
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var userId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                var cartItemToRemove = _context.CartItems.Find(id);
                if (cartItemToRemove != null)
                {
                    _context.CartItems.Remove(cartItemToRemove);
                    _context.SaveChanges();
                }
                return Json(new { success = true });

            }
            return Json(new { success = false });

        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            // Redirect to a confirmation page or the home page
            return RedirectToAction("Index", "Hello");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                // ChangePasswordAsync changes the user password
                var result = await _userManager.ChangePasswordAsync(user,
                    model.CurrentPassword, model.NewPassword);

                // The new password did not meet the complexity rules or
                // the current password is incorrect. Add these errors to
                // the ModelState and rerender ChangePassword view
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                // Upon successfully changing the password refresh sign-in cookie
                await _signInManager.RefreshSignInAsync(user);
                return View("ChangePasswordConfirmation");
            }

            return View(model);
        }

        public IActionResult Orders()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                var orders = _context.UserOrders.ToList();
                foreach (var order in orders)
                {
                    order.items = _context.OrderItems
                        .Where(oi => oi.OrderId == order.Id)
                        .ToList();
                }
                return View(orders);
            }
            else
            {
                var orders = _context.UserOrders.Where(p => p.UserId == _userManager.GetUserId(User)).ToList();
                foreach (var order in orders)
                {
                    order.items = _context.OrderItems
                        .Where(oi => oi.OrderId == order.Id)
                        .ToList();
                }
                return View(orders);

            }
        }
        public async Task<IActionResult> GuestCheckOut(string email, string name, string lastName, string address,string phone, string country, string city)
        {
            if (ModelState.IsValid)
            {
                
                // Check if the user already exists by email
                var existingUser = await _userManager.FindByEmailAsync(email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("SignupErrors", "Email is already in use.");
                    return Json(new { success = false }) ;

                }
                else
                    return Json(new { success = true });


            }
            return View();
        }
    }
}
