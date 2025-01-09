using Microsoft.AspNetCore.Identity;

namespace LibraryApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        private ICollection<Borrowing> _borrowings;

        public ApplicationUser()
        {
            _borrowings = new List<Borrowing>();
        }


        public virtual ICollection<Borrowing> Borrowings
        {
            get => _borrowings ?? (_borrowings = new List<Borrowing>());
            set => _borrowings = value ?? new List<Borrowing>();
        }

        public ICollection<Review> Reviews { get; set; }
    }
} 