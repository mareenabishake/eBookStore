using eBookStore.Data;
using eBookStore.Models;
using eBookStore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Security.Claims;

namespace eBookStore.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AuthService _authService;

        public CustomerController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, AuthService authService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("FirstName,LastName,NICNo,Address,DateOfBirth,Email,ContactNo,Password")] Customer customer)
        {

            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                var user = new IdentityUser 
                { 
                    UserName = customer.Email, 
                    NormalizedUserName = customer.Email.ToUpper(),
                    Email = customer.Email,
                    NormalizedEmail = customer.Email.ToUpper(),
                    EmailConfirmed = true,
                    PhoneNumber = customer.ContactNo,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                };

                var result = await _userManager.CreateAsync(user, customer.Password);

                if (result.Succeeded)
                {
                    
                    try
                    {
                        await _userManager.AddToRoleAsync(user, "Customer");
                        customer.UserId = user.Id;
                        customer.Role = "Customer";
                        customer.Password = "null"; // Do not store the plain text password

                        _context.Customers.Add(customer);
                        await _context.SaveChangesAsync();

                        // Check if the current user is an admin
                        if (!User.IsInRole("Admin"))
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return RedirectToAction("Dashboard", "Customer");
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, "Error during registration process.");
                    }
                }
                else
                {
                    Console.WriteLine("Failed to create IdentityUser. Errors:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"- {error.Description}");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                Console.WriteLine("ModelState is invalid. Errors:");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"- {error.ErrorMessage}");
                    }
                }
            }

            Console.WriteLine("Registration failed. Returning to registration view.");
            return View(customer);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool isValid = await _authService.ValidateCustomerCredentials(model.Email, model.Password);
                if (isValid)
                {
                    // Implement proper authentication here
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View(model);
        }

        // Customer management methods migrated from AdminController
        public async Task<IActionResult> ManageCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            return View(customers);
        }

        public async Task<IActionResult> EditCustomer(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer); // Return the customer model directly
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustomer(int id, [Bind("Id,FirstName,LastName,NICNo,Address,DateOfBirth,Email,ContactNo")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCustomer = await _context.Customers.FindAsync(id);
                    if (existingCustomer == null)
                    {
                        return NotFound();
                    }

                    existingCustomer.FirstName = customer.FirstName;
                    existingCustomer.LastName = customer.LastName;
                    existingCustomer.NICNo = customer.NICNo;
                    existingCustomer.Address = customer.Address;
                    existingCustomer.DateOfBirth = customer.DateOfBirth;
                    existingCustomer.Email = customer.Email;
                    existingCustomer.ContactNo = customer.ContactNo;

                    _context.Update(existingCustomer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ManageCustomers));
            }
            else
            {
                Console.WriteLine("ModelState is invalid. Errors:");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"- {error.ErrorMessage}");
                    }
                }
            }
            return View(customer);
        }

        public async Task<IActionResult> DeleteCustomer(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost, ActionName("DeleteCustomer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCustomerConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(customer.UserId);
            if (user == null)
            {
                return NotFound();
            }

            // Check if the current user is the customer or an admin
            if (User.IsInRole("Admin") || User.FindFirstValue(ClaimTypes.NameIdentifier) == customer.UserId)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    _context.Customers.Remove(customer);
                    await _context.SaveChangesAsync();

                    // If the user deleted their own account, sign them out
                    if (User.FindFirstValue(ClaimTypes.NameIdentifier) == customer.UserId)
                    {
                        await _signInManager.SignOutAsync();
                        return RedirectToAction("Index", "Home");
                    }

                    // If an admin deleted the account, redirect to manage customers
                    return RedirectToAction(nameof(ManageCustomers));
                }
                else
                {
                    // Handle errors
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                // Unauthorized access
                return Forbid();
            }

            // If we get here, something went wrong
            return View("Error");
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (customer == null)
            {
                return NotFound();
            }

            // Debugging: Print customer UserId
            Console.WriteLine($"Customer UserId: {customer.UserId}");

            var allOrders = await _context.Orders.Include(o => o.OrderItems).ToListAsync();
            // Debugging: Print all order IDs and their customer IDs
            foreach (var order in allOrders)
            {
                Console.WriteLine($"Order ID: {order.Id}, Customer ID: {order.CustomerId}");
            }

            var customerOrders = allOrders.Where(o => o.CustomerId == customer.UserId).ToList();
            // Debugging: Print count of customer's orders
            Console.WriteLine($"Customer order count: {customerOrders.Count}");

            var booksPurchased = customerOrders.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity);

            var cart = GetCart(); // Assuming you have a method to get the cart
            var viewModel = new CustomerDashboardViewModel
            {
                CustomerId = customer.UserId,
                TotalOrders = customerOrders.Count,
                BooksPurchased = booksPurchased,
                RecentOrders = customerOrders.OrderByDescending(o => o.OrderDate).Take(5).ToList(),
                RecentBooks = await _context.Books.OrderByDescending(b => b.Id).Take(5).ToListAsync(),
                CartItemCount = cart.Sum(item => item.Quantity)
            };

            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> OrderSummaries()
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (customer == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .Where(o => o.CustomerId == customer.UserId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        private List<CartItem> GetCart()
        {
            var userId = HttpContext.Session.GetString("UserId") ?? "anonymous";
            var cartJson = HttpContext.Session.GetString($"Cart_{userId}");
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson);
        }

        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (customer == null)
            {
                return NotFound();
            }

            var viewModel = new EditProfileViewModel
            {
                Id = customer.Id, // Add this line
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                NICNo = customer.NICNo,
                Address = customer.Address,
                DateOfBirth = customer.DateOfBirth,
                Email = customer.Email,
                ContactNo = customer.ContactNo
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (customer == null)
            {
                return NotFound();
            }

            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.NICNo = model.NICNo;
            customer.Address = model.Address;
            customer.DateOfBirth = model.DateOfBirth;
            customer.Email = model.Email;
            customer.ContactNo = model.ContactNo;

            _context.Update(customer);

            if (!string.IsNullOrEmpty(model.CurrentPassword) && !string.IsNullOrEmpty(model.NewPassword) && !string.IsNullOrEmpty(model.ConfirmNewPassword))
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
