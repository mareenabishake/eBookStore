using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace eBookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            Console.WriteLine($"AddToCart called with id: {id}");
            var book = _context.Books.Find(id);
            if (book == null)
            {
                Console.WriteLine("Book not found");
                return NotFound();
            }

            var cart = GetCart();
            Console.WriteLine($"Current cart items: {cart.Count}");
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
                Console.WriteLine("New item added to cart");
            }
            else
            {
                cartItem.Quantity++;
                Console.WriteLine("Existing item quantity increased");
            }

            SaveCart(cart);
            Console.WriteLine($"Cart saved, new item count: {cart.Count}");

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
        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Cart") });
            }

            var cart = GetCart();
            if (!cart.Any())
            {
                return RedirectToAction("Index");
            }

            var order = new Order
            {
                CustomerId = user.Id, // This is now a string
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(item => item.Price * item.Quantity),
                Status = "Pending",
                OrderItems = cart.Select(item => new OrderItem
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Confirmation", new { orderId = order.Id });
        }

        public IActionResult GetCartCount()
        {
            var cart = GetCart();
            return Json(cart.Sum(item => item.Quantity));
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public List<CartItem> GetCart()
        {
            var userId = HttpContext.Session.GetString("UserId") ?? "anonymous";
            var cartJson = HttpContext.Session.GetString($"Cart_{userId}");
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson);
        }

        private void SaveCart(List<CartItem> cart)
        {
            var userId = HttpContext.Session.GetString("UserId") ?? "anonymous";
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString($"Cart_{userId}", cartJson);
        }

        [HttpPost]
        public async Task<IActionResult> CompletePayment(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

            order.PaymentStatus = "Payment Made";
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
