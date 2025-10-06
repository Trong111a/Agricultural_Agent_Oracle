using Agricultural_Distributor.Entity;
using Microsoft.Win32;
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
using System.IO;
using Agricultural_Distributor.DAO;
using System.Data.SqlClient;

namespace Agricultural_Distributor
{
    /// <summary>
    /// Interaction logic for UCAddProduct.xaml
    /// </summary>
    public partial class UCAddProduct : UserControl
    {
        WDHome wDHome;
        private byte[]? imageBytes = null;
        public UCAddProduct(WDHome wDHome)
        {
            InitializeComponent();
            this.wDHome = wDHome;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
           
            if (!ValidInput(out string validError))
            {
                MessageBox.Show(validError, "LỖI", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Product product = new Product()
            {
                Name = txtName.Text,
                QualityStandard = txtQualityStandard.Text,
                MeasurementUnit = txtMeasurementUnit.Text,
                Photo = imageBytes,
                PurchasePrice = Convert.ToDouble(txtPurchasePrice.Text),
                SellingPrice = Convert.ToDouble(txtSellingPrice.Text),
                Quantity = int.Parse(txtQuantity.Text),
            };

            ProductDAO productDAO = new ProductDAO();
            string validateError = productDAO.CheckInfo(product);

            if (validateError != null)
            {
                MessageBox.Show(validateError, "LỖI", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                //var result = 
                productDAO.AddProduct(product);
                MessageBox.Show("THÊM NÔNG SẢN THÀNH CÔNG!!!");
                //UCAddProduct uCAddProduct = new UCAddProduct(wDHome);
                //wDHome.GetUC(uCAddProduct);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            ProductDAO productDAO = new ProductDAO();
            imageBytes = productDAO.InsertImage(photo);
        }
        private bool ValidInput(out string error)
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtPurchasePrice.Text) ||
                string.IsNullOrWhiteSpace(txtSellingPrice.Text) ||
                string.IsNullOrWhiteSpace(txtQuantity.Text))
            {
                error = "Vui lòng điền đầy đủ các trường bắt buộc.";
                return false;
            }
            if (!double.TryParse(txtPurchasePrice.Text, out _) ||
                !double.TryParse(txtSellingPrice.Text, out _) ||
                !int.TryParse(txtQuantity.Text, out _))
            {
                error = "Giá mua, giá bán và số lượng phải là số hợp lệ.";
                return false;
            }
            return true;
        }

    }
}
