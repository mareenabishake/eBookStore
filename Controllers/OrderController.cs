using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eBookStore.Data;
using eBookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace eBookStore.Controllers
{
    [Authorize] // Ensure only logged-in users can access these actions
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OrderDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> EditOrder(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Ensure the Customer is loaded
            if (order.Customer == null)
            {
                order.Customer = await _context.Users.FindAsync(order.CustomerId);
            }

            // Populate ViewBag.Customers for the dropdown
            ViewBag.Customers = await _context.Users
                .Select(u => new SelectListItem { Value = u.Id, Text = u.Email })
                .ToListAsync();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrder(int id, [Bind("Id,CustomerId,OrderDate,TotalAmount,Status,PaymentStatus")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingOrder = await _context.Orders
                        .Include(o => o.OrderItems)
                        .FirstOrDefaultAsync(o => o.Id == id);

                    if (existingOrder == null)
                    {
                        return NotFound();
                    }

                    existingOrder.CustomerId = order.CustomerId;
                    existingOrder.OrderDate = order.OrderDate;
                    existingOrder.TotalAmount = order.TotalAmount;
                    existingOrder.Status = order.Status;
                    existingOrder.PaymentStatus = order.PaymentStatus;

                    _context.Update(existingOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Check if the current user is a customer
                var user = await _userManager.GetUserAsync(User);
                if (await _userManager.IsInRoleAsync(user, "Customer"))
                {
                    return RedirectToAction("OrderSummaries", "Customer");
                }
                else
                {
                    return RedirectToAction(nameof(ManageOrders));
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

            // If we got this far, something failed, redisplay form
            ViewBag.Customers = await _context.Users
                .Select(u => new SelectListItem { Value = u.Id, Text = u.Email })
                .ToListAsync();

            return View(order);
        }

        // GET: Displays the order deletion confirmation page
        public async Task<IActionResult> DeleteOrder(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Handles the deletion of an order
        [HttpPost, ActionName("DeleteOrder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // Check if the current user is the owner of the order
            var user = await _userManager.GetUserAsync(User);
            if (order.CustomerId != user.Id && !await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return Forbid();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            // Redirect to the appropriate page based on user role
            if (await _userManager.IsInRoleAsync(user, "Customer"))
            {
                return RedirectToAction("OrderSummaries", "Customer");
            }
            else
            {
                return RedirectToAction(nameof(ManageOrders));
            }
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        public async Task<IActionResult> ManageOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        public IActionResult GenerateOrderReport()
        {
            var orders = _context.Orders.Include(o => o.Customer).ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                document.Add(new Paragraph("Orders Report"));
                document.Add(new Paragraph("\n"));

                PdfPTable table = new PdfPTable(5);
                table.AddCell("Order ID");
                table.AddCell("Customer Email");
                table.AddCell("Order Date");
                table.AddCell("Total Amount");
                table.AddCell("Status");

                foreach (var order in orders)
                {
                    table.AddCell(order.Id.ToString());
                    table.AddCell(order.Customer.Email);
                    table.AddCell(order.OrderDate.ToShortDateString());
                    table.AddCell(order.TotalAmount.ToString("C"));
                    table.AddCell(order.Status);
                }

                document.Add(table);
                document.Close();
                writer.Close();

                return File(ms.ToArray(), "application/pdf", "OrdersReport.pdf");
            }
        }
    }
}
