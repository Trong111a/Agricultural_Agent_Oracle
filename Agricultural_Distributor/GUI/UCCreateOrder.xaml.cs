using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using Agricultural_Distributor.DAO;
using Agricultural_Distributor.Entity;
using Agricultural_Distributor.GUI;

namespace Agricultural_Distributor
{
    /// <summary>
    /// Interaction logic for UCCreateOrder.xaml
    /// </summary>
    public partial class UCCreateOrder : UserControl
    {
        internal string result = "";

        WDHome wDHome;
        
        public UCCreateOrder(WDHome wDHome)
        {
            InitializeComponent();
            this.wDHome = wDHome;
            result = "Bán";
            LoadProducts();
        }

        private List<Entity.Product> products = new();

        internal List<Entity.Product> productSelect = new();

        internal string discount;

        internal string note;

        private void LoadProducts()
        {
            ProductDAO productDAO = new ProductDAO();
            products = productDAO.LoadProduct();

            lvProducts.ItemsSource = products;
        }

        private bool GetOrder(object sender, RoutedEventArgs e)
        {
            productSelect.Clear();

            foreach (var item in lvProducts.Items)
            {
                if (item is Entity.Product product && product.IsSelected)
                {
                    var container = lvProducts.ItemContainerGenerator.ContainerFromItem(product) as ListViewItem;
                    var txtBox = FindTextBoxInContainer(container);

                    if (txtBox != null && txtBox.IsEnabled)
                    {
                        try
                        {
                            product.Quantity = Convert.ToInt32(txtBox.Text);
                            productSelect.Add(product);
                        }
                        catch
                        {
                            return false;
                        }
                        
                    }
                }
            }

            foreach (var item in lvProducts.Items)
            {
                if (item is Entity.Product product && product.IsSelected == false)
                {
                    var container = lvProducts.ItemContainerGenerator.ContainerFromItem(product) as ListViewItem;
                    var txtBox = FindTextBoxInContainer(container);

                    if (txtBox != null && txtBox.IsEnabled == false)
                    {
                        if (productSelect.Contains(product))
                        {
                            productSelect.Remove(product);
                        }
                    }
                }
            }

            if (tbDiscount.Text != null) discount = tbDiscount.Text;
            else discount = "";
            if (tbNote.Text != null) note = tbNote.Text;
            else note = "";

            return true;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb?.DataContext is Entity.Product product)
            {
                if (product.Quantity >= 1)
                {
                    product.IsSelected = true;

                    var container = lvProducts.ItemContainerGenerator.ContainerFromItem(product) as ListViewItem;
                    var txtBox = FindTextBoxInContainer(container);
                    if (txtBox != null)
                    {
                        txtBox.IsEnabled = true;
                    }

                    UpdateTotalPrice();

                    productSelect.Add(product);
                }
                else
                {
                    cb.Dispatcher.BeginInvoke(new Action(() => cb.IsChecked = false), System.Windows.Threading.DispatcherPriority.Background);
                    MessageBox.Show("Không còn hàng", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb?.DataContext is Entity.Product product)
            {
                product.IsSelected = false;
                product.QuantitySelect = 0;

                var container = lvProducts.ItemContainerGenerator.ContainerFromItem(product) as ListViewItem;
                var txtBox = FindTextBoxInContainer(container);
                if (txtBox != null)
                {
                    txtBox.Text = "";
                    txtBox.IsEnabled = false;
                }

                UpdateTotalPrice();

                productSelect.Remove(product);
            }
        }

        private void UpdateTotalPrice()
        {
            double total = 0;
            foreach (Product product in products)
            {
                if (product.IsSelected == true)
                {
                    ProductDAO productDAO = new ProductDAO(product);
                    total += productDAO.Total;
                }
            }
            txtTotalPrice.Text = $"Tổng tiền: {total:N0} đ";
        }

        private TextBox FindTextBoxInContainer(DependencyObject container)
        {
            if (container == null) return null;

            var textBoxes = FindVisualChildren<TextBox>(container);
            return textBoxes.FirstOrDefault();
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    var child = VisualTreeHelper.GetChild(depObj, i);
                    if (child is T t)
                        yield return t;

                    foreach (var childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = sender as TextBox;
            if (txt?.DataContext is Entity.Product product && product.IsSelected)
            {
                if (int.TryParse(txt.Text, out int quantity))
                {
                    product.QuantitySelect = quantity;
                    
                }
                else
                {
                    product.QuantitySelect = 0;
                }

                UpdateTotalPrice();
            }
        }

        private void btnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            bool checkLoad = GetOrder(sender, e);            
            if (checkLoad == true && productSelect.Count > 0)
            {
                UCCreateTransaction uCCreateTransaction = new UCCreateTransaction(this, wDHome);
                wDHome.GetUC(uCCreateTransaction);
            }
            else MessageBox.Show("Hãy chọn và nhập số lượng nông sản!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
