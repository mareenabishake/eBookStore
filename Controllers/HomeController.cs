using eBookStore.Models;
using eBookStore.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Linq;

// This controller handles the home page and general navigation
namespace eBookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Displays the home page with featured books
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.Take(6).ToListAsync(); // Get the first 6 books
            return View(books);
        }

        // GET: Displays the privacy policy page
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Handles book search functionality
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction("Index");
            }

            var books = await _context.Books
                .Where(b => b.Title.Contains(searchTerm) || 
                            b.Author.Contains(searchTerm) || 
                            b.ISBN.Contains(searchTerm))
                .ToListAsync();

            ViewData["SearchTerm"] = searchTerm;
            return View(books);
        }

        // GET: Displays custom error page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
