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
        //Added DbSet for TelegramMessage entity
        public DbSet<TelegramMessage> TelegramMessages { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Image entity
            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Images");

                // Primary key
                entity.HasKey(i => i.Id);

                // Database uses VARCHAR(50) for Id; keep model int and convert.
                entity.Property(i => i.Id)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasColumnType("varchar(50)");

                // TimeStamp maps to SQL datetime
                entity.Property(i => i.TimeStamp)
                    .IsRequired()
                    .HasColumnType("datetime");

                // ImageType as VARCHAR(50)
                entity.Property(i => i.ImageType)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                // Existing model uses ImageData, but DB column is ImagePath VARCHAR(255)
                entity.Property(i => i.ImageData)
                    .IsRequired()
                    .HasColumnName("ImagePath")
                    .HasColumnType("varchar(255)");

                entity.Property(i => i.Description)
                    .HasColumnType("varchar(500)")
                    .IsRequired(false);

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
                    .HasColumnType("varchar(128)")
                    .IsRequired(false);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(i => i.OwnerUid)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Images_Users_OwnerUid");

                // Optional: create an index to query images by owner efficiently
                entity.HasIndex(i => i.OwnerUid);
                // Optional: create an index on TimeStamp for efficient querying by date
                entity.HasIndex(i => i.TimeStamp);

            });

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(u => u.OwnerUid);
                entity.Property(u => u.OwnerUid)
                    .IsRequired()
                    .HasColumnType("varchar(128)");

                entity.Property(u => u.TelegramChatId)
                    .HasColumnType("varchar(128)")
                    .IsRequired(false);

                entity.Property(u => u.ReportFrequency)
                    .IsRequired()
                    .HasDefaultValue(7);

                entity.Property(u => u.ReportEnabled)
                    .IsRequired()
                    .HasDefaultValue(true);
            });
        }
    }
}
