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

        public virtual DbSet<Access> Access { get; set; }
        public virtual DbSet<Admin> Admin { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<StatisticFunnel> StatisticFunnel { get; set; }
        public virtual DbSet<StatisticHeatmap> StatisticHeatmap { get; set; }
        public virtual DbSet<TrackedFunnelData> TrackedFunnelData { get; set; }
        public virtual DbSet<TrackedHeatmapData> TrackedHeatmapData { get; set; }
        public virtual DbSet<TrackingFunnelInfo> TrackingFunnelInfo { get; set; }
        public virtual DbSet<TrackingHeatmapInfo> TrackingHeatmapInfo { get; set; }
        public virtual DbSet<User> User { get; set; }
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

            modelBuilder.Entity<Access>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.OrganizationId });

                entity.Property(e => e.UserId).HasColumnName("userID");

                entity.Property(e => e.OrganizationId).HasColumnName("organizationID");

                entity.Property(e => e.Role).HasColumnName("role");
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.Property(e => e.AdminId)
                    .HasColumnName("adminID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Actived).HasColumnName("actived");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(50);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("fullName")
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(50);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.Property(e => e.OrganizationId).HasColumnName("organizationID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Removed).HasColumnName("removed");
            });

            modelBuilder.Entity<StatisticFunnel>(entity =>
            {
                entity.HasKey(e => e.TrackedFunnelDataId);

                entity.Property(e => e.TrackedFunnelDataId)
                    .HasColumnName("trackedFunnelDataID")
                    .ValueGeneratedNever();

                entity.Property(e => e.StatisticData)
                    .IsRequired()
                    .HasColumnName("statisticData");
            });

            modelBuilder.Entity<StatisticHeatmap>(entity =>
            {
                entity.HasKey(e => e.TrackedHeatmapDataId);

                entity.Property(e => e.TrackedHeatmapDataId)
                    .HasColumnName("trackedHeatmapDataID")
                    .ValueGeneratedNever();

                entity.Property(e => e.StatisticData)
                    .IsRequired()
                    .HasColumnName("statisticData");
            });

            modelBuilder.Entity<TrackedFunnelData>(entity =>
            {
                entity.Property(e => e.TrackedFunnelDataId)
                    .HasColumnName("trackedFunnelDataID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnName("createdAt");

                entity.Property(e => e.SessionId)
                    .IsRequired()
                    .HasColumnName("sessionID");

                entity.Property(e => e.TrackedSteps)
                    .IsRequired()
                    .HasColumnName("trackedSteps");

                entity.Property(e => e.WebId).HasColumnName("webID");
            });

            modelBuilder.Entity<TrackedHeatmapData>(entity =>
            {
                entity.Property(e => e.TrackedHeatmapDataId).HasColumnName("trackedHeatmapDataID");

                entity.Property(e => e.CreatedAt).HasColumnName("createdAt");

                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasColumnName("data");

                entity.Property(e => e.EventType).HasColumnName("eventType");

                entity.Property(e => e.TrackingUrl)
                    .IsRequired()
                    .HasColumnName("trackingUrl");

                entity.Property(e => e.WebId).HasColumnName("webID");
            });

            modelBuilder.Entity<TrackingFunnelInfo>(entity =>
            {
                entity.Property(e => e.TrackingFunnelInfoId).HasColumnName("trackingFunnelInfoID");

                entity.Property(e => e.CreatedAt).HasColumnName("createdAt");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Removed).HasColumnName("removed");

                entity.Property(e => e.Steps)
                    .IsRequired()
                    .HasColumnName("steps");

                entity.Property(e => e.WebId).HasColumnName("webID");
            });

            modelBuilder.Entity<TrackingHeatmapInfo>(entity =>
            {
                entity.Property(e => e.TrackingHeatmapInfoId).HasColumnName("trackingHeatmapInfoID");

                entity.Property(e => e.CreatedAt).HasColumnName("createdAt");

                entity.Property(e => e.ImageUrl)
                    .HasColumnName("imageUrl")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Removed).HasColumnName("removed");

                entity.Property(e => e.TrackingUrl)
                    .IsRequired()
                    .HasColumnName("trackingUrl");

                entity.Property(e => e.WebId).HasColumnName("webID");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("userID");

                entity.Property(e => e.Actived).HasColumnName("actived");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("fullName");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password");
            });

            modelBuilder.Entity<Website>(entity =>
            {
                entity.HasKey(e => e.WebId);

                entity.Property(e => e.WebId).HasColumnName("webID");

                entity.Property(e => e.DomainUrl)
                    .IsRequired()
                    .HasColumnName("domainUrl")
                    .IsUnicode(false);

                entity.Property(e => e.OrganizationId).HasColumnName("organizationID");

                entity.Property(e => e.Removed).HasColumnName("removed");

                entity.Property(e => e.Verified).HasColumnName("verified");
            });
        }
    }
}
