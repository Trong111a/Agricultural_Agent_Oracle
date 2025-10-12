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
using Agricultural_Distributor.DAO;
using Agricultural_Distributor.Entity;

namespace Agricultural_Distributor.GUI
{
    /// <summary>
    /// Interaction logic for UCTransManagement.xaml
    /// </summary>
    public partial class UCTransManagement : UserControl
    {
        WDHome wDHome;
        public UCTransManagement(WDHome wDHome)
        {
            InitializeComponent();
            this.wDHome = wDHome;
            GetTrans();
        }

        private void GetTrans()
        {
            lvPur.ItemsSource = LoadTrans("Mua", null);
            lvSell.ItemsSource = LoadTrans("Bán", null);
        }

        private List<Transactions> LoadTrans(string typeOfReceipt, DateTime? dt)
        {
            TransactionsDAO transactionsDAO = new TransactionsDAO();
            return transactionsDAO.LoadTrans(typeOfReceipt, dt);
        }

        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView currentListView)
            {
                if (currentListView.SelectedItem != null)
                {
                    if (currentListView == lvPur)
                    {
                        if (lvSell.SelectedItem != null)
                        {
                            lvSell.SelectedItem = null;
                        }
                    }
                    else if (currentListView == lvSell)
                    {
                        if (lvPur.SelectedItem != null)
                        {
                            lvPur.SelectedItem = null;
                        }
                    }
                }

                if (currentListView.SelectedItem is Transactions selectedItem)
                {
                    lvDetail.ItemsSource = null;

                    int receiptId = selectedItem.ReceiptId;
                    int transId = selectedItem.TransactionId;
                    transIdSelect = transId;

                    ReceiptDAO receiptDAO = new ReceiptDAO();
                    lvDetail.ItemsSource = receiptDAO.GetReceiptDetailList(receiptId);
                    Receipt receipt = receiptDAO.GetReceipt(receiptId);

                    priceTotalSelect = receipt.PriceTotal;
                    tblDis.Text = receipt.Discount.ToString() + "%";
                    tblNote.Text = (receipt != null && receipt.Note != null) ? receipt.Note : "";

                    double priceTotal = receipt.PriceTotal;

                    TransactionsDAO transactionDAO = new TransactionsDAO();
                    int repay = transactionDAO.GetRepayment(transId);

                    if (repay == priceTotal)
                    {
                        tblStatusTrans.Text = "Đã thanh toán";
                        tblStatusTrans.Foreground = (Brush)new BrushConverter().ConvertFromString("#FF2E8068");
                        tblPaid.Text = FormatCurrencyVN(priceTotal.ToString());
                        tblLeft.Text = FormatCurrencyVN("0");
                        btnConfirmTrans.IsEnabled = false;
                        btnConfirmTrans.Background = Brushes.Transparent;
                        btnConfirmTrans.Foreground = (Brush)new BrushConverter().ConvertFromString("#FF2E8068");
                    }
                    else
                    {
                        tblStatusTrans.Text = "Chưa thanh toán";
                        tblStatusTrans.Foreground = new SolidColorBrush(Colors.Red);
                        tblPaid.Text = FormatCurrencyVN(repay.ToString());
                        tblLeft.Text = FormatCurrencyVN((priceTotal - repay).ToString());
                        btnConfirmTrans.IsEnabled = true;
                        btnConfirmTrans.Background = (Brush)new BrushConverter().ConvertFromString("#FF2E8068");
                        btnConfirmTrans.Foreground = (Brush)new BrushConverter().ConvertFromString("#FFFFFF");
                    }
                }
                else
                {
                    lvDetail.ItemsSource = null;
                    tblDis.Text = "";
                    tblNote.Text = "";
                    tblStatusTrans.Text = "";
                    tblPaid.Text = "";
                    tblLeft.Text = "";
                    btnConfirmTrans.IsEnabled = false;
                    btnConfirmTrans.Background = Brushes.Transparent;
                    btnConfirmTrans.Foreground = (Brush)new BrushConverter().ConvertFromString("#FF2E8068");
                }
            }
        }

        private string FormatCurrencyVN(string input)
        {
            if (long.TryParse(input, out long amount))
            {
                return string.Format("{0:N0} VNĐ", amount).Replace(",", ".");
            }
            return "0 VNĐ";
        }

        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            if (dtp.SelectedDate.HasValue)
            {
                DateTime selectedDate = dtp.SelectedDate.Value.Date;
                lvSell.ItemsSource = LoadTrans("Bán", selectedDate);
                lvPur.ItemsSource = LoadTrans("Mua", selectedDate);
            }
        }

        int transIdSelect = 0;
        double priceTotalSelect = 0;

        private void btnConfirmTrans_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn xác nhận hóa đơn này đã được thanh toán đầy đủ không?",
                "Xác nhận Thanh Toán",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                TransactionsDAO transactionsDAO = new TransactionsDAO();
                if (transactionsDAO.ConfirmTrans(transIdSelect, priceTotalSelect))
                {
                    MessageBox.Show("Xác nhận thanh toán thành công!");

                    tblStatusTrans.Text = "Đã thanh toán";
                    tblPaid.Text = FormatCurrencyVN(priceTotalSelect.ToString()); 
                    tblLeft.Text = FormatCurrencyVN("0"); 

                    btnConfirmTrans.IsEnabled = false;
                    btnConfirmTrans.Background = Brushes.Transparent;
                    btnConfirmTrans.Foreground = (Brush)new BrushConverter().ConvertFromString("#FF2E8068");
                    GetTrans();
                }
                else
                {
                    MessageBox.Show("Xác nhận thanh toán thất bại. Vui lòng kiểm tra lại.");
                }
            }
        }
    }
}
