using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            return book;
        }

        [HttpPost]
        public async Task<ActionResult<Book>> AddBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book book)
        {
            if (id != book.Id) return BadRequest();

            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("title/{title}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByTitle(string title)
        {
            var books = await _context.Books.Where(b => b.Title.ToLower().Contains(title.ToLower())).ToListAsync();
            if (books == null || !books.Any()) return NotFound();
            return books;
        }

        [HttpGet("author/{author}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByAuthor(string author)
        {
            var books = await _context.Books.Where(b => b.Author.ToLower().Contains(author.ToLower())).ToListAsync();
            if (books == null || !books.Any()) return NotFound();
            return books;
        }

        [HttpPost("{id}/lend")]
        public async Task<IActionResult> LendBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null || book.Quantity <= 0) return BadRequest("Not available");

            book.Quantity--;
            await _context.SaveChangesAsync();
            return Ok(book);
        }

        [HttpPost("{id}/return")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            if (book.Quantity >= book.TotalQuantity)
                return BadRequest("All copies have already been returned.");

            book.Quantity++;
            await _context.SaveChangesAsync();
            return Ok(book);
        }

        [HttpGet("genre/{genre}")]
        public ActionResult<IEnumerable<Book>> GetBooksByGenre(string genre)
        {
            var books = _context.Books.Where(b => b.Genre.ToLower() == genre.ToLower()).ToList();

            if (!books.Any())
            {
                return NotFound($"No books found for genre '{genre}'.");
            }

            return Ok(books);
        }


    }
}
