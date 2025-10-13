using Agricultural_Distributor.Common;
using Agricultural_Distributor.DAO;
using Agricultural_Distributor.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
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

namespace Agricultural_Distributor.GUI
{
    /// <summary>
    /// Interaction logic for UCCreateTransaction.xaml
    /// </summary>
    public partial class UCCreateTransaction : UserControl
    {
        UCCreateOrder uCCreateOrder;

        WDHome wDHome;

        string priceTotal = "";

        double paidPrice;

        int repayment;

        bool checkCustomer;

        double priceRes;



        public UCCreateTransaction(UCCreateOrder uCCreateOrder, WDHome wDHome)
        {
            InitializeComponent();
            this.uCCreateOrder = uCCreateOrder;
            this.wDHome = wDHome;
            priceTotal = uCCreateOrder.txtTotalPrice.Text;
            double price = ConverPrice(priceTotal);
            double dis = 0;
            if (uCCreateOrder.tbDiscount.Text != "") dis = Convert.ToDouble(uCCreateOrder.tbDiscount.Text);
            priceRes = price * (1 - (dis / 100));
            tbRestIs.Text = $"{priceRes:N0} đ";
        }

        UCPurchaseProduct uCPurchase;

        public UCCreateTransaction(WDHome wDHome, UCPurchaseProduct uCPurchaseProduct)
        {
            InitializeComponent();
            this.uCPurchase = uCPurchaseProduct;
            this.wDHome = wDHome;
            priceTotal = uCPurchaseProduct.txtTotalPrice.Text;
            double price = ConverPrice(priceTotal);
            double dis = 0;
            if (uCPurchaseProduct.tbDiscount.Text != "") dis = Convert.ToDouble(uCPurchaseProduct.tbDiscount.Text);
            priceRes = price * (1 - (dis / 100));
            tbRestIs.Text = $"{priceRes:N0} đ";
            btnBack.Visibility = Visibility.Collapsed;
            FillListProduct();
            MessageBox.Show(this.uCPurchase.listProduct.Count().ToString());
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            wDHome.mainWork.Children.RemoveAt(wDHome.mainWork.Children.Count - 1);
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

        private double CalPrice()
        {
            double total;
            try
            {
                paidPrice = Convert.ToDouble(tbPaid.Text);
                total = priceRes - paidPrice;
            }
            catch
            {
                total = -1;
            }            
            return total;
        }

        private void tbPaid_TextChanged(object sender, TextChangedEventArgs e)
        {
            double total = CalPrice();
            if (total != -1) tbRestIs.Text = $"{total:N0} đ";
        }

        List<Product> listProNonId = new List<Product>();
        List<Product> listProId = new List<Product>();

        private void FillListProduct()
        {
            foreach (Product item in uCPurchase.listProduct)
            {
                if (item.ProductId != null)
                {
                    listProId.Add(item);
                }
                else listProNonId.Add(item);
            }
            if (listProId.Count > 0) UpdateProId();
            if (listProNonId.Count > 0) listProAfterAdd();
        }

        private void UpdateProId()
        {
            ProductDAO productDAO = new ProductDAO();
            foreach (var item in listProId)
            {
                WarehouseInfoDAO warehouseInfoDAO = new WarehouseInfoDAO();
                WarehouseInfo warehouseInfo = warehouseInfoDAO.GetInfoProduct(item.ProductId);
                item.Quantity = item.QuantitySelect + warehouseInfo.Quantity;
                productDAO.UpdateQuanPurPriceProduct(item);
            }
        }

        //BUG 

        private void listProAfterAdd()
        {
            ProductDAO productDAO = new ProductDAO();

            foreach (var item in listProNonId)
            {
                //item.Quantity = item.QuantitySelect;
                int? proId = productDAO.AddProduct(item);

                //int quan = item.Quantity;
                if (proId.HasValue)
                {
                    item.ProductId = proId.Value;
                    listProId.Add(item);
                }
            }
        }
        
        //BUG
        private void btnCreateTrans_Click(object sender, RoutedEventArgs e)
        {
            if (tbPhone.Text.Length > 0 && tbName.Text.Length > 0 && tbEmail.Text.Length > 0 && tbAddress.Text.Length > 0 && tbPaid.Text.Length > 0)
            {
                bool check;
                if (checkCustomer == false) check = AddCustomer();
                else check = UpdateCustomer();
                if (check)
                {
                    listProAfterAdd();

                    Receipt receipt = new Receipt();
                    if (uCCreateOrder != null)
                    {
                        receipt.TypeOfReceipt = uCCreateOrder.result;
                        receipt.ProductList = JsonConvert.SerializeObject(uCCreateOrder.productSelect);
                        if (uCCreateOrder.discount != "") receipt.Discount = Convert.ToDouble(uCCreateOrder.discount);
                        else receipt.Discount = 0;
                        receipt.Note = uCCreateOrder.note;
                        receipt.PriceTotal = priceRes;
                    }
                    else
                    {
                        

                        receipt.TypeOfReceipt = uCPurchase.result;
                        receipt.ProductList = JsonConvert.SerializeObject(listProId);
                        if (uCPurchase.discount != "") receipt.Discount = Convert.ToDouble(uCPurchase.discount);
                        else receipt.Discount = 0;
                        receipt.Note = uCPurchase.note;
                        receipt.PriceTotal = priceRes;
                    }

                    ReceiptDAO receiptDAO = new ReceiptDAO();
                    int? receiptId = receiptDAO.CreateReceipt(receipt);

                    if (receiptId.HasValue)
                    {
                        int nonNullableReceiptId = receiptId.Value;

                        AddReceipDetail(nonNullableReceiptId);
                        repayment = Convert.ToInt32(ConverPrice(tbPaid.Text));

                        Transactions transactions = new Transactions();
                        transactions.ReceiptId = nonNullableReceiptId; 
                        transactions.DeliveryAddress = tbAddress.Text;
                        transactions.Repayment = repayment;

                        int employeeIdValue;

                        if (SessionManager.IsAdmin == false)
                        {
                            if (SessionManager.AccountId.HasValue)
                            {
                                transactions.EmployeeId = SessionManager.AccountId.Value;
                            }
                            else
                            {
                                transactions.EmployeeId = 1;
                            }
                        }
                        else 
                        {
                            transactions.EmployeeId = 1;
                        }

                        transactions.CustomerId = customerID;
                        TransactionsDAO transactionsDAO = new TransactionsDAO(transactions);

                        if (transactionsDAO.CreateTrans())
                        {
                            foreach (var item in listProId)
                            {
                                MessageBox.Show(
                                $"Đã thêm sản phẩm:\n" +
                                $"- Tên: {item.ProductId}\n" +
                                $"- Số lượng: {item.Quantity}\n",

                                "Thông báo",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information
                            );
                            }
                            MessageBox.Show("Tạo đơn hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                            UCManageProduct uCManageProduct = new UCManageProduct(wDHome);
                            wDHome.GetUC(uCManageProduct);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Hãy nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void AddReceipDetail(int receiptId)
        {
            // Khởi tạo DAO cần thiết
            ProductDAO productDAO = new ProductDAO();
            ReceiptDAO receiptDAO = new ReceiptDAO();

            // ----------------------------------------------------
            // PHẦN XỬ LÝ ĐƠN BÁN HÀNG (uCCreateOrder != null)
            // Giả định các sản phẩm bán ra đều đã tồn tại trong DB, không cần logic AddProduct
            // ----------------------------------------------------
            if (uCCreateOrder != null)
            {
                foreach (Product pro in uCCreateOrder.productSelect)
                {
                    ReceiptDetail receiptDetail = new ReceiptDetail();
                    receiptDetail.ReceiptId = receiptId;
                    receiptDetail.ProductId = pro.ProductId;
                    receiptDetail.ProductName = pro.Name;
                    receiptDetail.Quantity = pro.Quantity;
                    receiptDetail.UnitPrice = pro.SellingPrice; // Giá bán

                    // ReceiptDAO receiptDAO = new ReceiptDAO(); // Không cần khởi tạo lại DAO trong vòng lặp
                    bool temp = receiptDAO.AddReceiptDetail(receiptDetail);
                    // Có thể thêm logic kiểm tra 'temp' ở đây
                }
            }
            // ----------------------------------------------------
            // PHẦN XỬ LÝ ĐƠN MUA HÀNG (uCCreateOrder == null)
            // Cần kiểm tra và lưu sản phẩm MỚI vào DB
            // ----------------------------------------------------
            else
            {
                foreach (Product pro in listProId) // listProId chứa listProduct từ UCPurchaseProduct
                {
                    int finalProductId = pro.ProductId;

                    // BƯỚC 1: KIỂM TRA VÀ CHÈN SẢN PHẨM MỚI (ProductId = 0)
                    if (pro.ProductId == 0)
                    {
                        // Cài đặt giá bán tạm thời cho sản phẩm mới (giả sử bằng giá mua)
                        if (pro.SellingPrice <= 0)
                        {
                            pro.SellingPrice = pro.PurchasePrice;
                        }

                        // Cài đặt Photo mặc định nếu DAO yêu cầu (giả định bạn không cần ảnh cho đơn mua)
                        pro.Photo = null;

                        // GỌI DAO ĐỂ THÊM SẢN PHẨM VÀ LẤY ID MỚI
                        int? newProductId = productDAO.AddProduct(pro);

                        if (newProductId.HasValue)
                        {
                            finalProductId = newProductId.Value;
                            pro.ProductId = finalProductId; // Cập nhật ProductId cho đối tượng trong bộ nhớ
                        }
                        else
                        {
                            // Xử lý lỗi: Không thể chèn sản phẩm mới, bỏ qua chi tiết hóa đơn này
                            MessageBox.Show($"CẢNH BÁO: Không thể lưu sản phẩm mới '{pro.Name}' vào Database. Chi tiết hóa đơn này bị bỏ qua.");
                            continue;
                        }
                    }

                    // BƯỚC 2: LƯU CHI TIẾT HÓA ĐƠN VỚI ProductId HỢP LỆ
                    ReceiptDetail receiptDetail = new ReceiptDetail();
                    receiptDetail.ReceiptId = receiptId;
                    receiptDetail.ProductId = finalProductId; // SỬ DỤNG ID HỢP LỆ (cũ hoặc mới)
                    //receiptDetail.ProductName = pro.Name;

                    // Xử lý Quantity (đã có logic tốt)
                    if (pro.QuantitySelect != null && pro.QuantitySelect != 0)
                        receiptDetail.Quantity = pro.QuantitySelect;
                    else
                        receiptDetail.Quantity = pro.Quantity;

                    receiptDetail.UnitPrice = pro.PurchasePrice; // Giá mua

                    // ReceiptDAO receiptDAO = new ReceiptDAO(); // Không cần khởi tạo lại DAO trong vòng lặp
                    bool temp = receiptDAO.AddReceiptDetail(receiptDetail);
                    // Có thể thêm logic kiểm tra 'temp' ở đây
                }
            }
        }

        private bool AddCustomer()
        {
            Customer customer = new Customer();
            customer.CustomerName = tbName.Text;
            customer.CustomerAddress = tbAddress.Text;
            customer.PhoneNumber = tbPhone.Text;
            customer.Email = tbEmail.Text;

            CustomerDAO customerDAO = new CustomerDAO();
            int customerId = customerDAO.AddCustomer(customer);
            customerID = customerId;
            return true;
        }

        int customerID;

        private bool UpdateCustomer()
        {
            CustomerDAO customerDAO = new CustomerDAO();
            Customer customer = customerDAO.GetCustomer(tbPhone.Text);
            customer.CustomerName = tbName.Text;
            customer.CustomerAddress = tbAddress.Text;
            customer.Email= tbEmail.Text;
            customer.PhoneNumber = tbPhone.Text.Trim();
            customerID = customer.CustomerId;

            return customerDAO.UpdateCustomer(customer);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == cbPayFull)
            {
                repayment = Convert.ToInt32(ConverPrice(priceRes.ToString()));
                cbNotPay.IsChecked = false;
                cbParticalPay.IsChecked = false;
                tbRestIs.Text = $"{0:N0} đ";
            }
            else if (sender == cbNotPay)
            {
                repayment = 0;
                cbPayFull.IsChecked = false;
                cbParticalPay.IsChecked = false;
                tbRestIs.Text = $"{priceRes:N0} đ";
            }
            else if (sender == cbParticalPay)
            {
                cbPayFull.IsChecked = false;
                cbNotPay.IsChecked = false;
                tbRestIs.Text = $"{priceRes:N0} đ";
            }
            tbPaid.Text = $"{repayment:N0} đ";
        }

        private bool IsTextNumeric(string text)
        {
            return text.All(char.IsDigit);
        }

        private void tbPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private string phoneTemp;

        private void tbPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbPhone.Text.Length <10)
            {
                tbPhone.Foreground = Brushes.Black;
            }
            else if (tbPhone.Text.Length == 10)
            {
                phoneTemp = tbPhone.Text;
                tbPhone.Foreground = Brushes.Red;
            }
            else if (tbPhone.Text.Length > 10)
            {
                tbPhone.Text = phoneTemp;
                tbPhone.SelectionStart = tbPhone.Text.Length;
            }
            checkCustomer = false;
        }

        private void tbPhone_LostFocus(object sender, RoutedEventArgs e)
        {
            CustomerDAO customerDAO = new CustomerDAO();
            Customer customer = customerDAO.GetCustomer(tbPhone.Text);
            if (customer != null)
            {
                tbName.Text = customer.CustomerName;
                tbAddress.Text = customer.CustomerAddress;
                tbEmail.Text = customer.Email;
                emailCheck = customer.Email;
                checkCustomer = true;
            }
            else checkCustomer = false;
        }

        string emailCheck = "";

        private void tbEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (emailCheck != tbEmail.Text)
            {
                CustomerDAO customerDAO = new CustomerDAO();
                if (customerDAO.CheckExist(tbEmail.Text)) MessageBox.Show("Đã tồn tại email", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }
}
