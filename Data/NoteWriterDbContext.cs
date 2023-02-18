using Microsoft.EntityFrameworkCore;

namespace NoteWriter.Data
{
    public class NoteWriterDbContext : DbContext
    {

        public NoteWriterDbContext(DbContextOptions<NoteWriterDbContext> options) : base(options)
        {
        }

        public DbSet<NoteWriter.Models.NoteModel> Notes { get; set; } = default!;
    }
}
