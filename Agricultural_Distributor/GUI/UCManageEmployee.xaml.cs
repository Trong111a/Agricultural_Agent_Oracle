using Agricultural_Distributor.Common;
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
    /// Interaction logic for UCManageEmployee.xaml
    /// </summary>
    public partial class UCManageEmployee : UserControl
    {
        internal string result = "";

        WDHome wDHome;
        int selectedPosition = -1;
        public UCManageEmployee(WDHome wDHome)
        {
            InitializeComponent();
            this.wDHome = wDHome;
            if (SessionManager.IsAdmin == true)
            {
                cbPosition.Visibility = Visibility.Visible;
                lbPosition.Visibility = Visibility.Visible;
                cbPosition.Text = "";
            }
            else
            {
                cbPosition.Visibility = Visibility.Hidden;
                lbPosition.Visibility = Visibility.Hidden;
                delete.Visibility = Visibility.Hidden;
                unlock.Visibility = Visibility.Hidden;

            }

            LoadEmployee();
        }
        private List<Entity.Employee> employees = new();
        private void LoadEmployee()
        {
            EmployeeDAO employeeDAO = new EmployeeDAO();
            employees = employeeDAO.LoadEmployee();

            lvProducts.ItemsSource = employees;
        }

        private void btnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            UCCheckWorkSchedule uCCreateOrder = new UCCheckWorkSchedule(wDHome);
            wDHome.GetUC(uCCreateOrder);
        }
        int empId = -1;
        private void lvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvProducts.SelectedItem is Employee selectedEmployee)
            {
                empId = selectedEmployee.EmployeeId;
                txtEmployeeName.Text = selectedEmployee.EmployeeName;
                txtSex.Text = selectedEmployee.Sex;
                txtBirthday.SelectedDate = selectedEmployee.Birthday;

                txtAddress.Text = selectedEmployee.EmployeeAddress;
                txtPhoneNumber.Text = selectedEmployee.PhoneNumber;
                txtEmail.Text = selectedEmployee.Email;
                selectedPosition = selectedEmployee.Position;
                if (selectedEmployee.Position == 1) cbPosition.Text = "Quản lý";
                else cbPosition.Text = "Nhân viên";
            }
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Employee em = new Employee(empId, txtEmployeeName.Text, txtBirthday.SelectedDate.Value, txtSex.Text, txtAddress.Text, txtPhoneNumber.Text, txtEmail.Text);
            EmployeeDAO employeeDAO = new EmployeeDAO();
            employeeDAO.addEmployee(em, cbPosition.Text);
            LoadEmployee();
        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            Employee em = new Employee(empId, txtEmployeeName.Text, txtBirthday.SelectedDate.Value, txtSex.Text, txtAddress.Text, txtPhoneNumber.Text, txtEmail.Text);
            EmployeeDAO employeeDAO = new EmployeeDAO();
            employeeDAO.updateEmployee(em);
            LoadEmployee();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            EmployeeDAO employeeDAO = new EmployeeDAO();
            employeeDAO.deleteEmployee(empId);
            employeeDAO.deleteAccount(empId);
            LoadEmployee();
        }

        private void unlock_Click(object sender, RoutedEventArgs e)
        {
            if (empId == -1)
            {
                MessageBox.Show("Vui lòng chọn một nhân viên để mở khóa.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedPosition != 1)
            {
                MessageBox.Show("Chức năng mở khóa chỉ được áp dụng cho tài khoản Quản lý.", "Lỗi ủy quyền", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Bạn có chắc chắn muốn mở khóa tài khoản Quản lý ID: {empId} không?",
                "Xác nhận Mở Khóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    EmployeeDAO employeeDAO = new EmployeeDAO();

                    employeeDAO.UnlockAccount(empId);

                    LoadEmployee();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể mở khóa tài khoản: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void ClearForm()
        {
            empId = -1;
            selectedPosition = -1;
            txtEmployeeName.Text = string.Empty;
            txtSex.Text = string.Empty;
            txtBirthday.SelectedDate = null;
            txtAddress.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty;
            txtEmail.Text = string.Empty;
            cbPosition.Text = string.Empty;
            lvProducts.SelectedItem = null;
        }
    }
}
