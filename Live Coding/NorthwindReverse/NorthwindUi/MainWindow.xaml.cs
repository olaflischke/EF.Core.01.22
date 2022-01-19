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

            var qCountries = context.Customers.Select(cu => cu.Country).Distinct();

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


                var qOrders = context.Orders.Where(od => od.CustomerId == tvi.Tag.ToString()).Select(od => od.OrderId);

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
    }
}
