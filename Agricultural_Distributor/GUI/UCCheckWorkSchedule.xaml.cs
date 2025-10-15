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
    /// <summary>
    /// Interaction logic for UCCheckWorkSchedule.xaml
    /// </summary>
    public partial class UCCheckWorkSchedule : UserControl
    {
        internal string result = "";
        DateTime load_date = DateTime.Now;


        WDHome wDHome;
        public UCCheckWorkSchedule(WDHome wDHome)
        {
            InitializeComponent();
            this.wDHome = wDHome;
            dtDateScheduleSearch.SelectedDate = DateTime.Today;
            btnAdd.Visibility = Visibility.Hidden;
            lvProducts.Visibility = Visibility.Hidden;
            LoadSchedule(load_date);
        }
        private List<Entity.Schedule> schedules = new();
        private void LoadSchedule(DateTime load_date)
        {
            if (load_date.Date != DateTime.Now.Date)
            {
                btnAddEmployee.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Hidden;
                btnChupAnhMC.Visibility = Visibility.Hidden;
            }
            else
            {
                btnAddEmployee.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Visible;
                btnChupAnhMC.Visibility = Visibility.Visible;
            }
            ScheduleDAO scheduleDAO = new ScheduleDAO();
            schedules = scheduleDAO.LoadSchedule(load_date);
            lvSchedules.ItemsSource = schedules;
        }
        int empId = 0;
        DateTime date;
        string imagePath;
        bool isSelect = false;
        private void lvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BitmapImage bitmapImage = new BitmapImage();
            imagePath = "";
            if (Uri.IsWellFormedUriString(imagePath, UriKind.Absolute))
            {
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(imagePath, UriKind.Absolute);
                bitmapImage.EndInit();
            }
            else
            {
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
                bitmapImage.EndInit();
            }
            CapturedImage.Source = bitmapImage;


            if (lvSchedules.SelectedItem is Schedule selectedSchedule)
            {
                isSelect = true;
                empId = selectedSchedule.EmployeeId;
                date = selectedSchedule.DayWork;
                lbManv.Content = selectedSchedule.EmployeeId;
                lbNamenv.Content = selectedSchedule.EmployeeName;
                txtTimeCheckIn.Text = selectedSchedule.TimeCheckIn.ToString(@"hh\:mm");
                txtTimeCheckOut.Text = selectedSchedule.TimeCheckOut.ToString(@"hh\:mm");
                imagePath = selectedSchedule.LinkPicture;
                ShowImageFromLink();
            }
        }
        public void ShowImageFromLink()
        {
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                if (Uri.IsWellFormedUriString(imagePath, UriKind.Absolute))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmapImage.EndInit();
                }
                else
                {
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
                    bitmapImage.EndInit();
                }
                CapturedImage.Source = bitmapImage;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Không thể tải hình ảnh: " + ex.Message);
            }
        }
        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (dtDateScheduleSearch.SelectedDate.Value == DateTime.Today)               
            {
                btnChupAnhMC.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Hidden;
                lvSchedules.Visibility = Visibility.Hidden;
                btnAddEmployee.Visibility = Visibility.Hidden;
                btnAdd.Visibility = Visibility.Visible;
                lvProducts.Visibility = Visibility.Visible;
                LoadEmployee();
            }
            else
            {
                MessageBox.Show("Chỉ có thể thêm nhân viên làm việc trong ngày hôm nay.");
            }

        }
        private List<Entity.Employee> employees = new();
        private void LoadEmployee()
        {
            EmployeeDAO employeeDAO = new EmployeeDAO();
            employees = employeeDAO.LoadEmployee();

            lvProducts.ItemsSource = employees;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            btnChupAnhMC.Visibility = Visibility.Visible;
            btnUpdate.Visibility = Visibility.Visible;
            lvSchedules.Visibility = Visibility.Visible;
            btnAddEmployee.Visibility = Visibility.Visible;
            btnAdd.Visibility = Visibility.Hidden;
            lvProducts.Visibility = Visibility.Hidden;

            DateTime dayWork = DateTime.Today;
            TimeSpan timeCheckIn = new TimeSpan(7, 0, 0);  // 07:00
            TimeSpan timeCheckOut = new TimeSpan(17, 0, 0);

            var selectedEmployees = employees.Where(emp => emp.IsSelected).ToList();

            ScheduleDAO dao = new ScheduleDAO();

            foreach (var emp in selectedEmployees)
            {
                Schedule sch = new Schedule(emp.EmployeeId, dayWork, new DateTime(dayWork.Year, dayWork.Month, dayWork.Day,
                                               timeCheckIn.Hours, timeCheckIn.Minutes, 0).TimeOfDay,
                                               new DateTime(dayWork.Year, dayWork.Month, dayWork.Day,
                                                timeCheckOut.Hours, timeCheckOut.Minutes, 0).TimeOfDay,
                                               "");
                dao.InsertSchedule(sch);
            }

            LoadSchedule(load_date);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScheduleDAO schedule = new ScheduleDAO();
            TimeSpan timeCheckIn = TimeSpan.Parse(txtTimeCheckIn.Text);
            TimeSpan timeCheckOut = TimeSpan.Parse(txtTimeCheckOut.Text);

            if (date.Date == DateTime.Now.Date)
            {
                if (timeCheckOut >= timeCheckIn) schedule.updateSchedule(empId, date, timeCheckIn, timeCheckOut);
                else MessageBox.Show("Checkin hoặc Checkout không hợp lệ!");
            }
            else
            {
                MessageBox.Show("Chỉ có thể chỉnh sửa lịch làm việc trong ngày hôm nay.");
            }

            LoadSchedule(load_date);
        }

        private void btnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            DateTime load_date = dtDateScheduleSearch.SelectedDate.Value;
            
                LoadSchedule(load_date);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //chup anh
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ScheduleDAO scheduleDAO = new ScheduleDAO();
            if(isSelect)
            {
                UCPictureShot uCCreateOrder = new UCPictureShot(wDHome, empId, date);
                wDHome.GetUC(uCCreateOrder);
            }
            else
            {
                MessageBox.Show("Chưa chọn nhân viên muốn chụp ảnh");
            }
        }
    }
}
