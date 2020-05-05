using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace UserAPIServices.EFModels
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
                optionsBuilder.UseSqlServer("server=34.87.96.112;Database=DBUT;Trusted_Connection=False;user Id=sqlserver;password=123");
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

                entity.Property(e => e.DayJoin).HasColumnName("dayJoin");

                entity.Property(e => e.Role).HasColumnName("role");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Access)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Access_Organization1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Access)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Access_User1");
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.Property(e => e.AdminId).HasColumnName("adminID");

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

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.Property(e => e.OrganizationId).HasColumnName("organizationID");

                entity.Property(e => e.CreatedAt).HasColumnName("createdAt");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

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

                entity.HasOne(d => d.TrackedFunnelData)
                    .WithOne(p => p.StatisticFunnel)
                    .HasForeignKey<StatisticFunnel>(d => d.TrackedFunnelDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StatisticFunnel_TrackedFunnelData");
            });

            modelBuilder.Entity<StatisticHeatmap>(entity =>
            {
                entity.HasKey(e => e.TrackedHeatmapDataId);

                entity.ToTable("statisticHeatmap");

                entity.Property(e => e.TrackedHeatmapDataId)
                    .HasColumnName("trackedHeatmapDataID")
                    .ValueGeneratedNever();

                entity.Property(e => e.StatisticData)
                    .IsRequired()
                    .HasColumnName("statisticData");

                entity.HasOne(d => d.TrackedHeatmapData)
                    .WithOne(p => p.StatisticHeatmap)
                    .HasForeignKey<StatisticHeatmap>(d => d.TrackedHeatmapDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_statisticHeatmap_TrackedHeatmapData");
            });

            modelBuilder.Entity<TrackedFunnelData>(entity =>
            {
                entity.Property(e => e.TrackedFunnelDataId).HasColumnName("trackedFunnelDataID");

                entity.Property(e => e.CreatedAt).HasColumnName("createdAt");

                entity.Property(e => e.SessionId)
                    .IsRequired()
                    .HasColumnName("sessionID");

                entity.Property(e => e.TrackedSteps)
                    .IsRequired()
                    .HasColumnName("trackedSteps");

                entity.Property(e => e.WebId).HasColumnName("webID");

                entity.HasOne(d => d.Web)
                    .WithMany(p => p.TrackedFunnelData)
                    .HasForeignKey(d => d.WebId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrackedFunnelData_Website");
            });

            modelBuilder.Entity<TrackedHeatmapData>(entity =>
            {
                entity.Property(e => e.TrackedHeatmapDataId).HasColumnName("trackedHeatmapDataID");

                entity.Property(e => e.CreatedAt).HasColumnName("createdAt");

                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasColumnName("data");

                entity.Property(e => e.EventType).HasColumnName("eventType");

                entity.Property(e => e.ScreenHeight).HasColumnName("screenHeight");

                entity.Property(e => e.ScreenWidth).HasColumnName("screenWidth");

                entity.Property(e => e.SessionId).HasColumnName("sessionID");

                entity.Property(e => e.TrackingUrl)
                    .IsRequired()
                    .HasColumnName("trackingUrl");

                entity.Property(e => e.WebId).HasColumnName("webID");

                entity.HasOne(d => d.Web)
                    .WithMany(p => p.TrackedHeatmapData)
                    .HasForeignKey(d => d.WebId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrackedHeatmapData_Website");
            });

            modelBuilder.Entity<TrackingFunnelInfo>(entity =>
            {
                entity.Property(e => e.TrackingFunnelInfoId).HasColumnName("trackingFunnelInfoID");

                entity.Property(e => e.AuthorId).HasColumnName("authorID");

                entity.Property(e => e.CreatedAt).HasColumnName("createdAt");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Removed).HasColumnName("removed");

                entity.Property(e => e.Steps)
                    .IsRequired()
                    .HasColumnName("steps");

                entity.Property(e => e.WebId).HasColumnName("webID");

                entity.HasOne(d => d.Web)
                    .WithMany(p => p.TrackingFunnelInfo)
                    .HasForeignKey(d => d.WebId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrackingFunnelInfo_Website");
            });

            modelBuilder.Entity<TrackingHeatmapInfo>(entity =>
            {
                entity.Property(e => e.TrackingHeatmapInfoId).HasColumnName("trackingHeatmapInfoID");

                entity.Property(e => e.AuthorId).HasColumnName("authorID");

                entity.Property(e => e.CreatedAt).HasColumnName("createdAt");

                entity.Property(e => e.EndAt).HasColumnName("endAt");

                entity.Property(e => e.LgImageUrl).HasColumnName("lgImageUrl");

                entity.Property(e => e.MdImageUrl).HasColumnName("mdImageUrl");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Removed).HasColumnName("removed");

                entity.Property(e => e.SmImageUrl).HasColumnName("smImageUrl");

                entity.Property(e => e.Tracking).HasColumnName("tracking");

                entity.Property(e => e.TrackingUrl)
                    .IsRequired()
                    .HasColumnName("trackingUrl");

                entity.Property(e => e.TypeUrl)
                    .HasColumnName("typeUrl")
                    .HasMaxLength(20)
                    .HasDefaultValueSql("('match')");

                entity.Property(e => e.Version).HasColumnName("version");

                entity.Property(e => e.WebId).HasColumnName("webID");

                entity.HasOne(d => d.Web)
                    .WithMany(p => p.TrackingHeatmapInfo)
                    .HasForeignKey(d => d.WebId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrackingHeatmapInfo_Website");
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

                entity.Property(e => e.AuthorId).HasColumnName("authorID");

                entity.Property(e => e.CreatedAt).HasColumnName("createdAt");

                entity.Property(e => e.DomainUrl)
                    .IsRequired()
                    .HasColumnName("domainUrl");

                entity.Property(e => e.OrganizationId).HasColumnName("organizationID");

                entity.Property(e => e.Removed).HasColumnName("removed");

                entity.Property(e => e.Verified).HasColumnName("verified");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Website)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Website_User");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Website)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Website_Organization");
            });
        }
    }
}
