using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options)
        {
        }

        // Definicje DbSet dla każdej encji
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Borrower> Borrowers { get; set; }
        public DbSet<Borrowing> Borrowings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguracja relacji wiele-do-wielu między Book a Author
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Authors)
                .WithMany(a => a.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookAuthors",
                    j => j.HasOne<Author>().WithMany(),
                    j => j.HasOne<Book>().WithMany());

            // Konfiguracja relacji jeden-do-wielu między Borrowing a Book
            modelBuilder.Entity<Borrowing>()
                .HasOne(b => b.Book)
                .WithMany(bk => bk.Borrowings)
                .HasForeignKey(b => b.BookId);

            // Konfiguracja relacji jeden-do-wielu między Borrowing a Borrower
            modelBuilder.Entity<Borrowing>()
                .HasOne(b => b.Borrower)
                .WithMany(br => br.Borrowings)
                .HasForeignKey(b => b.BorrowerId);
        }
    }
}
