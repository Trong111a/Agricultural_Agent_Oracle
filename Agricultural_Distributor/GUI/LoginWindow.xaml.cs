using Agricultural_Distributor.Common;
using Agricultural_Distributor.DAO;
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

namespace Agricultural_Distributor.GUI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void ImageCloseEye_MouseDown(object sender, MouseButtonEventArgs e)
        {

            txtPasswordVisible.Text = passwordBox.Password;


            txtPasswordVisible.Visibility = Visibility.Visible;
            passwordBox.Visibility = Visibility.Collapsed;

 
            ImageCloseEye.Visibility = Visibility.Collapsed;
            ImageEye.Visibility = Visibility.Visible;
        }

        private void ImageEye_MouseDown(object sender, MouseButtonEventArgs e)
        {
            passwordBox.Password = txtPasswordVisible.Text;

            txtPasswordVisible.Visibility = Visibility.Collapsed;
            passwordBox.Visibility = Visibility.Visible;

            ImageCloseEye.Visibility = Visibility.Visible;
            ImageEye.Visibility = Visibility.Collapsed;
        }

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (txtPasswordVisible.Visibility == Visibility.Visible)
            {
                txtPasswordVisible.Text = passwordBox.Password;
            }
        }

        // private void btnLogin_Click(object sender, RoutedEventArgs e)
        // {
        //     string username = txtUsername.Text;
        //     string password = (passwordBox.Visibility == Visibility.Visible) ? passwordBox.Password : txtPasswordVisible.Text;

        //     AccountDAO accountDAO = new AccountDAO();
        //     bool isLoggedIn = accountDAO.CheckLogin(username, password);

        //     if (isLoggedIn)
        //     {
        //         var loggedAccount = accountDAO.GetLoggedInAccount();

        //         SessionManager.Username = loggedAccount.Username;
        //         SessionManager.IsAdmin = loggedAccount.IsAdmin;
        //         SessionManager.AccountId = loggedAccount.Id;

        //         MessageBox.Show($"Xin chào {loggedAccount.Username}!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

        //         WDHome home = new WDHome();
        //         UCManageProduct uCManageProduct = new UCManageProduct(home);
        //         home.GetUC(uCManageProduct);
        //         home.Show();
        //         this.Hide();
        //     }
        //     else
        //     {
        //         MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        //     }

        // }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = (passwordBox.Visibility == Visibility.Visible)
                                ? passwordBox.Password
                                : txtPasswordVisible.Text;

            try
            {

                var connect = new Connect(username, password);
                connect.ConnectDB();
                AccountDAO accountDAO = new AccountDAO();
                bool isLoggedIn = accountDAO.CheckLogin(username, password);

                SessionManager.Username = username;


                //SessionManager.IsAdmin = false; 
                SessionManager.Connect = connect;

                var roles = RoleManager.GetUserRoles(connect);
                SessionManager.Roles = roles;
                if (roles.Contains("CHUDAILY"))
                {

                    SessionManager.IsAdmin = true;
                }
                else
                {
                    SessionManager.IsAdmin = false;
                }
                if (isLoggedIn)
                {
                    var loggedAccount = accountDAO.GetLoggedInAccount();

                    SessionManager.Username = loggedAccount.Username;
                    SessionManager.IsAdmin = loggedAccount.IsAdmin;
                    SessionManager.AccountId = loggedAccount.Id;

                    MessageBox.Show($"Xin chào {username}!", "Đăng nhập thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    WDHome home = new WDHome();
                    UCManageProduct uCManageProduct = new UCManageProduct(home);
                    home.GetUC(uCManageProduct);
                    home.Show();
                    this.Hide();
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show("❌ Sai tên đăng nhập hoặc mật khẩu.\n" +ex.Message, "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
 

        private void btnForgot_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            ForgotPasswordWindow f = new ForgotPasswordWindow();
            f.Show();
        }
    }
}
