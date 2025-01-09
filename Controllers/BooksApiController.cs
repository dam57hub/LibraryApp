using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Models;
using System.Text.Json.Serialization;

[Route("api/[controller]")]
[ApiController]
public class BooksApiController : ControllerBase
{
    private readonly LibraryContext _context;

    public BooksApiController(LibraryContext context)
    {
        _context = context;
    }

    // GET: api/BooksApi
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetBooks()
    {
        var books = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Borrowings)
            .AsNoTracking()
            .Select(b => new
            {
                b.BookId,
                b.Title,
                b.ISBN,
                Author = new
                {
                    b.Author.AuthorId,
                    b.Author.Name
                },
                Borrowings = b.Borrowings.Select(br => new
                {
                    br.BorrowingId,
                    br.BorrowDate,
                    br.ReturnDate
                })
            })
            .ToListAsync();

        return Ok(books);
    }

    // GET: api/BooksApi/5
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetBook(int id)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Borrowings)
            .FirstOrDefaultAsync(b => b.BookId == id);

        if (book == null)
        {
            return NotFound();
        }

        var result = new
        {
            book.BookId,
            book.Title,
            book.ISBN,
            Author = new
            {
                book.Author.AuthorId,
                book.Author.Name
            },
            Borrowings = book.Borrowings.Select(b => new
            {
                b.BorrowingId,
                b.BorrowDate,
                b.ReturnDate
            })
        };

        return result;
    }

    // POST: api/BooksApi
    [HttpPost]
    public async Task<ActionResult<Book>> PostBook(Book book)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var authorExists = await _context.Authors.AnyAsync(a => a.AuthorId == book.AuthorId);
        if (!authorExists)
        {
            return BadRequest("Selected author does not exist");
        }

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBook), new { id = book.BookId }, book);
    }

    // PUT: api/BooksApi/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBook(int id, Book book)
    {
        if (id != book.BookId)
        {
            return BadRequest();
        }

        var authorExists = await _context.Authors.AnyAsync(a => a.AuthorId == book.AuthorId);
        if (!authorExists)
        {
            return BadRequest("Selected author does not exist");
        }

        _context.Entry(book).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/BooksApi/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books
            .Include(b => b.Borrowings)
            .FirstOrDefaultAsync(b => b.BookId == id);

        if (book == null)
        {
            return NotFound();
        }

        if (book.Borrowings.Any(b => b.ReturnDate == null))
        {
            return BadRequest("Cannot delete book while it is borrowed.");
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.BookId == id);
    }
} 