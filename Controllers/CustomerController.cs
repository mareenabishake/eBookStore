using eBookStore.Data;
using eBookStore.Models;
using eBookStore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eBookStore.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;

        public CustomerController(ApplicationDbContext context, AuthService authService)
        {
            _context = context;
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
            if (ModelState.IsValid)
            {
                customer.Password = _authService.HashPassword(customer.Password);
                customer.Role = "Customer"; // Set the role to Customer
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
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
    }
}
