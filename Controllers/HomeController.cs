using eBookStore.Models;
using eBookStore.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.Take(6).ToListAsync(); // Get the first 6 books
            return View(books);
        }

        public IActionResult Privacy()
        {
            return View();
        }

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
