using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECommerceWebApp.Data;
using ECommerceWebApp.Models;
using ECommerceWebApp.ViewModels;

namespace ECommerceWebApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchString, int? categoryId, decimal? minPrice, decimal? maxPrice, int? minRating, int page = 1, int pageSize = 10)
        {
            IQueryable<Product> products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.ProductImages)
                .Include(p => p.Reviews);

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString));
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            if (minRating.HasValue)
            {
                products = products.Where(p => p.Reviews.Any() && p.Reviews.Average(r => r.Rating) >= minRating.Value);
            }

            var totalProducts = await products.CountAsync();
            var productList = await products
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new ProductIndexViewModel
            {
                Products = productList,
                SearchString = searchString,
                CategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MinRating = minRating,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize),
                Categories = await _context.Categories.ToListAsync()
            };

            return View(viewModel);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.ProductImages)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin,Seller")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                
                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    StockQuantity = model.StockQuantity,
                    CategoryId = model.CategoryId,
                    SellerId = user!.Id
                };

                _context.Add(product);
                await _context.SaveChangesAsync();

                // Handle image uploads
                if (model.Images != null && model.Images.Any())
                {
                    await SaveProductImages(product.Id, model.Images);
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
            return View(model);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Check if user is authorized to edit this product
            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && product.SellerId != user!.Id)
            {
                return Forbid();
            }

            var viewModel = new EditProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                ExistingImages = product.ProductImages.ToList()
            };

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(viewModel);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> Edit(int id, EditProductViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _context.Products.FindAsync(id);
                    if (product == null)
                    {
                        return NotFound();
                    }

                    // Check if user is authorized to edit this product
                    var user = await _userManager.GetUserAsync(User);
                    if (!User.IsInRole("Admin") && product.SellerId != user!.Id)
                    {
                        return Forbid();
                    }

                    product.Name = model.Name;
                    product.Description = model.Description;
                    product.Price = model.Price;
                    product.StockQuantity = model.StockQuantity;
                    product.CategoryId = model.CategoryId;
                    product.UpdatedAt = DateTime.UtcNow;

                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    // Handle new image uploads
                    if (model.Images != null && model.Images.Any())
                    {
                        await SaveProductImages(product.Id, model.Images);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", model.CategoryId);
            return View(model);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Check if user is authorized to delete this product
            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && product.SellerId != user!.Id)
            {
                return Forbid();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product != null)
            {
                // Check if user is authorized to delete this product
                var user = await _userManager.GetUserAsync(User);
                if (!User.IsInRole("Admin") && product.SellerId != user!.Id)
                {
                    return Forbid();
                }

                // Delete related product images first
                if (product.ProductImages.Any())
                {
                    _context.ProductImages.RemoveRange(product.ProductImages);
                }

                // Delete related reviews
                if (product.Reviews.Any())
                {
                    _context.Reviews.RemoveRange(product.Reviews);
                }

                // Delete the product
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        private async Task SaveProductImages(int productId, IFormFileCollection images)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
            Directory.CreateDirectory(uploadsFolder);

            foreach (var image in images)
            {
                if (image.Length > 0)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    var productImage = new ProductImage
                    {
                        ProductId = productId,
                        ImageUrl = "/images/products/" + uniqueFileName,
                        AltText = $"Image for product {productId}"
                    };

                    _context.ProductImages.Add(productImage);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
