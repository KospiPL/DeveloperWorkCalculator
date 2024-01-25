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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkItemDetails>(entity =>
            {
                entity.ToTable("WorkItems");
                entity.HasKey(e => e.Id);

                //entity.OwnsOne(e => e.Fields, fieldsBuilder =>
                //{
                //    fieldsBuilder.Property(f => f.AreaPath).HasMaxLength(255);
                //    fieldsBuilder.Property(f => f.TeamProject).HasMaxLength(255);
                //    fieldsBuilder.Property(f => f.IterationPath).HasMaxLength(255);
                //    fieldsBuilder.Property(f => f.WorkItemType).HasMaxLength(50);
                //    fieldsBuilder.Property(f => f.State).HasMaxLength(50);
                //    fieldsBuilder.Property(f => f.Title).IsRequired().HasMaxLength(255);
                //    fieldsBuilder.Property(f => f.BoardColumn).HasMaxLength(50);
                //    fieldsBuilder.Property(f => f.ActivatedDate).HasColumnType("datetime");
                //    fieldsBuilder.Property(f => f.CreatedDate).HasColumnType("datetime");

                //    fieldsBuilder.OwnsOne(f => f.AssignedTo, atBuilder =>
                //    {
                //        atBuilder.Property(a => a.DisplayName).HasColumnName("AssignedToDisplayName").HasMaxLength(255);
                //    });
                //});
            });
        }
    }
}