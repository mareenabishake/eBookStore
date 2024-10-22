using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Data;
using System.Text.Json;

namespace eBookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }

            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(item => item.BookId == id);
            if (cartItem == null)
            {
                cart.Add(new CartItem { 
                    BookId = id, 
                    Title = book.Title, 
                    Price = book.Price, 
                    Quantity = 1,
                    ImageUrl = book.ImageUrl 
                });
            }
            else
            {
                cartItem.Quantity++;
            }

            SaveCart(cart);

            return Json(new { success = true, count = cart.Sum(item => item.Quantity) });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int id, bool increase)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.BookId == id);
            if (item != null)
            {
                if (increase)
                    item.Quantity++;
                else if (item.Quantity > 1)
                    item.Quantity--;
                else
                    cart.Remove(item);

                SaveCart(cart);
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult RemoveItem(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.BookId == id);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
            return Json(new { success = true });
        }

        [Authorize]
        public IActionResult Checkout()
        {
            var cart = GetCart();
            // Implement checkout logic here
            // For now, we'll just return a view
            return View(cart);
        }

        public IActionResult GetCartCount()
        {
            var cart = GetCart();
            return Json(cart.Sum(item => item.Quantity));
        }

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson);
        }

        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("Cart", cartJson);
        }
    }
}
