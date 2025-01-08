using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

public class AuthorsController : Controller
{
    private readonly LibraryContext _context;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(LibraryContext context, ILogger<AuthorsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Authors
    public async Task<IActionResult> Index()
    {
        var authors = await _context.Authors
            .Include(a => a.Books)
            .AsNoTracking()
            .ToListAsync();

        return View(authors);
    }

    // GET: Authors/Create
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Authors/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([Bind("Name")] Author author)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully created author with ID: {author.AuthorId}");
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating author: {ex}");
            ModelState.AddModelError("", "Unable to save the author. Please try again.");
        }

        return View(author);
    }

    // GET: Authors/Delete/{id}
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var author = await _context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.AuthorId == id);

        if (author == null)
        {
            return NotFound();
        }

        return View(author);
    }

    // POST: Authors/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var author = await _context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.AuthorId == id);

        if (author == null)
        {
            return NotFound();
        }

        try
        {
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Successfully deleted author with ID: {id}");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting author: {ex}");
            ModelState.AddModelError("", "Unable to delete the author. Please ensure all related books are deleted first.");
            return View(author);
        }
    }
} 