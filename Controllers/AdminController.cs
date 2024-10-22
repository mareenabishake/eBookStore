using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eBookStore.Services;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly AuthService _authService;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(ApplicationDbContext context, AuthService authService, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _authService = authService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Dashboard()
    {
        var viewModel = new AdminDashboardViewModel
        {
            TotalBooks = await _context.Books.CountAsync(),
            TotalCustomers = await _context.Customers.CountAsync(),
            TotalOrders = await _context.Orders.CountAsync(),
            RecentBooks = await _context.Books.OrderByDescending(b => b.Id).Take(5).ToListAsync(),
            RecentOrders = await _context.Orders.OrderByDescending(o => o.OrderDate).Take(5).ToListAsync()
        };

        return View(viewModel);
    }

    public IActionResult RegisterAdmin()
    {
        return View();
    }

    // Admins Management
    public async Task<IActionResult> ManageAdmins()
    {
        var admins = await _context.Admins.ToListAsync();
        return View(admins);
    }

    public IActionResult CreateAdmin()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAdmin([Bind("FirstName,LastName,NICNo,Address,DateOfBirth,Email,ContactNo,Password")] Admin admin)
    {
        Console.WriteLine("Admin registration process started");
        Console.WriteLine($"Received admin data: Email={admin.Email}, FirstName={admin.FirstName}, LastName={admin.LastName}");

        ModelState.Remove("UserId");

        if (ModelState.IsValid)
        {
            var user = new IdentityUser 
            { 
                UserName = admin.Email, 
                NormalizedUserName = admin.Email.ToUpper(),
                Email = admin.Email,
                NormalizedEmail = admin.Email.ToUpper(),
                EmailConfirmed = true,
                PhoneNumber = admin.ContactNo,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0
            };

            Console.WriteLine($"Attempting to create IdentityUser with Email: {user.Email}, Phone: {user.PhoneNumber}");

            var result = await _userManager.CreateAsync(user, admin.Password);

            if (result.Succeeded)
            {
                Console.WriteLine("IdentityUser created successfully");
                
                try
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    Console.WriteLine("Added user to Admin role");

                    admin.UserId = user.Id;
                    admin.Role = "Admin";
                    admin.Password = "null";

                    _context.Admins.Add(admin);
                    await _context.SaveChangesAsync();

                    Console.WriteLine("Admin saved successfully");

                    return RedirectToAction(nameof(ManageAdmins));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in admin registration: {ex.Message}");
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

        Console.WriteLine("Admin registration failed. Returning to registration view.");
        return View(admin);
    }

    public async Task<IActionResult> EditAdmin(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var admin = await _context.Admins.FindAsync(id);
        if (admin == null)
        {
            return NotFound();
        }
        return View(admin);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAdmin(int id, [Bind("Id,Username,FirstName,LastName,NICNo,Address,DateOfBirth,Email,ContactNo")] Admin admin)
    {
        if (id != admin.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var existingAdmin = await _context.Admins.FindAsync(id);
                if (existingAdmin == null)
                {
                    return NotFound();
                }

                existingAdmin.FirstName = admin.FirstName;
                existingAdmin.LastName = admin.LastName;
                existingAdmin.NICNo = admin.NICNo;
                existingAdmin.Address = admin.Address;
                existingAdmin.DateOfBirth = admin.DateOfBirth;
                existingAdmin.Email = admin.Email;
                existingAdmin.ContactNo = admin.ContactNo;

                _context.Update(existingAdmin);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminExists(admin.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(ManageAdmins));
        }
        return View(admin);
    }

    public async Task<IActionResult> DeleteAdmin(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var admin = await _context.Admins
            .FirstOrDefaultAsync(m => m.Id == id);
        if (admin == null)
        {
            return NotFound();
        }

        return View(admin);
    }

    [HttpPost, ActionName("DeleteAdmin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAdminConfirmed(int id)
    {
        var admin = await _context.Admins.FindAsync(id);
        _context.Admins.Remove(admin);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ManageAdmins));
    }

    private bool AdminExists(int id)
    {
        return _context.Admins.Any(e => e.Id == id);
    }
}
