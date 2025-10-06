using Agricultural_Distributor.DAO;
using Agricultural_Distributor.Entity;
using Agricultural_Distributor.DTO;
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
using System.Windows.Controls.Primitives;

namespace Agricultural_Distributor.GUI
{
    /// <summary>
    /// Interaction logic for UCSearchProduct.xaml
    /// </summary>
   
    public partial class UCSearchProduct : UserControl
    {
        public event Action<ProductDTO> SearchSubmitted;
        
        List<Product> allProducts = new List<Product>();
        private int currentPage = 0;
        private int pageSize = 4;
        WDHome wDHome;

        private string productId;
        private UCUpdateProduct uc;
        private UCManageProduct parentControl;
        private Product selectedProduct;

        private string sqlSame = "SELECT p.*, w.QUANTITY, w.MEASUREMENTUNIT FROM PRODUCT p JOIN WAREHOUSEINFO w ON p.PRODUCTID = w.PRODUCTID WHERE p.PRODUCTNAME = :Keyword";
        private string sql = "SELECT p.*, w.QUANTITY, w.MEASUREMENTUNIT FROM PRODUCT p JOIN WAREHOUSEINFO w ON p.PRODUCTID = w.PRODUCTID WHERE UPPER(p.PRODUCTNAME) LIKE UPPER(:Keyword)";
        public UCSearchProduct(UCManageProduct parent, UCUpdateProduct uc, WDHome wDHome)
        {
            InitializeComponent();
            this.parentControl = parent;
                  
            LoadProduct();
            this.uc = uc;
            this.wDHome = wDHome;
        }
        private void LoadProduct()
        {
            ProductDAO productDAO = new ProductDAO();
            allProducts = productDAO.GetProductList();
        }
        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "Nhập tên nông sản cần tìm ...")
            {
                txtSearch.Text = "";
                txtSearch.Foreground = Brushes.Black;
            }
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Nhập tên nông sản cần tìm ...";
                txtSearch.Foreground = Brushes.Gray;
            }
        }
        //private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        //{

        //    //if (e.Key == Key.Down)
        //    //{
        //    //    if (lstSuggestions.Items.Count > 0)
        //    //    {
        //    //        lstSuggestions.SelectedIndex = 0;
        //    //        var item = lstSuggestions.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
        //    //        item?.BringIntoView();

        //    //        e.Handled = true;
        //    //    }
        //    //}


        //}
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string keyword = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(keyword))
                {
                    SearchProduct(keyword, false);
                    
                    popupSuggestions.IsOpen = false;
                    //List<Product> list = new ProductDAO().SearchProduct(sql, keyword);
                   
                }
            }
        }


        private void SearchProduct(string keyword, bool exactMatch)
        {
           
            ProductDAO productDAO = new ProductDAO();
            string query = exactMatch ? sqlSame : sql;
            List<Product> list = productDAO.SearchProduct(query, keyword);
            if (list != null && list.Count > 0)
            {
                //var productList = new ProductDAO().SearchProduct(sql, keyword);
                //var productDTOList = ConvertToDTO(productList);
                //parentControl.DisplayProducts(productDTOList);
            }
            else
            {
                MessageBox.Show("KHÔNG TÌM THẤY SẢN PHẨM NÀO");
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearch.Text != "Nhập tên nông sản cần tìm ...")
            {
                string keyword = txtSearch.Text.Trim();

                if (string.IsNullOrEmpty(keyword))
                {
                    popupSuggestions.IsOpen = false;
                    return;
                }
                else
                {
                    ProductDAO productDAO = new ProductDAO();
                    //var suggestions = allProducts.Where(p => p.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0).Select(p => new ProductDTO(p.ProductId, p.Name)).ToList();

                    var suggestions = productDAO.SearchProduct(sql, "%" + keyword + "%").ToList();
                    //MessageBox.Show(suggestions.Count.ToString());
                    if (suggestions.Any())
                    {
                        popupSuggestions.IsOpen = true;
                        lstSuggestions.ItemsSource = suggestions;
                    }
                    else
                    {

                        popupSuggestions.IsOpen = false;
                    }
                }
            }
                           
        }

        //private void lstSuggestions_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter && lstSuggestions.SelectedItem is Product selected)
        //    {
        //        MessageBox.Show(selected.Name + " " + selected.ProductId);
        //        selectedProduct = selected;
        //        popupSuggestions.IsOpen = false;
                
        //        //List<Product> productList = new ProductDAO().SearchProduct(sqlSame, selected.Name);
        //        //for (int i = 0; i < productList.Count; i++)
        //        //{
        //        //    productList[i] = new ProductDAO().GetProductByID(selected.ProductId);
        //        //}
        //    }
        //    else
        //    {
        //        lstSuggestions.Focus();
        //        popupSuggestions.IsOpen = false;
        //    }
        //}
        private void txtSearch_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (popupSuggestions.IsOpen)
            {
                if (e.Key == Key.Enter)
                {
                    //MessageBox.Show(parentControl.ProductGrid.Items.Count.ToString());
                    List<Product> lsPro = new List<Product>();
                    foreach (var suggestion in lstSuggestions.Items)
                    {
                        Product pro = suggestion as Product;
                        lsPro.Add(pro);
                    }

                    List<UCProductInfo> lsUcProInfor = new List<UCProductInfo>(); 
                    foreach(var item in lsPro)
                    {
                        item.Name = item.Name.ToUpper();
                        var uc = new UCProductInfo(wDHome);
                        uc.DataContext = item;
                        lsUcProInfor.Add(uc);
                    }
                    parentControl.AllProducts = lsPro;
                    parentControl.ProductGrid.Items.Clear();
                    foreach (var item in lsUcProInfor)
                    {

                        parentControl.ProductGrid.Items.Add(item);

                    }
                }
            }
        }

        private List<ProductDTO> ConvertToDTO(List<Product> products)
        {
            return products.Select(p => ProductDTO.FromProduct(p)).ToList();
        }
        //private void lstSuggestions_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (lstSuggestions.SelectedItem is Product selected)
        //    {
        //        selectedProduct = selected;

        //        txtSearch.Text = selected.Name;
        //        popupSuggestions.IsOpen = false;

        //        SearchProduct(selected.Name, true);

        //        List<Product>productList = new ProductDAO().SearchProduct(sqlSame, selected.Name);
        //        var productDTOList = ConvertToDTO(productList);
        //        parentControl.DisplayProducts(productDTOList);

        //        //for (int i = 0; i < productList.Count; i++)
        //        //{
        //        //    productList[i] = new ProductDAO().GetProductByID(selected.ProductId);
        //        //}
        //    }
        //}

        private void lstSuggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (lstSuggestions.SelectedItem != null)
            {
                var selectedProduct = lstSuggestions.SelectedItem as Product;
                if (selectedProduct != null)
                {
                    txtSearch.Text = selectedProduct.Name;
                    popupSuggestions.IsOpen = false;

                    List<Product> lsPro = new ProductDAO().SearchProduct(sql, selectedProduct.Name);
                    //foreach (var suggestion in lstSuggestions.Items)
                    //{
                    //    Product pro = suggestion as Product;
                    //    lsPro.Add(pro);
                    //}

                    List<UCProductInfo> lsUcProInfor = new List<UCProductInfo>();
                    foreach (var item in lsPro)
                    {
                        item.Name = item.Name.ToUpper();
                        var uc = new UCProductInfo(wDHome);
                        uc.DataContext = item;
                        lsUcProInfor.Add(uc);
                    }
                    parentControl.AllProducts = lsPro;
                    parentControl.ProductGrid.Items.Clear();
                    foreach (var item in lsUcProInfor)
                    {
                        parentControl.ProductGrid.Items.Add(item);
                    }
                }
                
            }
        }

        private void lstSuggestions_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (lstSuggestions.SelectedItem is Product selected)
            {
                List<Product> lsPro = new ProductDAO().SearchProduct(sql, selected.Name);
                foreach (var suggestion in lstSuggestions.Items)
                {
                    Product pro = suggestion as Product;
                    lsPro.Add(pro);
                }

                List<UCProductInfo> lsUcProInfor = new List<UCProductInfo>();
                foreach (var item in lsPro)
                {
                    item.Name = item.Name.ToUpper();
                    var uc = new UCProductInfo(wDHome);
                    uc.DataContext = item;
                    lsUcProInfor.Add(uc);
                }
                parentControl.AllProducts = lsPro;
                parentControl.ProductGrid.Items.Clear();
                foreach (var item in lsUcProInfor)
                {

                    parentControl.ProductGrid.Items.Add(item);

                }
            }
        }

        private void popupSuggestions_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
