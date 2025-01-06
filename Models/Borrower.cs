namespace LibraryApp.Models
{
    public class Borrower
    {
        public int BorrowerId { get; set; }
        public required string FullName { get; set; }
        public required ICollection<Borrowing> Borrowings { get; set; }
    }
}