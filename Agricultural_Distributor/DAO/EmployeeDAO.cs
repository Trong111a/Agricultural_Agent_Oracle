using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agricultural_Distributor.Entity;
using System.Windows;
using Oracle.ManagedDataAccess.Client;

namespace Agricultural_Distributor.DAO
{
    internal class EmployeeDAO
    {
        ConnectOracle connectOracle = new();
        Employee employee;

        public EmployeeDAO() { }

        public EmployeeDAO(Employee employee)
        {
            this.employee = employee;
        }

        public List<Employee> LoadEmployee()
        {
            List<Employee> employees = new();
            connectOracle.Connect();

            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;

   
            oraCmd.CommandText = "SELECT employeeId, employeeName, birthday, sex, employeeAddress, phoneNumber, email, IsActive, position " +
                                 "FROM Employee WHERE IsActive = 1 AND employeeId <> 1";


            oraCmd.Connection = connectOracle.oraCon;

            OracleDataReader reader = oraCmd.ExecuteReader();
            while (reader.Read())
            {
                int EmployeeId = reader.GetInt32(0);
                string EmployeeName = reader.GetString(1);
                DateTime Birthday = reader.GetDateTime(2);
                string Sex = reader.GetString(3);
                string EmployeeAddress = reader.GetString(4);
                string PhoneNumber = reader.GetString(5);

                string Email = reader.IsDBNull(6) ? null : reader.GetString(6);


                Employee employee = new(EmployeeId, EmployeeName, Birthday, Sex, EmployeeAddress, PhoneNumber, Email);
                employees.Add(employee);
            }
            reader.Close();
            connectOracle.Disconnect();
            return employees;
        }

        public bool addEmployee(Employee employee, string position)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(employee.EmployeeName) ||
                    string.IsNullOrWhiteSpace(employee.EmployeeAddress) ||
                    string.IsNullOrWhiteSpace(employee.PhoneNumber) ||
                    string.IsNullOrWhiteSpace(employee.Email) ||
                    string.IsNullOrWhiteSpace(employee.Sex))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin nhân viên.");
                    return false;
                }

                connectOracle.Connect();

                OracleCommand oraCmd = new();
                oraCmd.CommandType = CommandType.Text;

                oraCmd.CommandText = "INSERT INTO Employee (employeeName, birthday, sex, employeeAddress, phoneNumber, email, isActive, position) " +
                                     "VALUES (:employeeName, :birthday, :sex, :employeeAddress, :phoneNumber, :email, :isActive, :position)";

                oraCmd.Connection = connectOracle.oraCon;
                oraCmd.Parameters.Add("employeeName", employee.EmployeeName);
                oraCmd.Parameters.Add("birthday", employee.Birthday);
                oraCmd.Parameters.Add("sex", employee.Sex);
                oraCmd.Parameters.Add("employeeAddress", employee.EmployeeAddress);
                oraCmd.Parameters.Add("phoneNumber", employee.PhoneNumber);
                oraCmd.Parameters.Add("email", employee.Email);
                oraCmd.Parameters.Add("isActive", 1);

                int positionValue = (position == "Quản lý") ? 1 : 0;
                oraCmd.Parameters.Add("position", positionValue);

                int rowsAffected = oraCmd.ExecuteNonQuery();
                connectOracle.Disconnect();

                MessageBox.Show("Thêm nhân viên thành công");
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm nhân viên: " + ex.Message);
                connectOracle.Disconnect();
                return false;
            }
        }


        public bool updateEmployee(Employee employee)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(employee.EmployeeName) ||
                    string.IsNullOrWhiteSpace(employee.EmployeeAddress) ||
                    string.IsNullOrWhiteSpace(employee.PhoneNumber) ||
                    string.IsNullOrWhiteSpace(employee.Email) ||
                    string.IsNullOrWhiteSpace(employee.Sex))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                connectOracle.Connect();

                string query = "UPDATE Employee SET " +
                               "employeeName = :EmployeeName, " +
                               "birthday = :Birthday, " +
                               "sex = :Sex, " +
                               "employeeAddress = :EmployeeAddress, " +
                               "phoneNumber = :PhoneNumber, " +
                               "email = :Email " +
                               "WHERE employeeId = :EmployeeId";

                OracleCommand oraCmd = new(query, connectOracle.oraCon);

                oraCmd.Parameters.Add("EmployeeId", employee.EmployeeId);
                oraCmd.Parameters.Add("EmployeeName", employee.EmployeeName);
                oraCmd.Parameters.Add("Birthday", employee.Birthday);
                oraCmd.Parameters.Add("Sex", employee.Sex);
                oraCmd.Parameters.Add("EmployeeAddress", employee.EmployeeAddress);
                oraCmd.Parameters.Add("PhoneNumber", employee.PhoneNumber);
                oraCmd.Parameters.Add("Email", employee.Email);

                int rowsAffected = oraCmd.ExecuteNonQuery();
                connectOracle.Disconnect();

                MessageBox.Show("Cập nhật thông tin nhân viên thành công");
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật nhân viên: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                connectOracle.Disconnect();
                return false;
            }
        }

        public bool deleteEmployee(int employeeId) 
        {
            try
            {
                connectOracle.Connect();

                string query = "UPDATE Employee SET IsActive = 0 WHERE employeeId = :employeeId";

                using (OracleCommand oraCmd = new(query, connectOracle.oraCon))
                {
                    oraCmd.Parameters.Add("employeeId", employeeId);

          
                    int rowsAffected = oraCmd.ExecuteNonQuery();
                    connectOracle.Disconnect();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật trạng thái nhân viên: " + ex.Message);
                connectOracle.Disconnect();
                return false;
            }
        }
    }
}