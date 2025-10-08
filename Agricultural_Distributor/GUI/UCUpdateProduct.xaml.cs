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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.IO;

namespace Agricultural_Distributor
{
    /// <summary>
    /// Interaction logic for UCUpdateProduct.xaml
    /// </summary>
    public partial class UCUpdateProduct : UserControl
    {
        WDHome wDHome;
        private byte[]? imageBytes;
        private int productId;
        private Product selectedProduct;

        public UCUpdateProduct(WDHome wDHome, int productId)
        {
            InitializeComponent();
            this.productId = productId;
            this.wDHome = wDHome;
            this.selectedProduct = new Product();
            LoadProductInfo();

        }
        public void LoadProductInfo()
        {
            ProductDAO productDAO = new ProductDAO();
            this.selectedProduct = productDAO.GetProductByID(productId);
            if (selectedProduct != null)
            {
                this.DataContext = selectedProduct;

                txtName.Text = selectedProduct.Name;
                txtPurchasePrice.Text = selectedProduct.PurchasePrice.ToString();
                txtSellingPrice.Text = selectedProduct.SellingPrice.ToString();
                txtQualityStandard.Text = selectedProduct.QualityStandard;
                txtMeasurementUnit.Text = selectedProduct.MeasurementUnit;
                txtQuantity.Text = selectedProduct.Quantity.ToString();

                if (selectedProduct.Photo != null)
                {
                    photo.Source = Utils.ConvertBytesToImage(selectedProduct.Photo);
                }
            }
            else
            {
                MessageBox.Show("Không thể lấy thông tin sản phẩm.");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            

            if (!ValidInput(out string error))
            {
                MessageBox.Show(error, "LỖI", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            selectedProduct.Name = txtName.Text;
            selectedProduct.PurchasePrice = Convert.ToDouble(txtPurchasePrice.Text);
            selectedProduct.SellingPrice = Convert.ToDouble(txtSellingPrice.Text);
            selectedProduct.Quantity = Convert.ToInt32(txtQuantity.Text);
            selectedProduct.MeasurementUnit = txtMeasurementUnit.Text;
            selectedProduct.QualityStandard = txtQualityStandard.Text;

            ProductDAO productDAO = new ProductDAO();
            string validateError = productDAO.CheckInfo(selectedProduct);

            if (validateError != null)
            {
                MessageBox.Show(validateError, "LỖI", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                productDAO.UpdateProduct(selectedProduct);
                //MessageBox.Show("CHỈNH SỬA THÀNH CÔNG!!!");

                UCManageProduct uc = new UCManageProduct(wDHome);
                wDHome.GetUC(uc);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message);
            }
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            ProductDAO productDAO = new ProductDAO();
            imageBytes = productDAO.InsertImage(photo);

            selectedProduct.Photo = imageBytes;
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
           // try
            //{
                var result = MessageBox.Show("BẠN CÓ CHẮC CHẮN MUỐN XOÁ SẢN PHẨM NÀY KHÔNG?","Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes) {
                    ProductDAO productDAO = new ProductDAO();
                    productDAO.DeleteProduct(selectedProduct.ProductId);

                    
                    UCManageProduct uc = new UCManageProduct(wDHome);
                    wDHome.GetUC(uc);
                }
                else
                {
                    LoadProductInfo();
                }
                
            //}
            //catch (Exception ex)
            //{
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //}
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            UCManageProduct uc = new UCManageProduct(wDHome);
            wDHome.GetUC(uc);
        }
    }
}
