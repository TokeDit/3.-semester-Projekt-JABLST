using Microsoft.EntityFrameworkCore;
using Rest_SikkerApi.models;

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
        // DbSet for Image entity

        // Ties the Image model to a database table named "Images"
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Image entity
            modelBuilder.Entity<Image>(entity =>
            {
                // Primary key
                entity.HasKey(i => i.Id);

                // Id as VARCHAR (since it's a string in your model)
                entity.Property(i => i.Id)
                    .IsRequired()
                    .HasMaxLength(50);

                // TimeStamp as VARCHAR
                entity.Property(i => i.TimeStamp)
                    .IsRequired()
                    .HasMaxLength(50);

                // ImageType as VARCHAR
                entity.Property(i => i.ImageType)
                    .IsRequired()
                    .HasMaxLength(50);

                // Store as VARBINARY(MAX) - binary data
                entity.Property(i => i.ImageData)
                    .IsRequired()
                    .HasColumnType("varbinary(max)");

                entity.Property(i => i.Description)
                    .HasMaxLength(500);

                // Confidence: single-precision float -> SQL Server "real"
                entity.Property(i => i.Confidence)
                      .HasColumnType("real")
                      .IsRequired(false)
                      .HasDefaultValue(0f);

                // DetectedObject: textual description of detected object
                entity.Property(i => i.DetectedObject)
                      .HasMaxLength(200)
                      .IsRequired(false);

                // OwnerUid (Firebase UID) - tie image to a user
                // Allow null for legacy images; change IsRequired() if you want it mandatory
                entity.Property(i => i.OwnerUid)
                    .HasMaxLength(128)
                    .IsRequired(false);

                // Optional: create an index to query images by owner efficiently
                entity.HasIndex(i => i.OwnerUid);
                // Optional: create an index on TimeStamp for efficient querying by date
                entity.HasIndex(i => i.TimeStamp);
            });
        }
    }
}
