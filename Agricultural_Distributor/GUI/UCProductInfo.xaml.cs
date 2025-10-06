using Agricultural_Distributor.DAO;
using Agricultural_Distributor.DTO;
using Agricultural_Distributor.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Interaction logic for UCProductInfo.xaml
    /// </summary>
    public partial class UCProductInfo : UserControl
    {
        WDHome wDHome;
        public UCProductInfo( WDHome wDHome)
        {
            InitializeComponent();                      
            this.wDHome = wDHome;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Product currentProduct)
            {
                int productIdToUpdate = currentProduct.ProductId;
                if (productIdToUpdate > 0)
                {
                    UCUpdateProduct updateProductUC = new UCUpdateProduct(wDHome, productIdToUpdate);
                    wDHome.GetUC(updateProductUC);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy ProductId.");
                }
            }
            else if (DataContext is ProductDTO currentProduct2)
            {
                int productIdToUpdate = currentProduct2.ProductId;
                if (productIdToUpdate > 0)
                {
                    UCUpdateProduct updateProductUC = new UCUpdateProduct(wDHome, productIdToUpdate);
                    wDHome.GetUC(updateProductUC);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy ProductId.");
                }
            }
            else
            {

                MessageBox.Show("Không có sản phẩm nào được chọn.");
            }
        }

    }
}
