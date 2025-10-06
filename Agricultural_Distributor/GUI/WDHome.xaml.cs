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
using System.Windows.Shapes;

namespace Agricultural_Distributor
{
    /// <summary>
    /// Interaction logic for WDHome.xaml
    /// </summary>
    public partial class WDHome : Window
    {
        int position;

        public WDHome()
        {
            InitializeComponent();
            this.position = 0;
            LoadControlCenter();
        }

        private void LoadControlCenter()
        {
            UCControlCenter uCControlCenter = new UCControlCenter(this, position);
            //UCCreateOrder uCCreateOrder = new UCCreateOrder();
            GetUCBegin(uCControlCenter, uCControlCenter);
        }

        private void GetUCBegin(UserControl ucControl, UserControl ucWork)
        {
            Grid.SetRow(ucControl, 0);
            Grid.SetColumn(ucControl, 0);
            mainControl.Children.Add(ucControl);

            //Grid.SetRow(ucWork, 0);
            //Grid.SetColumn(ucWork, 1);
            //mainWork.Children.Add(ucWork);
        }

        public void GetUC(UserControl uc)
        {
            //mainWork.Children.Clear();
            Grid.SetRow(uc, 0);
            Grid.SetColumn(uc, 1);
            mainWork.Children.Add(uc);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
