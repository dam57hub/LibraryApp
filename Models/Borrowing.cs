namespace LibraryApp.Models
{
    public class Borrowing
    {
        public int BorrowingId { get; set; }
        public int BookId { get; set; }
        public required Book Book { get; set; }
        public string UserId { get; set; }
        public required ApplicationUser User { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}