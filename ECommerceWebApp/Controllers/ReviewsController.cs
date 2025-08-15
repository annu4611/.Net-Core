using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceWebApp.Data;
using ECommerceWebApp.Models;
using ECommerceWebApp.ViewModels;

namespace ECommerceWebApp.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please provide a valid rating and review.";
                return RedirectToAction("Details", "Products", new { id = model.ProductId });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            // Check if user has already reviewed this product
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ProductId == model.ProductId && r.UserId == user.Id);

            if (existingReview != null)
            {
                TempData["ErrorMessage"] = "You have already reviewed this product.";
                return RedirectToAction("Details", "Products", new { id = model.ProductId });
            }

            var review = new Review
            {
                ProductId = model.ProductId,
                UserId = user.Id,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your review has been added successfully!";
            return RedirectToAction("Details", "Products", new { id = model.ProductId });
        }

        // POST: Reviews/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            // Check if user owns this review or is admin
            if (review.UserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var productId = review.ProductId;
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Review deleted successfully.";
            return RedirectToAction("Details", "Products", new { id = productId });
        }
    }
}
