using Agricultural_Distributor.Common;
using Agricultural_Distributor.DAO;
using Agricultural_Distributor.Entity;
using Agricultural_Distributor.GUI;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace Agricultural_Distributor
{
    /// <summary>
    /// Interaction logic for UCControlCenter.xaml
    /// </summary>
    public partial class UCControlCenter : UserControl
    {
        WDHome? wDHome;
        int position;

        public UCControlCenter(WDHome wDHome, int position)
        {
            InitializeComponent();
            this.wDHome = wDHome;
            this.position = position;
            LoadControlCenter();
            if (SessionManager.IsAdmin == false)
            {
                //btnViewRevenue.Visibility = Visibility.Collapsed;
                btnAudit.Visibility = Visibility.Collapsed;
                //btnTransManage.Visibility = Visibility.Collapsed;
            } 
        }

        private void LoadControlCenter()
        {
            if (position == 0) btnViewRevenue.Visibility = Visibility.Visible;
        }

        private void btnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Chọn Yes để **MUA**, chọn No để **BÁN** nông sản", "BẠN MUỐN TẠO ĐƠN MUA HAY ĐƠN BÁN?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
            {
                UCCreateOrder uCCreateOrder = new UCCreateOrder(wDHome);
                wDHome.GetUC(uCCreateOrder);
            }
            else
            {
                UCPurchaseProduct uCPurchaseProduct = new UCPurchaseProduct(wDHome);
                wDHome.GetUC(uCPurchaseProduct);
            }
            
        }

        private void btnProductManage_Click(object sender, RoutedEventArgs e)
        {
            UCManageProduct uCManageProduct = new UCManageProduct(wDHome);
            wDHome.GetUC(uCManageProduct);
        }

        private void btnCustomerManage_Click(object sender, RoutedEventArgs e)
        {
            UCManageCustomer uCManageCustomer = new UCManageCustomer(wDHome);
            wDHome.GetUC(uCManageCustomer);
        }

        private void btnTransManage_Click(object sender, RoutedEventArgs e)
        {
            UCTransManagement uCTransManagement = new UCTransManagement(wDHome);
            wDHome.GetUC(uCTransManagement);
        }

        private void btnStatistic_Click(object sender, RoutedEventArgs e)
        {
            UCStatistic uCStatistic = new UCStatistic(wDHome);
            wDHome.GetUC(uCStatistic);
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWD = new LoginWindow();

            loginWD.Show();

            
            if (wDHome != null)
            {
                wDHome.Close();
                wDHome = null; 
            }
        }

        //private void btnProductManage_Click_1(object sender, RoutedEventArgs e)
        //{
        //    UCManageProduct uCManageProduct = new UCManageProduct(wDHome);
        //    wDHome.GetUC(uCManageProduct);
        //}

        private void btnEmployeeManage_Click(object sender, RoutedEventArgs e)
        {
            UCManageEmployee uCCreateOrder = new UCManageEmployee(wDHome);
            wDHome.GetUC(uCCreateOrder);
        }

        private void btnViewRevenue_Click(object sender, RoutedEventArgs e)
        {
            UCManagerRevenue uCCreateOrder = new UCManagerRevenue(wDHome);
            wDHome.GetUC(uCCreateOrder);
        }

        private void btnViewTimekeeping_Click(object sender, RoutedEventArgs e)
        {
            UCCheckWorkSchedule uCCreateOrder = new UCCheckWorkSchedule(wDHome);
            wDHome.GetUC(uCCreateOrder);
        }

        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            UCAudit audit = new UCAudit(wDHome);
            wDHome.GetUC(audit);
        }
    }
}
