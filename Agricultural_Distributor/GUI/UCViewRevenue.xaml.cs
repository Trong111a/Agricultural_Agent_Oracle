using Agricultural_Distributor.DAO;
using Agricultural_Distributor.Entity;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
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

namespace Agricultural_Distributor.GUI
{
    public partial class UCViewRevenue : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private List<string> listProducts;
        string ngay, thang, nam;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        WDHome wDHome;
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        public UCViewRevenue(WDHome wDHome, List<string> selectedProducts, string n)
        {
            InitializeComponent();
            this.wDHome = wDHome;
            DataContext = this;
            listProducts = selectedProducts;
            nam = n;
            RevenueDAO revenueDAO = new RevenueDAO();
            if (nam == "")
            {
                string[] labels;
                List<List<float>> doanhThuNam = revenueDAO.GetRevenueAll(listProducts, out labels);
                Labels = labels;
                CreateRevenueChartByYears(listProducts, doanhThuNam, labels);
            }

            else
            {
                List<List<float>> doanhThuNam = revenueDAO.GetRevenueByYear(listProducts, nam);
                CreateRevenueChart(listProducts, doanhThuNam);
            }
            
        }
        private void CreateRevenueChartByYears(List<string> productNames, List<List<float>> revenuesByYear, string[] years)
        {
            SeriesCollection = new SeriesCollection();

            for (int i = 0; i < productNames.Count; i++)
            {
                var productRevenue = new ChartValues<float>(revenuesByYear[i]);
                SeriesCollection.Add(new ColumnSeries
                {
                    Title = productNames[i],
                    Values = productRevenue
                });
            }

            Labels = years; // Mỗi nhãn là một năm
            Formatter = value => value.ToString("N0"); // Format doanh thu
        }

        public void CreateRevenueChart(List<string> listProducts, List<List<float>> revenuesByMonth)
        {
            SeriesCollection = new SeriesCollection();

            for (int i = 0; i < listProducts.Count; i++)
            {
                // Lấy doanh thu của từng sản phẩm theo tháng (12 giá trị)
                var monthlyRevenues = new ChartValues<float>();

                for (int month = 0; month < 12; month++)
                {
                    monthlyRevenues.Add(revenuesByMonth[month][i]);
                }

                // Tạo series cho từng sản phẩm
                SeriesCollection.Add(new ColumnSeries
                {
                    Title = listProducts[i],
                    Values = monthlyRevenues
                });
            }
            Labels = Enumerable.Range(1, 12).Select(m => $"Tháng {m}").ToArray();

            DataContext = this;
        }
    }
}
