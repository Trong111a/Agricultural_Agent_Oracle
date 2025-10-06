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
using System.Net;
using System.Net.Mail;
using System.Windows.Threading;


namespace Agricultural_Distributor.GUI
{
    /// <summary>
    /// Interaction logic for ForgotPasswordWindow.xaml
    /// </summary>
    public partial class ForgotPasswordWindow : Window
    {
        public ForgotPasswordWindow()
        {
            InitializeComponent();
        }

        private string currentEmail = "";
        private DateTime otpGeneratedTime;
        private DispatcherTimer otpTimer;
        private TimeSpan otpTimeLeft;



        private void SendOTP_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập email.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string email = txtEmail.Text.Trim();
            AccountDAO dao = new AccountDAO();

            if (!dao.IsExistsEmail(email))
            {
                MessageBox.Show("Email không tồn tại trong hệ thống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            currentEmail = txtEmail.Text.Trim();
            generatedOTP = GenerateOTP();
            otpGeneratedTime = DateTime.Now;
            SendOTPEmail(currentEmail, generatedOTP);
            StartOTPTimer();

            MessageBox.Show("Mã OTP đã được gửi đến email của bạn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            EmailPanel.Visibility = Visibility.Collapsed;
            OTPPanel.Visibility = Visibility.Visible;
        }



        private string generatedOTP = "";

        private void SendOTPEmail(string email, string otp)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("nguyenvantrong3254@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Mã OTP khôi phục mật khẩu";
                mail.Body = $"Mã OTP của bạn là: {otp}";

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.Credentials = new NetworkCredential("nguyenvantrong3254@gmail.com", "lyjq wycq ytcp dlfw");
                smtpClient.EnableSsl = true;

                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể gửi email. Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private string GenerateOTP()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999).ToString();
        }


        private void VerifyOTP_Click(object sender, RoutedEventArgs e)
        {
            if (DateTime.Now > otpGeneratedTime.AddMinutes(5))
            {
                MessageBox.Show("Mã OTP đã hết hạn. Vui lòng gửi lại OTP mới.", "Hết hạn", MessageBoxButton.OK, MessageBoxImage.Warning);
                OTPPanel.Visibility = Visibility.Collapsed;
                EmailPanel.Visibility = Visibility.Visible;
                return;
            }

            if (txtOTP.Text == generatedOTP)
            {
                OTPPanel.Visibility = Visibility.Collapsed;
                PasswordPanel.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Mã OTP không đúng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            string newPass = txtNewPassword.Password;
            string confirmPass = txtConfirmPassword.Password;

            if (string.IsNullOrWhiteSpace(newPass) || string.IsNullOrWhiteSpace(confirmPass))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("Mật khẩu không khớp.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AccountDAO dao = new AccountDAO();
            bool success = dao.ResetPassword(currentEmail, newPass);

            if (success)
            {
                MessageBox.Show("Mật khẩu đã được cập nhật. Vui lòng đăng nhập lại.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
                new LoginWindow().ShowDialog();
            }
            else
            {
                MessageBox.Show("Không thể cập nhật mật khẩu. Vui lòng kiểm tra lại email.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            LoginWindow lg = new LoginWindow();
            lg.ShowDialog();
        }


        private void StartOTPTimer()
        {
            otpTimeLeft = TimeSpan.FromMinutes(5);

            otpTimer = new DispatcherTimer();
            otpTimer.Interval = TimeSpan.FromSeconds(1);
            otpTimer.Tick += OtpTimer_Tick;
            otpTimer.Start();

            UpdateOTPTimerLabel();
        }

        private void OtpTimer_Tick(object sender, EventArgs e)
        {
            otpTimeLeft = otpTimeLeft.Subtract(TimeSpan.FromSeconds(1));

            if (otpTimeLeft <= TimeSpan.Zero)
            {
                otpTimer.Stop();
                lblOtpCountdown.Content = "Mã OTP đã hết hạn!";
            }
            else
            {
                UpdateOTPTimerLabel();
            }
        }

        private void UpdateOTPTimerLabel()
        {
            lblOtpCountdown.Content = $"Mã OTP sẽ hết hạn sau: {otpTimeLeft.Minutes:D2}:{otpTimeLeft.Seconds:D2}";
        }


        private void imgNewEye_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtNewPasswordVisible.Text = txtNewPassword.Password;
            txtNewPasswordVisible.Visibility = Visibility.Visible;
            txtNewPassword.Visibility = Visibility.Collapsed;
            imgNewCloseEye.Visibility = Visibility.Collapsed;
            imgNewEye.Visibility = Visibility.Visible;
        }

        private void imgNewCloseEye_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtNewPassword.Password = txtNewPasswordVisible.Text;
            txtNewPassword.Visibility = Visibility.Visible;
            txtNewPasswordVisible.Visibility = Visibility.Collapsed;
            imgNewCloseEye.Visibility = Visibility.Visible;
            imgNewEye.Visibility = Visibility.Collapsed;
        }

        private void imgConfirmEye_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtConfirmPasswordVisible.Text = txtConfirmPassword.Password;
            txtConfirmPasswordVisible.Visibility = Visibility.Visible;
            txtConfirmPassword.Visibility = Visibility.Collapsed;
            imgConfirmCloseEye.Visibility = Visibility.Collapsed;
            imgConfirmEye.Visibility = Visibility.Visible;
        }

        private void imgConfirmCloseEye_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtConfirmPassword.Password = txtConfirmPasswordVisible.Text;
            txtConfirmPassword.Visibility = Visibility.Visible;
            txtConfirmPasswordVisible.Visibility = Visibility.Collapsed;
            imgConfirmCloseEye.Visibility = Visibility.Visible;
            imgConfirmEye.Visibility = Visibility.Collapsed;
        }
    }
}
