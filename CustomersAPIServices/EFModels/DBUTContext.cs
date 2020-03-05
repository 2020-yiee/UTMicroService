using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CustomersAPIServices.EFModels
{
    public partial class DBUTContext : DbContext
    {
        public DBUTContext()
        {
        }

        public DBUTContext(DbContextOptions<DBUTContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TrackedData> TrackedData { get; set; }
        public virtual DbSet<TrackingInfor> TrackingInfor { get; set; }
        public virtual DbSet<WebOwner> WebOwner { get; set; }
        public virtual DbSet<Website> Website { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("server=34.87.174.35;Database=DBUT;Trusted_Connection=False;user Id=sqlserver;password=123");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<TrackedData>(entity =>
            {
                entity.Property(e => e.TrackedDataId).HasColumnName("trackedDataID");

                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasColumnName("data");

                entity.Property(e => e.TrackingId).HasColumnName("trackingID");

                entity.HasOne(d => d.Tracking)
                    .WithMany(p => p.TrackedData)
                    .HasForeignKey(d => d.TrackingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrackedData_TrackingInfor");
            });

            modelBuilder.Entity<TrackingInfor>(entity =>
            {
                entity.HasKey(e => e.TrackingId);

                entity.Property(e => e.TrackingId).HasColumnName("trackingID");

                entity.Property(e => e.IsRemoved).HasColumnName("isRemoved");

                entity.Property(e => e.TrackingType)
                    .IsRequired()
                    .HasColumnName("trackingType")
                    .HasMaxLength(50);

                entity.Property(e => e.TrackingUrl)
                    .IsRequired()
                    .HasColumnName("trackingUrl");

                entity.Property(e => e.WebId).HasColumnName("webID");

                entity.HasOne(d => d.Web)
                    .WithMany(p => p.TrackingInfor)
                    .HasForeignKey(d => d.WebId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrackingInfor_Website");
            });

            modelBuilder.Entity<WebOwner>(entity =>
            {
                entity.Property(e => e.WebOwnerId).HasColumnName("webOwnerID");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("fullName");

                entity.Property(e => e.IsRemoved).HasColumnName("isRemoved");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password");

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasColumnName("role")
                    .HasMaxLength(50);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username");
            });

            modelBuilder.Entity<Website>(entity =>
            {
                entity.HasKey(e => e.WebId);

                entity.Property(e => e.WebId).HasColumnName("webID");

                entity.Property(e => e.IsRemoved).HasColumnName("isRemoved");

                entity.Property(e => e.WebOwnerId).HasColumnName("webOwnerID");

                entity.Property(e => e.WebUrl)
                    .IsRequired()
                    .HasColumnName("webUrl")
                    .IsUnicode(false);

                entity.HasOne(d => d.WebOwner)
                    .WithMany(p => p.Website)
                    .HasForeignKey(d => d.WebOwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Website_Customer");
            });
        }
    }
}
