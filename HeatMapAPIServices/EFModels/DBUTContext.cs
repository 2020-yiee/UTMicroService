using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HeatMapAPIServices.EFModels
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

        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<DataStore> DataStore { get; set; }
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

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CustomerId).HasColumnName("customerId");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("full_name")
                    .HasMaxLength(100);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(50);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasColumnName("role")
                    .HasMaxLength(10);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("user_name")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DataStore>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Data).HasColumnName("data");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.WebId).HasColumnName("webId");

                entity.HasOne(d => d.Web)
                    .WithMany(p => p.DataStore)
                    .HasForeignKey(d => d.WebId)
                    .HasConstraintName("FK_DataStore_Website");
            });

            modelBuilder.Entity<Website>(entity =>
            {
                entity.HasKey(e => e.WebId);

                entity.Property(e => e.WebId)
                    .HasColumnName("webId")
                    .ValueGeneratedNever();

                entity.Property(e => e.CustomerId).HasColumnName("customerId");

                entity.Property(e => e.WebUrl)
                    .IsRequired()
                    .HasColumnName("webUrl")
                    .IsUnicode(false);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Website)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Website_Customer");
            });
        }
    }
}
