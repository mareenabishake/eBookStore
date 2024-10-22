using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using Microsoft.EntityFrameworkCore;
using eBookStore.Data;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Linq;

namespace eBookStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.ToListAsync();
            return View(books);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
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

        public async Task<IActionResult> ManageBooks()
        {
            var books = await _context.Books.ToListAsync();
            return View("ManageBooks", books);
        }

        public IActionResult CreateBook()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBook([Bind("Title,Author,ISBN,Description,Price,StockQuantity,Publisher,Genre,PublishedYear")] Book book, IFormFile image)
        {
            Console.WriteLine("Entering CreateBook POST method.");

            if (ModelState.IsValid)
            {
                Console.WriteLine("Model state is valid.");

                if (image != null && image.Length > 0)
                {
                    Console.WriteLine("Image is provided.");
                    var fileName = Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "books", fileName);

                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath)); // Ensure directory exists

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }
                        book.ImageUrl = fileName; // Store only the filename
                        Console.WriteLine("Image uploaded successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error uploading image: {ex.Message}");
                        ModelState.AddModelError("", "There was an error uploading the image. Please try again.");
                        return View(book);
                    }
                }
                else
                {
                    Console.WriteLine("No image provided, using default image.");
                    book.ImageUrl = "default-book.jpg";
                }

                try
                {
                    _context.Add(book);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Book added to database successfully.");
                    return RedirectToAction(nameof(ManageBooks));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving book to database: {ex.Message}");
                    ModelState.AddModelError("", "There was an error saving the book. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("Model state is invalid.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Model error: {error.ErrorMessage}");
                }
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
        public async Task<IActionResult> EditBook(int id, [Bind("Id,Title,Author,ISBN,Description,Price,StockQuantity,ImageUrl,Publisher,Genre,PublishedYear")] Book book, IFormFile image)
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

                        // Ensure the directory exists
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }

                        book.ImageUrl = fileName; // Store only the filename
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
    }
}
