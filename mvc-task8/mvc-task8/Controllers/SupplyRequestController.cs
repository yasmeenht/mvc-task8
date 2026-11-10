using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_task8.Data;
using mvc_task8.Models;
using System.Security.Claims;

namespace mvc_task8.Controllers
{
    [Authorize]
    public class SupplyRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupplyRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
        }

        private IQueryable<SupplyRequest> Requests()
        {
            var requests = _context.SupplyRequests.Include(r => r.User)
                .Include(r => r.RequestItems).ThenInclude(i => i.Product).AsQueryable();
            if (!IsAdmin())
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                requests = requests.Where(r => r.UserId == userId);
            }
            return requests;
        }

        private SupplyRequest? OwnRequest(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Requests().FirstOrDefault(r => r.Id == id && r.UserId == userId);
        }

        public IActionResult Index()
        {
            return View(Requests().OrderByDescending(r => r.RequestDate).ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            var items = _context.Products.Select(p => new RequestItem
            {
                ProductId = p.Id,
                Product = p
            }).ToList();
            return View(items);
        }

        [HttpPost]
        public IActionResult Create(List<RequestItem> items)
        {
            var requestItems = GetItems(items);
            if (requestItems.Count == 0)
            {
                ModelState.AddModelError("", "Select at least one product with a positive quantity.");
                return View(items);
            }

            var request = new SupplyRequest
            {
                RequestDate = DateTime.Now,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                Status = "Pending",
                RequestItems = requestItems,
                TotalPrice = requestItems.Sum(i => i.Price * i.Quantity)
            };
            _context.SupplyRequests.Add(request);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var request = Requests().FirstOrDefault(r => r.Id == id);
            if (request == null)
            {
                return NotFound();
            }
            return View(request);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var request = OwnRequest(id);
            if (request == null)
            {
                return NotFound();
            }
            if (request.Status != "Pending")
            {
                return Forbid();
            }

            var items = _context.Products.ToList().Select(p => new RequestItem
            {
                ProductId = p.Id,
                Product = p,
                Quantity = request.RequestItems.FirstOrDefault(i => i.ProductId == p.Id)?.Quantity ?? 0
            }).ToList();
            ViewBag.Id = id;
            return View(items);
        }

        [HttpPost]
        public IActionResult Edit(int id, List<RequestItem> items)
        {
            var request = OwnRequest(id);
            if (request == null)
            {
                return NotFound();
            }
            if (request.Status != "Pending")
            {
                return Forbid();
            }

            var requestItems = GetItems(items);
            if (requestItems.Count == 0)
            {
                ModelState.AddModelError("", "Select at least one product with a positive quantity.");
                ViewBag.Id = id;
                return View(items);
            }

            _context.RequestItems.RemoveRange(request.RequestItems);
            request.RequestItems = requestItems;
            request.TotalPrice = requestItems.Sum(i => i.Price * i.Quantity);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private List<RequestItem> GetItems(List<RequestItem> items)
        {
            var products = _context.Products.ToList();
            foreach (var item in items)
            {
                item.Product = products.FirstOrDefault(p => p.Id == item.ProductId)!;
            }
            return items.Where(i => i.Quantity > 0 && i.Product != null)
                .GroupBy(i => i.ProductId).Select(g => new RequestItem
                {
                    ProductId = g.Key,
                    Quantity = g.Sum(i => i.Quantity),
                    Price = g.First().Product.Price
                }).ToList();
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var request = OwnRequest(id);
            if (request == null)
            {
                return NotFound();
            }
            return View(request);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var request = OwnRequest(id);
            if (request == null)
            {
                return NotFound();
            }
            _context.SupplyRequests.Remove(request);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var request = _context.SupplyRequests.Find(id);
            if (request == null)
            {
                return NotFound();
            }
            if (request.Status != "Pending" || (status != "Approved" && status != "Rejected"))
            {
                return BadRequest();
            }
            request.Status = status;
            _context.SaveChanges();
            return RedirectToAction("Details", new { id });
        }
    }
}

