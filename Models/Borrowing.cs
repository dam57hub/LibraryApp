namespace LibraryApp.Models
{
    public class Borrowing
    {
        public int BorrowingId { get; set; }
        public int BookId { get; set; }
        public required Book Book { get; set; }
        public int BorrowerId { get; set; }
        public required Borrower Borrower { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}