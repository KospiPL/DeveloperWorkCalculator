using D.W.C.Lib.D.W.C.Models;
using Microsoft.EntityFrameworkCore;

namespace D.W.C.API.D.W.C.Service
{
    public class MyDatabaseContext : DbContext
    {
        public MyDatabaseContext(DbContextOptions<MyDatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<WorkItemDetails> WorkItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkItemDetails>(entity =>
            {
                entity.ToTable("WorkItems");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.AssignedTo)
                    .HasMaxLength(255);

                entity.Property(e => e.Status)
                    .HasMaxLength(50);

                entity.Property(e => e.AreaPath)
                    .HasMaxLength(255);

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime");

                entity.Property(e => e.ActivatedDate)
                    .HasColumnType("datetime")
                    .IsRequired(false);

                entity.Property(e => e.LastChangedDate)
                    .HasColumnType("datetime")
                    .IsRequired(false);
            });
        }
    }
}