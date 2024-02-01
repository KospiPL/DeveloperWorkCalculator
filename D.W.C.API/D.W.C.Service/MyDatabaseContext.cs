using D.W.C.Lib.D.W.C.Models;
using DevWorkCalc.D.W.C.Models;
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
        public DbSet<WorkItemHistoryList> WorkItemHistories { get; set; }
        public DbSet<Iteration> Iterations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkItemDetails>(entity =>
            {
                entity.ToTable("WorkItems");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).IsRequired();
                entity.Property(e => e.Rev);

                entity.OwnsOne(e => e.Fields, fieldsBuilder =>
                {
                    fieldsBuilder.Property(f => f.AreaPath).HasMaxLength(255);
                    fieldsBuilder.Property(f => f.TeamProject).HasMaxLength(255);
                    fieldsBuilder.Property(f => f.IterationPath).HasMaxLength(255);
                    fieldsBuilder.Property(f => f.WorkItemType).HasMaxLength(50);
                    fieldsBuilder.Property(f => f.State).HasMaxLength(50);
                    fieldsBuilder.Property(f => f.Title).IsRequired().HasMaxLength(255);
                    fieldsBuilder.Property(f => f.BoardColumn).HasMaxLength(50);
                    fieldsBuilder.Property(f => f.CreatedDate).HasColumnType("datetime");
                    fieldsBuilder.Property(f => f.ActivatedDate).HasColumnType("datetime");
                    fieldsBuilder.Property(f => f.ResolvedDate).HasColumnType("datetime");

                    fieldsBuilder.OwnsOne(f => f.AssignedTo, atBuilder =>
                    {
                        atBuilder.Property(a => a.DisplayName).HasColumnName("AssignedToDisplayName").HasMaxLength(255);
                    });
                });
            });
            modelBuilder.Entity<WorkItemHistoryList>().HasNoKey();
            modelBuilder.Ignore<BoardColumn>();
            modelBuilder.Entity<WorkItemHistory>()
             .ToTable("ITEM_HIS")
             .HasKey(w => new { w.Id, w.Rev });

            modelBuilder.Entity<WorkItemHistory>()
                .Property(w => w.Fields.System_ChangedDate.OldValue)
                .HasColumnName("System_ChangedDate_OldValue");

            modelBuilder.Entity<WorkItemHistory>()
                .Property(w => w.Fields.System_ChangedDate.NewValue)
                .HasColumnName("System_ChangedDate_NewValue");

            modelBuilder.Entity<WorkItemHistory>()
                .Property(w => w.Fields.System_BoardColumn.OldValue)
                .HasColumnName("System_BoardColumn_OldValue");

            modelBuilder.Entity<WorkItemHistory>()
                .Property(w => w.Fields.System_BoardColumn.NewValue)
                .HasColumnName("System_BoardColumn_NewValue");
            modelBuilder.Entity<Iteration>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).IsRequired();
                entity.Property(e => e.Name).HasMaxLength(255);
                entity.Property(e => e.Path).HasMaxLength(255);
                entity.OwnsOne(e => e.Attributes, a =>
                {
                    a.Property(p => p.StartDate).HasColumnName("StartDate").HasColumnType("datetime");
                    a.Property(p => p.FinishDate).HasColumnName("FinishDate").HasColumnType("datetime");
                });
            });
        }
    }
}