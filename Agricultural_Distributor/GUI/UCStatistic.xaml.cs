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
    /// Interaction logic for UCStatistic.xaml
    /// </summary>
    public partial class UCStatistic : UserControl
    {
        WDHome wDHome;
        public UCStatistic(WDHome wDHome)
        {
            InitializeComponent();
            this.wDHome = wDHome;
            GetProduct();
            lsFore.ItemsSource = PredictProducts(productStatistics);
        }

        private void lsStatistic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listview && listview.SelectedItem is Product selectedItem)
            {
                lsDetail.ItemsSource = null;
                int productId = selectedItem.ProductId;
                ProductDAO productDAO = new ProductDAO();
                lsDetail.ItemsSource = productDAO.GetQuanMonth(productId);
            }
        }

        List<ProductStatistic> productStatistics = new List<ProductStatistic>();

        private void GetProduct()
        {
            ProductDAO productDAO = new ProductDAO();
            List<Product> products = productDAO.LoadProduct();
            lsStatistic.ItemsSource = products;
            foreach (Product product in products)
            {
                ReceiptDetailDAO receiptDetailDAO = new ReceiptDetailDAO();
                if (receiptDetailDAO.CheckProductExist(product.ProductId))
                {
                    ProductStatistic productStatistic = new ProductStatistic();
                    productStatistic.ProductId = product.ProductId;
                    productStatistic.ProductName = product.Name;
                    ProductDAO productDetailDAO = new ProductDAO();
                    List<QuanMonth> quanMonths = productDetailDAO.GetQuanMonth(product.ProductId);
                    foreach (QuanMonth item in quanMonths)
                    {
                        productStatistic.MonthlyQuantities.Add(item.Quan);
                    }
                    productStatistics.Add(productStatistic);
                }
            }

        }

        private (double a, double b) CalculateRegression(List<int> x, List<int> y)
        {
            int n = x.Count;
            double sumX = x.Sum();
            double sumY = y.Sum();
            double sumXY = x.Zip(y, (xi, yi) => xi * yi).Sum();
            double sumX2 = x.Select(xi => xi * xi).Sum();

            double a = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            double b = (sumY - a * sumX) / n;

            return (a, b);
        }

        private int Predict(int futureX, double a, double b)
        {
            return (int)Math.Round(a * futureX + b);
        }

        private List<ProductPrediction> PredictProducts(List<ProductStatistic> allProducts)
        {
            List<ProductPrediction> predictions = new();

            foreach (var product in allProducts)
            {
                var recentMonths = product.MonthlyQuantities.
                    Skip(Math.Max(0, product.MonthlyQuantities.Count - 3))
                    .ToList();
                if (recentMonths.Count >= 2)
                {
                    var x = Enumerable.Range(1, recentMonths.Count).ToList();
                    var y = recentMonths;

                    var (a, b) = CalculateRegression(x, y);
                    int nextMonth = x.Count + 1;
                    int predicted = Predict(nextMonth, a, b);

                    ProductPrediction proPre = new ProductPrediction();
                    proPre.ProductId = product.ProductId;
                    proPre.ProductName = product.ProductName;
                    proPre.PredictedQuantity = predicted;

                    predictions.Add(proPre);
                }
                else
                {
                    if (recentMonths.Count == 1)
                    {
                        ProductPrediction proPre = new ProductPrediction();
                        proPre.ProductId = product.ProductId;
                        proPre.ProductName = product.ProductName;
                        proPre.PredictedQuantity = recentMonths[0];

                        predictions.Add(proPre);
                    }
                }
            }

            return predictions
                .OrderByDescending(p => p.PredictedQuantity)
                //.Take(3)
                .ToList();
        }
    }
}
