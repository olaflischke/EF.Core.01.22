using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkPlanDal
{
    public class WorkPlanContext : DbContext
    {
        public Action<string> Log { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=WorkPlanDb;Integrated Security=True;MultipleActiveResultSets=True");
            optionsBuilder.LogTo(t => Log?.Invoke(t));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task>(entity =>
               {
                   // Task-Tabelle soll "Tasks" heißen
                   entity.ToTable("Tasks");
                   // Id-Spalte soll "ProzessId" heißen
                   entity.Property(p => p.Id).HasColumnName("ProcessId");
                   // Alternativ zu [Key]-Annotation
                   //entity.HasKey(p => p.Id);
                   // Zusammengesetzer PK
                   //entity.HasKey(t => new { t.Id, t.Description });

                   //entity.Property(p => p.Id).ValueGeneratedOnAddOrUpdate();
                   //entity.Property(p => p.Description).HasMaxLength(450);
                   //entity.Property<DateTime>("LastSave");
               }
            );

            modelBuilder.Entity<Employee>().ToTable("Employees", e => e.IsTemporal(
                s =>
                {
                    s.HasPeriodStart("ValidFrom");
                    s.HasPeriodEnd("ValidTo");
                    s.UseHistoryTable("EmployeeHistoricalData");
                }));
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Task> Tasks { get; set; }
     
        public DbSet<Person> Persons { get; set; }
    }
}
