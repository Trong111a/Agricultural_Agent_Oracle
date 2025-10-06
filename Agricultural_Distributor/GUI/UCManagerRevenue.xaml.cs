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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Agricultural_Distributor.GUI
{
    
    public partial class UCManagerRevenue : UserControl
    {
        WDHome wDHome;
        public UCManagerRevenue(WDHome wDHome)
        {
            InitializeComponent();
            this.wDHome = wDHome;
            LoadProduct();
        }
        private List<Entity.Revenue> revenues = new();
        public void LoadProduct()
        {
            RevenueDAO revenueDAO = new RevenueDAO();

            revenues = revenueDAO.LoadProduct();
            lvProducts.ItemsSource = revenues;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedProducts = revenues
                .Where(p => p.IsSelected)
                .Select(p => p.ProductName)
                .ToList();
            if (!selectedProducts.Any())
            {
                selectedProducts = revenues.Select(p => p.ProductName).ToList();
            }
            string nam = txtNam.Text;
            UCViewRevenue uCCreateOrder = new UCViewRevenue(wDHome, selectedProducts, nam);
            wDHome.GetUC(uCCreateOrder);
        }
    }
}
