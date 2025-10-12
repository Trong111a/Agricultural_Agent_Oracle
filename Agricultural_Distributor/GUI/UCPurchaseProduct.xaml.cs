using Agricultural_Distributor.DAO;
using Agricultural_Distributor.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Agricultural_Distributor.GUI
{
    /// <summary>
    /// Interaction logic for UCPurchaseProduct.xaml
    /// </summary>
    public partial class UCPurchaseProduct : UserControl
    {
       // internal List<Entity.Product> productSelect = new();
        private List<Entity.Product> products = new();

        internal List<Entity.Product> listProduct = new List<Entity.Product>();
        internal string result = "";
        internal string discount;

        internal string note;
        WDHome wDHome;
        public UCPurchaseProduct(WDHome wDHome)
        {
            InitializeComponent();
            result = "Mua";
            this.wDHome = wDHome;
            LoadProducts();
            SetList();
            lvNewProducts.ItemsSource = lsNew;
            
        }

        private TextBox FindTextBoxInContainer(DependencyObject container)
        {
            if (container == null) return null;

            var textBoxes = FindVisualChildren<TextBox>(container);
            return textBoxes.FirstOrDefault();
        }


        private bool GetOrder(object sender, RoutedEventArgs e)
        {
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
                            listProduct.Add(product);
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
                        if (listProduct.Contains(product))
                        {
                            listProduct.Remove(product);
                        }
                    }
                }
            }
            return true;
        }

        List<Product> lsNew = new List<Product>();

        private void SetList()
        {
            for (int i = 0; i < 6; i++)
            {
                Product product = new Product();
                lsNew.Add(product);
            }
        }

        private void LoadProducts()
        {
            ProductDAO productDAO = new ProductDAO();
            products = productDAO.LoadProduct();
            foreach (var product in products)
            {
                product.PurPriceOriginal = product.PurchasePrice;
            }
            lvProducts.ItemsSource = products;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb?.DataContext is Entity.Product product)
            {
                product.IsSelected = true;

                var container = lvProducts.ItemContainerGenerator.ContainerFromItem(product) as ListViewItem;
                var txtQuantity = FindTextBoxInContainer(container, "txtQuantity_lsvProducts");
                if (txtQuantity != null)
                {
                    txtQuantity.IsEnabled = true;
                }

                var txtPurPrice = FindTextBoxInContainer(container, "txtPurPrice_lsvProducts");
                if (txtPurPrice != null) 
                {
                    txtPurPrice.IsEnabled = true;
                }

                UpdateTotalPrice();
                listProduct.Add(product);
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

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb?.DataContext is Entity.Product product)
            {
                product.IsSelected = false;
                product.QuantitySelect = 0;

                var container = lvProducts.ItemContainerGenerator.ContainerFromItem(product) as ListViewItem;
                var txtQuantity = FindTextBoxInContainer(container, "txtQuantity_lsvProducts");
                if (txtQuantity != null)
                {
                    txtQuantity.Text = "";
                    txtQuantity.IsEnabled = false;
                }

                var txtPurPrice = FindTextBoxInContainer(container, "txtPurPrice_lsvProducts");
                if (txtPurPrice != null)
                {

                    txtPurPrice.Text = product.PurPriceOriginal.ToString();
                    txtPurPrice.IsEnabled = false;
                }

                UpdateTotalPrice();
                listProduct.Remove(product);
            }
        }
        private TextBox FindTextBoxInContainer(DependencyObject container, string name)
        {
            if (container == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(container); i++)
            {
                var child = VisualTreeHelper.GetChild(container, i);
                if (child is TextBox tb && tb.Name == name)
                    return tb;

                var result = FindTextBoxInContainer(child, name);
                if (result != null) return result;
            }

            return null;
        }
        private TextBox FindTextBoxInContainer(ListViewItem container, string name)
        {
            return FindVisualChild<TextBox>(container, name);
        }

        private T FindVisualChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T element && element.Name == name)
                {
                    return element;
                }

                var result = FindVisualChild<T>(child, name);
                if (result != null)
                    return result;
            }
            return null;
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
        private void Quantity_TextChanged(object sender, TextChangedEventArgs e)
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
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Entity.Product product)
            {
                if (ValidateProduct(product))
                {

                    double temp = ConverPrice(txtTotalPrice.Text);
                    double newProPrice = temp + product.PurchasePrice * product.Quantity;
                    txtTotalPrice.Text = $"Tổng tiền: {newProPrice:N0} đ";

                    MessageBox.Show("Đã thêm sản phẩm vào danh sách!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        private bool ValidateProduct(Entity.Product product)
        {
            if (product.Quantity <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0!");
                return false;
            }

            if (product.PurchasePrice <= 0)
            {
                MessageBox.Show("Giá mua phải lớn hơn 0!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(product.MeasurementUnit) || !Regex.IsMatch(product.MeasurementUnit, @"^[a-zA-Z]+$"))
            {
                MessageBox.Show("Đơn vị đo phải chỉ chứa chữ cái!");
                return false;
            }

            return true;
        }

    
        private void btnCreateList_Click(object sender, RoutedEventArgs e)
        {
            if (tbDiscount.Text != null) discount = tbDiscount.Text;
            else discount = "";
            if (tbNote.Text != null) note = tbNote.Text;
            else note = "";

            listProduct.Clear();

            foreach (var product in products)
            {
                if (product.IsSelected && product.QuantitySelect > 0)
                {
                    product.Quantity = product.QuantitySelect;
                    listProduct.Add(product);
                }
            }

            GetNewProducts();

            if (listProduct.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một sản phẩm hoặc thêm sản phẩm mới.");
                return;
            }

            UCCreateTransaction uCCreateTransaction = new UCCreateTransaction(wDHome, this);
            wDHome.GetUC(uCCreateTransaction);
        }

        private bool GetNewProducts()
        {
            for (int i = 0; i < lvNewProducts.Items.Count; i++)
            {
                if (lvNewProducts.Items[i] is Entity.Product product)
                {
                    var container = lvNewProducts.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                    if (container == null) continue;

                    var txtName = FindVisualChild<TextBox>(container, "txtName_lsvNew");
                    var txtQuantity = FindVisualChild<TextBox>(container, "txtQuantity_lsvNew");
                    var txtPurchasePrice = FindVisualChild<TextBox>(container, "txtPurchasePrice_lsvNew");
                    var txtMeasurementUnit = FindVisualChild<TextBox>(container, "txtMeasurementUnit_lsvNew");
                    var txtQualityStandard = FindVisualChild<TextBox>(container, "txtQualityStandard_lsvNew");

                    if (txtName != null && !string.IsNullOrWhiteSpace(txtName.Text))
                    {
                        product.Name = txtName.Text;
                        product.QualityStandard = txtQualityStandard?.Text;

                        if (txtQuantity == null || !int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
                        {
                            if (!string.IsNullOrWhiteSpace(txtName.Text))
                            {
                                MessageBox.Show($"Sản phẩm mới '{product.Name}': Số lượng phải là số nguyên dương.");
                                FocusTxt(txtQuantity);
                            }
                            return false;
                        }
                        product.Quantity = quantity;

                        if (txtPurchasePrice == null || !double.TryParse(txtPurchasePrice.Text, out double price) || price <= 0)
                        {
                            if (!string.IsNullOrWhiteSpace(txtName.Text))
                            {
                                MessageBox.Show($"Sản phẩm mới '{product.Name}': Giá mua phải là số dương.");
                                FocusTxt(txtPurchasePrice);
                            }
                            return false; 
                        }
                        product.PurchasePrice = price;
                        if (txtMeasurementUnit == null || string.IsNullOrWhiteSpace(txtMeasurementUnit.Text) || !Regex.IsMatch(txtMeasurementUnit.Text, @"^[a-zA-Z]+$"))
                        {
                            if (!string.IsNullOrWhiteSpace(txtName.Text))
                            {
                                MessageBox.Show($"Sản phẩm mới '{product.Name}': Đơn vị đo phải chỉ chứa chữ cái.");
                                FocusTxt(txtMeasurementUnit);
                            }
                            return false; 
                        }
                        product.MeasurementUnit = txtMeasurementUnit.Text;

                        if (!listProduct.Contains(product))
                        {
                            listProduct.Add(product);
                        }
                    }
                }
            }
            return true; 
        }

        private void txtName_LostFocus(object sender, RoutedEventArgs e)
        {
            var txtName = sender as TextBox;
            if (txtName?.DataContext is Entity.Product product)
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Không thể để trống ô này!!");
                    txtName.Text = "0";
                    FocusTxt(txtName);
                }
                else
                {
                    product.Name = txtName.Text;
                }
            }
        }

        private void FocusTxt(TextBox txt)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                txt.Focus();
            }), System.Windows.Threading.DispatcherPriority.Input);
        }
        private void txtQuantity_LostFocus(object sender, RoutedEventArgs e)
        {
            var txtQuantity = sender as TextBox;
            if (txtQuantity?.DataContext is Entity.Product product)
            {
                if (string.IsNullOrWhiteSpace(txtQuantity.Text))
                {
                    MessageBox.Show("Không được bỏ trống ô này");
                    txtQuantity.Text = "0";
                    int quantity = int.Parse(txtQuantity.Text);
                    FocusTxt(txtQuantity);                   
                }
                else if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
                {
                    MessageBox.Show("Số lượng phải là số lớn hơn 0");
                    txtQuantity.Text = "0";
                    quantity = 0;
                    FocusTxt(txtQuantity);
                }
                else {
                    product.Quantity = quantity;
                }
            }
        }

        private void txtPurchasePrice_LostFocus(object sender, RoutedEventArgs e)
        {
            var txtPurchasePrice = sender as TextBox;
            if (txtPurchasePrice?.DataContext is Entity.Product product)
            {
                if (string.IsNullOrWhiteSpace(txtPurchasePrice.Text))
                {
                    MessageBox.Show("Không được bỏ trống ô này");
                    txtPurchasePrice.Text = "0";
                    double price = double.Parse(txtPurchasePrice.Text);
                    FocusTxt(txtPurchasePrice);
                    
                }
                else if (!double.TryParse(txtPurchasePrice.Text, out double purchasePrice) || purchasePrice < 0)
                {
                    MessageBox.Show("Giá mua phải là số lớn hơn 0");
                    txtPurchasePrice.Text = "0";
                    purchasePrice = 0;
                    FocusTxt(txtPurchasePrice);
                }
                else
                {
                    product.PurchasePrice = purchasePrice;
                }               
            }
        }

        private void txtQualityStandard_LostFocus(object sender, RoutedEventArgs e)
        {
            var txtQualityStandard = sender as TextBox;
            if (txtQualityStandard?.DataContext is Entity.Product product)
            {
                product.QualityStandard = txtQualityStandard.Text;
            }
        }
        private void txtMeasurementUnit_LostFocus(object sender, RoutedEventArgs e)
        {
            var txtMeasurementUnit = sender as TextBox;
            if (txtMeasurementUnit?.DataContext is Entity.Product product)
            {
                if (string.IsNullOrWhiteSpace(txtMeasurementUnit.Text))
                {
                    MessageBox.Show("Không được để trống ô này");
                    txtMeasurementUnit.Text = "0";
                    FocusTxt(txtMeasurementUnit);

                }
                else if (!Regex.IsMatch(txtMeasurementUnit.Text, @"^[a-zA-Z]+$"))
                {
                    MessageBox.Show("Đơn vị chỉ được chứa các chữ cái (không chứa số hoặc ký tự đặc biệt)");
                    txtMeasurementUnit.Text = "0";
                    FocusTxt(txtMeasurementUnit);
                }
                else
                {
                    product.MeasurementUnit = txtMeasurementUnit.Text;
                }
            }
        }
        private void txtPurPrice_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox txt && txt.DataContext is Entity.Product product && product.IsSelected)
            {
                if (double.TryParse(txt.Text, out double price) && price > 0)
                {
                    product.PurchasePriceSelect = price;
                }
                else
                {
                    MessageBox.Show("Giá mua phải là số lớn hơn 0");
                    txt.Text = product.PurPriceOriginal.ToString();
                    FocusTxt(txt);
                }
            }
        }

        private double ConverPrice(string priceStr)
        {
            string text = priceStr;

            string price = text.Replace("Tổng tiền:", "")
                                    .Replace("đ", "")
                                    .Trim()
                                    .Replace(".", "");

            if (double.TryParse(price, out double totalAmount))
            {
                return totalAmount;
            }
            else
            {
                return -1;
            }

        }
    }
}
