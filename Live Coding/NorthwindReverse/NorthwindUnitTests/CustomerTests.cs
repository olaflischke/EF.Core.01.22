using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NorthwindDal.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
//using System.Transactions;
using Xunit;
using Xunit.Abstractions;

namespace NorthwindUnitTests
{
    public class CustomerTests
    {
        NorthwindContext context;
        string customerId = "ALFKI";

        private readonly ITestOutputHelper output;

        public CustomerTests(ITestOutputHelper output)
        {
            context = new NorthwindContext();

            this.output = output;
        }

        [Fact]
        public void IsCustomerLoading()
        {
            //
            Customer customer = context.Customers.Find(customerId);


            Assert.NotNull(customer);
        }

        [Fact]
        public void HasCustomerOrders()
        {
            // Eager Loading mit Include
            Customer customer = context.Customers.AsNoTracking().Include(od => od.Orders).Where(cu => cu.CustomerId == customerId).FirstOrDefault();

            Order order = customer.Orders.First();

            output.WriteLine($"{order.OrderId}");

            //Assert.Equal(6, customer?.Orders?.Count);
            Assert.Equal(EntityState.Detached, context.Entry(customer).State);
            Assert.Equal(EntityState.Detached, context.Entry(order).State);
        }

        [Fact]
        public void HasOrderCustomer()
        {
            // Pre-Loading der Customer-Objekte
            List<Customer> customers = context.Customers.ToList();

            Order order = context.Orders.Find(11011);

            Assert.NotNull(order.Customer);

            output.WriteLine(order.Customer.CompanyName);
        }

        [Fact]
        public void HowManyCustomers()
        {

            context.ChangeTracker.AutoDetectChangesEnabled = false;

            //LogIt("Load Customers...");

            List<Customer> customers = context.Customers.ToList();

            context.Log = LogIt;

            LogIt("Access local...");

            Stopwatch sw = new Stopwatch();

            sw.Start();

            int count = context.Customers.Local.Count;
            sw.Stop();

            LogIt($"...took {sw.ElapsedMilliseconds}ms.");

            Assert.Equal(91, count);
        }

        private void LogIt(string obj)
        {
            output.WriteLine(obj);
        }

        [Fact]
        public void IsTransactionUsable()
        {
            using IDbContextTransaction transaction = context.Database.BeginTransaction();

            try
            {
                Customer customer = new Customer() { CustomerId = "ABCDE", CompanyName = "Peters Laden", ContactName = "Peter" };
                context.Customers.Add(customer);

                context.SaveChanges();

                // SavePoint für den Rollback
                transaction.CreateSavepoint("CustomerSaved");

                Order order = new Order() { CustomerId = customer.CustomerId };
                context.Orders.Add(order);

                context.SaveChanges();

                transaction.Commit();

            }
            catch (Exception)
            {
                transaction.RollbackToSavepoint("CustomerSaved");
                // TODO: Fehlerbehandlung
            }
        }
    }
}