using Microsoft.EntityFrameworkCore;

namespace Rest_SikkerApi.data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Add your DbSets here (entity models)
        // Example:
        // Table for music records
        // public DbSet<MusicRecord> MusicRecords { get; set; }
        // Table for users
        // public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            //// Configure entity relationships and constraints here
            //modelBuilder.Entity<User>(entity =>
            //{
            //    // Primary key with IDENTITY(1,1)
            //    entity.HasKey(u => u.Id);
            //    entity.Property(u => u.Id)
            //        .ValueGeneratedOnAdd(); // This creates IDENTITY(1,1)

            //    // Name as VARCHAR, NOT NULL
            //    entity.Property(u => u.Name)
            //        .IsRequired() // NOT NULL
            //        .HasColumnType("varchar(100)"); // VARCHAR with max length

            //    // Age as INT
            //    entity.Property(u => u.Age)
            //        .HasColumnType("int");
            //});

        }
    }
}
