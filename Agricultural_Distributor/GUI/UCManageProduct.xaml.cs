using Agricultural_Distributor.Entity;
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
using Agricultural_Distributor.GUI;
using Agricultural_Distributor.DTO;
using System.Collections.ObjectModel;

namespace Agricultural_Distributor
{
    /// <summary>
    /// Interaction logic for UCManageProduct.xaml
    /// </summary>
    
    public partial class UCManageProduct : UserControl
    {
        
        private int currentPage = 0;
        private int pageSize = 4;
        private List<Product>? allProducts;

        WDHome wDHome;
        private UCUpdateProduct uc;

        internal List<Product>? AllProducts { get => allProducts; set => allProducts = value; }

        public UCManageProduct(WDHome wDHome)
        {
            InitializeComponent();
            this.wDHome = wDHome;
            //LoadSearchContainer();
            LoadProductList();

            var searchBox = new UCSearchProduct(this, uc, wDHome);
            SearchContainer.Children.Clear();
            SearchContainer.Children.Add(searchBox);
        }
        public void LoadProductList()
        {
            ProductDAO productDAO = new ProductDAO();
            AllProducts = productDAO.GetProductList();
            int start = currentPage * pageSize;
            int end = Math.Min(start + pageSize, AllProducts.Count);

            for (int i = start; i < end; i++)
            {
                var product = AllProducts[i];
                product.Name = product.Name.ToUpper();
                var uc = new UCProductInfo(wDHome);
                uc.DataContext = product;

                ProductGrid.Items.Add(uc);
            }
            currentPage++;
        }

        private void ProductScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight - 10)
            {
                if (currentPage * pageSize < AllProducts.Count)
                {
                    LoadProductList();
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            UCAddProduct uCAddProduct = new UCAddProduct(wDHome);
            wDHome.GetUC(uCAddProduct);
        }

        public void DisplayProducts(List<ProductDTO> products)
        {
            ProductGrid.Items.Clear();
            foreach (var product in products)
            {
                product.Name = product.Name.ToUpper(); // Optional

                var uc = new UCProductInfo(wDHome); // hoặc new UCProductInfo(wDHome);
                uc.DataContext = product;

                ProductGrid.Items.Add(uc);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

