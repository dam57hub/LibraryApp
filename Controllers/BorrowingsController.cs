using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LibraryApp.Controllers
{
    [Authorize]
    public class BorrowingsController : Controller
    {
        private readonly LibraryContext _context;
        private readonly ILogger<BorrowingsController> _logger;

        public BorrowingsController(LibraryContext context, ILogger<BorrowingsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Manage()
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Borrowings)
                    .ThenInclude(b => b.User)
                .AsNoTracking()
                .ToListAsync();

            // Get all non-admin users
            var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var allUsers = await userManager.Users.ToListAsync();
            var nonAdminUsers = new List<ApplicationUser>();
            
            foreach (var user in allUsers)
            {
                if (!await userManager.IsInRoleAsync(user, "Admin"))
                {
                    nonAdminUsers.Add(user);
                }
            }

            ViewBag.Users = nonAdminUsers;
            return View(books);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LendBook(int bookId, string userId, DateTime expectedReturnDate)
        {
            var book = await _context.Books
                .Include(b => b.Borrowings)
                .FirstOrDefaultAsync(b => b.BookId == bookId);

            var user = await _context.Users.FindAsync(userId);

            if (book == null || user == null)
            {
                return NotFound();
            }

            if (book.Borrowings.Any(b => b.ReturnDate == null))
            {
                return BadRequest("Book is already borrowed");
            }

            var borrowing = new Borrowing
            {
                BookId = bookId,
                UserId = userId,
                BorrowDate = DateTime.Now,
                ExpectedReturnDate = expectedReturnDate,
                Book = book,
                User = user
            };

            try
            {
                _context.Borrowings.Add(borrowing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Manage));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error lending book: {ex}");
                return BadRequest("Error lending book");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReturnBook(int bookId)
        {
            var borrowing = await _context.Borrowings
                .FirstOrDefaultAsync(b => b.BookId == bookId && b.ReturnDate == null);

            if (borrowing == null)
            {
                return NotFound();
            }

            try
            {
                borrowing.ReturnDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Manage));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error returning book: {ex}");
                return BadRequest("Error returning book");
            }
        }

        [Authorize]
        public async Task<IActionResult> MyBooks()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Manage");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var myBorrowings = await _context.Borrowings
                .Include(b => b.Book)
                    .ThenInclude(b => b.Author)
                .Where(b => b.UserId == userId && b.ReturnDate == null)
                .OrderBy(b => b.ExpectedReturnDate)
                .AsNoTracking()
                .ToListAsync();

            return View(myBorrowings);
        }
    }
} 