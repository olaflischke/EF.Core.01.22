using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WorkPlanDal;
using Xunit;
using Xunit.Abstractions;

namespace WorkPlanDalUnitTests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void IsDatabaseCreating()
        {
            WorkPlanContext context = new WorkPlanContext();

            context.Log = LogIt;

            #region Workplan-Data
            Person klaus = new Person() { Name = "Klaus" };
            Person barbara = new Person() { Name = "Barbara" };

            context.Persons.Add(klaus);
            context.Persons.Add(barbara);

            Task aufaeumen = new Task() { Description = "Aufräumen" };
            Task streichen = new Task() { Description = "Küche streichen" };

            aufaeumen.Worker.Add(klaus);
            aufaeumen.Worker.Add(barbara);

            streichen.Worker.Add(barbara);

            klaus.Tasks.Add(aufaeumen);

            barbara.Tasks.Add(aufaeumen);
            barbara.Tasks.Add(streichen);

            context.Tasks.Add(aufaeumen);
            context.Tasks.Add(streichen);

            #endregion

            #region Employee-Data
            context.AddRange(
                            new Employee
                            {
                                Name = "Pinky Pie",
                                Address = "Sugarcube Corner, Ponyville, Equestria",
                                Department = "DevDiv",
                                Position = "Party Organizer",
                                AnnualSalary = 100.0m
                            },
                            new Employee
                            {
                                Name = "Rainbow Dash",
                                Address = "Cloudominium, Ponyville, Equestria",
                                Department = "DevDiv",
                                Position = "Ponyville weather patrol",
                                AnnualSalary = 900.0m
                            },
                            new Employee
                            {
                                Name = "Fluttershy",
                                Address = "Everfree Forest, Equestria",
                                Department = "DevDiv",
                                Position = "Animal caretaker",
                                AnnualSalary = 30.0m
                            });
            #endregion

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.SaveChanges();
        }

        private void LogIt(string obj)
        {
            output.WriteLine(obj);
        }

        [Fact]
        public void ReadEmployees()
        {
            WorkPlanContext context = new WorkPlanContext();

            var employees = context.Employees.ToList();
            foreach (var employee in employees)
            {
                var employeeEntry = context.Entry(employee);
                var validFrom = employeeEntry.Property<DateTime>("ValidFrom").CurrentValue;
                var validTo = employeeEntry.Property<DateTime>("ValidTo").CurrentValue;

                output.WriteLine($"  Employee {employee.Name} valid from {validFrom} to {validTo}");
            }

        }

        [Fact]
        public void ChangeEmployee()
        {
            WorkPlanContext context = new WorkPlanContext();

            Employee employee = context.Employees.Where(e => e.Name =="Rainbow Dash").First();
            employee.AnnualSalary *= 1.5m;

            context.SaveChanges();

            var history = context
                            .Employees
                            .TemporalAll()
                            .Where(e => e.Name == "Rainbow Dash")
                            .OrderBy(e => EF.Property<DateTime>(e, "ValidFrom"))
                            .Select(
                                e => new
                                {
                                    Employee = e,
                                    ValidFrom = EF.Property<DateTime>(e, "ValidFrom"),
                                    ValidTo = EF.Property<DateTime>(e, "ValidTo")
                                })
                            .ToList();

            foreach (var pointInTime in history)
            {
                output.WriteLine($"  Employee {pointInTime.Employee.Name} gets paid '{pointInTime.Employee.AnnualSalary}' from {pointInTime.ValidFrom} to {pointInTime.ValidTo}");
            }
        }
    }
}