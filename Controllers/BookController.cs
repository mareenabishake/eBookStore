using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using Microsoft.EntityFrameworkCore;
using eBookStore.Data;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using iTextSharp.text;
using iTextSharp.text.pdf;

// This controller handles all book-related operations
// Including listing, details, creation, editing, and management of books
namespace eBookStore.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BookController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Retrieves all books and displays them
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.ToListAsync();
            return View(books);
        }

        // GET: Displays details of a specific book, including comments
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.BookComments)
                .ThenInclude(bc => bc.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Retrieves all books for management purposes
        public async Task<IActionResult> ManageBooks()
        {
            var books = await _context.Books.ToListAsync();
            return View("ManageBooks", books);
        }

        // GET: Displays the form to create a new book
        public IActionResult CreateBook()
        {
            return View();
        }

        // POST: Handles the creation of a new book, including image upload
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

        // GET: Displays the form to edit an existing book
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

        // POST: Handles the editing of an existing book, including image update
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

        // GET: Retrieves and displays books purchased by the current user
        [Authorize]
        public async Task<IActionResult> PurchasedBooks()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Console.WriteLine($"User ID: {user.Id}");

            var purchasedBooks = await _context.OrderItems
                .Where(oi => oi.Order.CustomerId == user.Id)
                .GroupBy(oi => oi.BookId)
                .Select(g => new PurchasedBookViewModel
                {
                    Book = g.First().Book,
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .ToListAsync();

            Console.WriteLine($"Number of purchased books: {purchasedBooks.Count}");

            return View(purchasedBooks);
        }

        // GET: Displays the form to add a comment to a book
        [Authorize]
        public async Task<IActionResult> Comment(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var viewModel = new BookCommentViewModel
            {
                BookId = book.Id,
                BookTitle = book.Title
            };

            return View(viewModel);
        }

        // POST: Handles the addition of a comment to a book
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Comment(BookCommentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var comment = new BookComment
                {
                    BookId = model.BookId,
                    UserId = user.Id,
                    CommentText = model.CommentText,
                    CommentDate = DateTime.Now
                };

                _context.BookComments.Add(comment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(PurchasedBooks));
            }

            return View(model);
        }

        // GET: Generates a PDF report of all books
        public IActionResult GenerateBookReport()
        {
            var books = _context.Books.ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                document.Add(new Paragraph("Books Report"));
                document.Add(new Paragraph("\n"));

                PdfPTable table = new PdfPTable(6);
                table.AddCell("Title");
                table.AddCell("Author");
                table.AddCell("ISBN");
                table.AddCell("Price");
                table.AddCell("Stock");
                table.AddCell("Genre");

                foreach (var book in books)
                {
                    table.AddCell(book.Title);
                    table.AddCell(book.Author);
                    table.AddCell(book.ISBN);
                    table.AddCell(book.Price.ToString("C"));
                    table.AddCell(book.StockQuantity.ToString());
                    table.AddCell(book.Genre);
                }

                document.Add(table);
                document.Close();
                writer.Close();

                return File(ms.ToArray(), "application/pdf", "BooksReport.pdf");
            }
        }
    }
}
