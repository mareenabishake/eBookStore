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

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly AuthService _authService;

    public AdminController(ApplicationDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
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
    public async Task<IActionResult> CreateAdmin([Bind("Username,FirstName,LastName,NICNo,Address,DateOfBirth,Email,ContactNo,Password")] Admin admin)
    {
        if (ModelState.IsValid)
        {
            admin.Role = "Admin";
            _context.Add(admin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageAdmins));
        }
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

                existingAdmin.Username = admin.Username;
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
