using Agricultural_Distributor.DAO;
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

namespace Agricultural_Distributor
{
    /// <summary>
    /// Interaction logic for UCManageCustomer.xaml
    /// </summary>
    public partial class UCManageCustomer : UserControl
    {
        WDHome wDHome;
        public UCManageCustomer(WDHome wDHome)
        {
            InitializeComponent();
            this.wDHome = wDHome;
            LoadCustomer();
        }
        private List<Entity.Customer> customer = new();

        private void LoadCustomer()
        {
            CustomerDAO customerDAO = new CustomerDAO();
            customer = customerDAO.LoadCustomer();
            listCustomers.ItemsSource = customer;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                Entity.Customer selectedCustomer = btn.Tag as Entity.Customer;
                if (selectedCustomer != null)
                {
                    UCCustomerDetail uCCustomerDetail = new UCCustomerDetail(wDHome, selectedCustomer.CustomerId);
                    wDHome.GetUC(uCCustomerDetail);
                }
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string keyword = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(keyword))
                {
                    SearchCustomer(keyword);
                }
            }
        }

        private void SearchCustomer(string keyword)
        {
            CustomerDAO customerDAO = new CustomerDAO();
            List<Entity.Customer> list = customerDAO.SearchCustomer(keyword);

            if (list != null && list.Count > 0)
            {
                listCustomers.ItemsSource = list;
            }
            else
            {
                MessageBox.Show("KHÔNG TÌM THẤY KHÁCH HÀNG NÀO");
                UCManageCustomer uc = new UCManageCustomer(wDHome);
                wDHome.GetUC(uc);

            }
        }
        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "Nhập thông tin cần tìm ...")
            {
                txtSearch.Text = "";
                txtSearch.Foreground = Brushes.Black;
            }
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Nhập thông tin cần tìm ...";
                txtSearch.Foreground = Brushes.Gray;
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            UCManageCustomer uc = new UCManageCustomer(wDHome);
            wDHome.GetUC(uc);

        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
