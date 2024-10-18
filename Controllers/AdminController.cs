using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eBookStore.Services;
using System.IO;

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

    public IActionResult AddBook()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBook([Bind("Title,Author,ISBN,Description,Price,StockQuantity")] Book book)
    {
        if (ModelState.IsValid)
        {
            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }
        return View(book);
    }

    public IActionResult RegisterAdmin()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterAdmin([Bind("Username,FirstName,LastName,NICNo,Address,DateOfBirth,Email,ContactNo,Password")] Admin admin)
    {
        if (ModelState.IsValid)
        {
            admin.Role = "Admin";
            _context.Add(admin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }
        return View(admin);
    }

    public async Task<IActionResult> ManageBooks()
    {
        var books = await _context.Books.ToListAsync();
        return View(books);
    }

    public IActionResult CreateBook()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBook([Bind("Title,Author,ISBN,Description,Price,StockQuantity")] Book book)
    {
        if (ModelState.IsValid)
        {
            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageBooks));
        }
        return View(book);
    }

    public async Task<IActionResult> EditBook(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBook(int id, [Bind("Id,Title,Author,ISBN,Description,Price,StockQuantity,ImageUrl")] Book book, IFormFile image)
    {
        if (id != book.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (image != null && image.Length > 0)
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "books", fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    book.ImageUrl = "/images/books/" + fileName;
                }

                _context.Update(book);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(ManageBooks));
        }
        return View(book);
    }

    public async Task<IActionResult> DeleteBook(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    [HttpPost, ActionName("DeleteBook")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBookConfirmed(int id)
    {
        var book = await _context.Books.FindAsync(id);
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ManageBooks));
    }

    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.Id == id);
    }

    // Customers Management
    public async Task<IActionResult> ManageCustomers()
    {
        var customers = await _context.Customers.ToListAsync();
        return View(customers);
    }

    public IActionResult CreateCustomer()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCustomer([Bind("FirstName,LastName,Email,Password")] Customer customer)
    {
        if (ModelState.IsValid)
        {
            _context.Add(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageCustomers));
        }
        return View(customer);
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
        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCustomer(int id, [Bind("Id,FirstName,LastName,Email")] Customer customer)
    {
        if (id != customer.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(customer);
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
        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ManageCustomers));
    }

    private bool CustomerExists(int id)
    {
        return _context.Customers.Any(e => e.Id == id);
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
    public async Task<IActionResult> CreateAdmin([Bind("Username,Email,Password")] Admin admin)
    {
        if (ModelState.IsValid)
        {
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
    public async Task<IActionResult> EditAdmin(int id, [Bind("Id,Username,Email,Password")] Admin admin)
    {
        if (id != admin.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(admin);
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
