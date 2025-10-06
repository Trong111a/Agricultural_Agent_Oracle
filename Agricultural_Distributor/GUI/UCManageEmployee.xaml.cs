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

        public UCManageEmployee(WDHome wDHome)
        {
            InitializeComponent();
            this.wDHome = wDHome;
            if (SessionManager.IsAdmin == true)
            {
                cbPosition.Visibility = Visibility.Visible;
                lbPosition.Visibility = Visibility.Visible;
            }
            else
            {
                cbPosition.Visibility = Visibility.Hidden;
                lbPosition.Visibility = Visibility.Hidden;
            }
            cbPosition.Text = "";
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

                
            }
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Xử lý khi checkbox được check
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Xử lý khi checkbox bị bỏ chọn
        }

        private void TextBox_TextChanged()
        {

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
            LoadEmployee();
        }
    }
}
