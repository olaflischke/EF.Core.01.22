using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WorkPlanDalReverse.Model
{
    public partial class WorkPlanDbContext : DbContext
    {
        public WorkPlanDbContext()
        {
        }

        public WorkPlanDbContext(DbContextOptions<WorkPlanDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Person> Persons { get; set; } = null!;
        public virtual DbSet<Task> Tasks { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=WorkPlanDb;Integrated Security=True;MultipleActiveResultSets=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task>(entity =>
            {
                entity.HasMany(d => d.Workers)
                    .WithMany(p => p.Tasks)
                    .UsingEntity<Dictionary<string, object>>(
                        "PersonTask",
                        l => l.HasOne<Person>().WithMany().HasForeignKey("WorkerId"),
                        r => r.HasOne<Task>().WithMany().HasForeignKey("TasksId"),
                        j =>
                        {
                            j.HasKey("TasksId", "WorkerId");

                            j.ToTable("PersonTask");

                            j.HasIndex(new[] { "WorkerId" }, "IX_PersonTask_WorkerId");
                        });
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
