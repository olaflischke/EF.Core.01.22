using Microsoft.EntityFrameworkCore;
using NorthwindDal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NorthwindUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NorthwindContext context = new NorthwindContext();

            context.Log = LogIt;

            context.Database.Migrate();

            var qCountries = context.Customers.Select(cu => cu.Country).Distinct();
            //var qCustomers = context.Customers.ToList();

            //foreach (Customer cu in qCustomers)
            foreach (string country in qCountries)
            {
                TreeViewItem tvi = new TreeViewItem() { Header = country };

                tvi.Items.Add(new TreeViewItem());
                tvi.Expanded += Country_Expanded;

                trvCustomers.Items.Add(tvi);
            }
        }

        private void LogIt(string logString)
        {
            txtLog.Text += logString + Environment.NewLine;
            txtLog.ScrollToEnd();
        }

        private void Country_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem tviCountry)
            {
                tviCountry.Items.Clear();

                NorthwindContext context = new NorthwindContext();
                context.Log = LogIt;

                var qCustomersFromCountry = context.Customers.Where(cu => cu.Country == tviCountry.Header.ToString())
                                                            .Select(cu => new { cu.CustomerId, cu.CompanyName });

                //var qCustomersFromCountry = context.Customers.AsNoTracking().Where(cu => cu.Country == tviCountry.Header.ToString());
                //var qCustomersFromCountry = context.Customers.Where(cu => cu.Country == tviCountry.Header.ToString());

                foreach (var customer in qCustomersFromCountry)
                {
                    TreeViewItem tviCustomer = new TreeViewItem() { Header = customer.CompanyName, Tag = customer.CustomerId };
                    tviCustomer.Selected += TviCountry_Selected;
                    tviCountry.Items.Add(tviCustomer);
                }
            }
        }

        private void TviCountry_Selected(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem tvi && tvi.Tag != null)
            {
                NorthwindContext context = new NorthwindContext();
                context.Log = LogIt;


                var qOrders = context.Orders.Where(od => od.CustomerId == tvi.Tag.ToString()); //.Select(od => od.OrderId);

                cbxOrders.ItemsSource = qOrders.ToList();

            }
        }

        private void cbxOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int orderId = Convert.ToInt32(cbxOrders.SelectedItem);

            NorthwindContext context = new NorthwindContext();
            context.Log = LogIt;


            var qOrderInfo = context.OrderDetails.Where(od => od.OrderId == orderId)
                                                .Select(od => new { od.Quantity, od.Product.ProductName, od.UnitPrice });

            dgOrderInfo.ItemsSource = qOrderInfo.ToList();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            NorthwindContext context = new NorthwindContext();
            Customer customer = new();

            AddEditCustomer addCustomer = new AddEditCustomer(customer);

            if (addCustomer.ShowDialog() == true)
            {
                // Dem ChangeTracker des Contextes den neuen Customer bekanntmachen
                // durch Hinzufügen zum passenden DbSet des DbContextes
                context.Customers.Add(customer); // EntityState = Added
                context.SaveChanges();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (trvCustomers.SelectedItem is TreeViewItem tvi && tvi.Tag != null)
            {
                string customerId = tvi.Tag.ToString();

                using (NorthwindContext context = new NorthwindContext())
                {
                    context.Log = LogIt;

                    Customer customer = context.Customers.Find(customerId);
                    if (customer != null)
                    {
                        AddEditCustomer dlgEditCustomer = new AddEditCustomer(customer);
                        if (dlgEditCustomer.ShowDialog() == true)
                        {
                            try
                            {
                                context.SaveChanges();

                            }
                            catch (DbUpdateConcurrencyException ex)
                            {
                                // Database wins
                                //MessageBox.Show("Daten in der Datenbank neuer als Deine.\n\rIch lade die Daten neu, versuch es nochmal.");
                                //context.Entry(customer).Reload(); // nur, wenn kein lokaler Context

                                // Client wins
                                context.Entry(customer).OriginalValues.SetValues(context.Entry(customer).GetDatabaseValues());
                                context.SaveChanges();

                            }
                            catch (Exception ex) {
                                // Allgemeine Behandlung
                            }
                        }
                        else
                        {
                            // (hier wegen lokalem DbContext nicht notwendig):

                            // Alte Werte aus der Datenbank holen
                            context.Entry(customer).Reload();

                            // Alternative:
                            // Alte Werte aus Speicherwerten wiederherstellen
                            context.Entry(customer).CurrentValues.SetValues(context.Entry(customer).OriginalValues);
                            context.Entry(customer).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                        }
                    }
                }
            }
        }
    }
}
