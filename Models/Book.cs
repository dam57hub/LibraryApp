using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryApp.Models
{
    public class Book
    {
        private ICollection<Borrowing> _borrowings;

        public Book()
        {
            _borrowings = new List<Borrowing>();
        }

        public int BookId { get; set; }
        
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "ISBN is required")]
        public string ISBN { get; set; }
        
        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        
        public virtual Author Author { get; set; }

        [InverseProperty("Book")]
        public virtual ICollection<Borrowing> Borrowings 
        { 
            get => _borrowings ?? (_borrowings = new List<Borrowing>());
            set => _borrowings = value ?? new List<Borrowing>();
        }
    }
}