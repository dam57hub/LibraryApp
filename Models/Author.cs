public class Author
{
    public int AuthorId { get; set; }
    public required string Name { get; set; }
    public required ICollection<Book> Books { get; set; }
}