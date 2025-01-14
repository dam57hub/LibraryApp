using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

public class BooksController : Controller
{
    private readonly LibraryContext _context;
    private readonly ILogger<BooksController> _logger;

    public BooksController(LibraryContext context, ILogger<BooksController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Books
    public async Task<IActionResult> Index()
    {
        var books = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Borrowings)
            .AsNoTracking()
            .ToListAsync();

        return View(books);
    }

    // GET: Books/Create
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Authors = await _context.Authors.ToListAsync();
        return View();
    }

    // POST: Books/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([Bind("Title,ISBN,AuthorId")] Book book)
    {
        foreach (var modelState in ModelState)
        {
            _logger.LogInformation($"Key: {modelState.Key}, Value: {modelState.Value?.AttemptedValue}");
            if (modelState.Value?.Errors.Count > 0)
            {
                foreach (var error in modelState.Value.Errors)
                {
                    _logger.LogInformation($"Error for {modelState.Key}: {error.ErrorMessage}");
                }
            }
        }

        _logger.LogInformation($"Received book data - Title: {book.Title}, ISBN: {book.ISBN}, AuthorId: {book.AuthorId}");

        ModelState.Remove("Author");
        
        try
        {
            if (ModelState.IsValid)
            {
                var authorExists = await _context.Authors.AnyAsync(a => a.AuthorId == book.AuthorId);
                if (!authorExists)
                {
                    ModelState.AddModelError("AuthorId", "Selected author does not exist");
                    _logger.LogWarning($"Invalid AuthorId: {book.AuthorId}");
                }
                else
                {
                    _context.Books.Add(book);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Successfully created book with ID: {book.BookId}");
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                _logger.LogWarning($"Invalid ModelState: {errors}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating book: {ex}");
            ModelState.AddModelError("", "Unable to save the book. Please try again.");
        }

        ViewBag.Authors = await _context.Authors.ToListAsync();
        return View(book);
    }

    // GET: Books/Edit/{id}
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.BookId == id);

        if (book == null)
        {
            return NotFound();
        }

        ViewBag.Authors = await _context.Authors.ToListAsync();
        return View(book);
    }

    // POST: Books/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, [Bind("BookId,Title,ISBN,AuthorId")] Book book)
    {
        if (id != book.BookId)
        {
            return NotFound();
        }

        ModelState.Remove("Author");

        try
        {
            if (ModelState.IsValid)
            {
                var authorExists = await _context.Authors.AnyAsync(a => a.AuthorId == book.AuthorId);
                if (!authorExists)
                {
                    ModelState.AddModelError("AuthorId", "Selected author does not exist");
                    _logger.LogWarning($"Invalid AuthorId: {book.AuthorId}");
                }
                else
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Successfully updated book with ID: {book.BookId}");
                    return RedirectToAction(nameof(Index));
                }
            }
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (!await BookExists(book.BookId))
            {
                return NotFound();
            }
            else
            {
                _logger.LogError($"Error updating book: {ex}");
                ModelState.AddModelError("", "Unable to save changes. Try again.");
            }
        }

        ViewBag.Authors = await _context.Authors.ToListAsync();
        return View(book);
    }

    // GET: Books/Delete/{id}
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Borrowings)
            .FirstOrDefaultAsync(b => b.BookId == id);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // POST: Books/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        try
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Successfully deleted book with ID: {id}");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting book: {ex}");
            ModelState.AddModelError("", "Unable to delete the book. Please try again.");
            return View(book);
        }
    }

    private async Task<bool> BookExists(int id)
    {
        return await _context.Books.AnyAsync(e => e.BookId == id);
    }
}
