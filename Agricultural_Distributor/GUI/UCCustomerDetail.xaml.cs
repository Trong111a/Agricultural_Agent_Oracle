using Agricultural_Distributor.DAO;
using Agricultural_Distributor.Entity;
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
using System.Xml.Linq;

namespace Agricultural_Distributor
{
    /// <summary>
    /// Interaction logic for UCCustomerDetail.xaml
    /// </summary>
    public partial class UCCustomerDetail : UserControl
    {
        WDHome wDHome;
        private int customerId;
        private Customer customer;
        public UCCustomerDetail(WDHome wDHome, int customerId)
        {
            InitializeComponent();
            this.wDHome = wDHome;
            this.customerId = customerId;
            this.customer = new Customer();
            LoadCustomerInfo();
        }

        public void LoadCustomerInfo()
        {
            CustomerDAO customerDAO = new CustomerDAO();
            this.customer = customerDAO.GetCustomerByID(customerId);
            if (customer != null)
            {
                this.DataContext = customer;

                txtCustomerName.Text = customer.CustomerName;
                txtCustomerAddress.Text = customer.CustomerAddress;
                txtPhoneNumber.Text = customer.PhoneNumber;
                txtEmail.Text = customer.Email;
            }
            else
            {
                MessageBox.Show("KHÔNG LẤY ĐƯỢC THÔNG TIN KHÁCH HÀNG!!!");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            customer.CustomerName = txtCustomerName.Text;
            customer.CustomerAddress = txtCustomerAddress.Text;
            customer.PhoneNumber = txtPhoneNumber.Text;
            customer.Email = txtEmail.Text;

            CustomerDAO customerDAO = new CustomerDAO();
            string error = customerDAO.ValidInput(customer);
            if (error != null)
            {
                MessageBox.Show(error, "LỖI", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!CheckEmpty(out string validateError)) {
                MessageBox.Show(validateError, "LỖI", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool update = customerDAO.UpdateCustomer(customer);
            if (update)
            {
                MessageBox.Show("CHỈNH SỬA THÔNG TIN KHÁCH HÀNG THÀNH CÔNG!!!");
                UCManageCustomer uc = new UCManageCustomer(wDHome);
                wDHome.GetUC(uc);
            }
            else
            {
                MessageBox.Show("CẬP NHẬT THẤT BẠI!", "LỖI", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CheckEmpty(out string error)
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text) ||
                string.IsNullOrWhiteSpace(txtCustomerAddress.Text) ||
                string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                error = "Vui lòng điền đầy đủ các trường thông tin.";
                return false;
            }
            return true;

        }
    }
}
