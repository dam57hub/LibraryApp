public class Book
{
    public int BookId { get; set; }
    public required string Title { get; set; }
    public required string ISBN { get; set; }
    public required ICollection<Author> Authors { get; set; }
    public required ICollection<Borrowing> Borrowings { get; set; }
}