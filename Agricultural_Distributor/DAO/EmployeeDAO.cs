using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agricultural_Distributor.Entity;
using System.Windows;
using Oracle.ManagedDataAccess.Client;
using Agricultural_Distributor.Common;

namespace Agricultural_Distributor.DAO
{
    internal class EmployeeDAO
    {
        //connect connect = new();
        Connect connect = SessionManager.Connect;
        Employee employee;

        public EmployeeDAO() { }

        public EmployeeDAO(Employee employee)
        {
            this.employee = employee;
        }

        public List<Employee> LoadEmployee()
        {
            List<Employee> employees = new();
            //connect.Connect();
            connect.ConnectDB();

            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;

   
            //oraCmd.CommandText = "SELECT employeeId, employeeName, birthday, sex, employeeAddress, phoneNumber, email, IsActive, position " +
            //                     "FROM Employee WHERE IsActive = 1 AND employeeId <> 1";
            oraCmd.CommandText = "SELECT employeeId, employeeName, birthday, sex, employeeAddress, phoneNumber, email, IsActive, position FROM AGRICULTURAL_AGENT.Employee WHERE employeeId <> 1";


            oraCmd.Connection = connect.oraCon;

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
                int isActive = reader.GetInt32(7);
                int position = reader.GetInt32(8);


                Employee employee = new(EmployeeId, EmployeeName, Birthday, Sex, EmployeeAddress, PhoneNumber, Email, position);
                employees.Add(employee);
                employee.IsActive = isActive;
            }
            reader.Close();
            connect.Disconnect();
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

                connect.ConnectDB();

                OracleCommand oraCmd = new();
                oraCmd.CommandType = CommandType.Text;

                oraCmd.CommandText = "INSERT INTO AGRICULTURAL_AGENT.Employee (employeeName, birthday, sex, employeeAddress, phoneNumber, email, isActive, position) " +
                                     "VALUES (:employeeName, :birthday, :sex, :employeeAddress, :phoneNumber, :email, :isActive, :position)";

                oraCmd.Connection = connect.oraCon;
                oraCmd.Parameters.Add("employeeName", employee.EmployeeName);
                oraCmd.Parameters.Add("birthday", employee.Birthday);
                oraCmd.Parameters.Add("sex", employee.Sex);
                oraCmd.Parameters.Add("employeeAddress", employee.EmployeeAddress);
                oraCmd.Parameters.Add("phoneNumber", employee.PhoneNumber);
                oraCmd.Parameters.Add("email", employee.Email);
                oraCmd.Parameters.Add("isActive", 1);

                int positionValue = (position == "Quản lý") ? 1 : 2;
                oraCmd.Parameters.Add("position", positionValue);

                int rowsAffected = oraCmd.ExecuteNonQuery();
                connect.Disconnect();

                MessageBox.Show("Thêm nhân viên thành công");
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm nhân viên: " + ex.Message);
                connect.Disconnect();
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

                connect.ConnectDB();

                string query = "UPDATE AGRICULTURAL_AGENT.Employee SET " +
                               "employeeName = :EmployeeName, " +
                               "birthday = :Birthday, " +
                               "sex = :Sex, " +
                               "employeeAddress = :EmployeeAddress, " +
                               "phoneNumber = :PhoneNumber, " +
                               "email = :Email " +
                               //"isActive" = "1"+
                               "WHERE employeeId = :EmployeeId";

                OracleCommand oraCmd = new(query, connect.oraCon);

                //oraCmd.Parameters.Add("EmployeeId", Convert.ToInt32(employee.EmployeeId));
                //oraCmd.Parameters.Add("EmployeeName", employee.EmployeeName);
                //oraCmd.Parameters.Add("Birthday", employee.Birthday);
                //oraCmd.Parameters.Add("Sex", employee.Sex);
                //oraCmd.Parameters.Add("EmployeeAddress", employee.EmployeeAddress);
                //oraCmd.Parameters.Add("PhoneNumber", employee.PhoneNumber);
                //oraCmd.Parameters.Add("Email", employee.Email);
                
                oraCmd.Parameters.Add("EmployeeName", OracleDbType.NVarchar2).Value = employee.EmployeeName;
                oraCmd.Parameters.Add("Birthday", OracleDbType.Date).Value = employee.Birthday;
                oraCmd.Parameters.Add("Sex", OracleDbType.NVarchar2).Value = employee.Sex;
                oraCmd.Parameters.Add("EmployeeAddress", OracleDbType.NVarchar2).Value = employee.EmployeeAddress;
                oraCmd.Parameters.Add("PhoneNumber", OracleDbType.Varchar2).Value = employee.PhoneNumber;
                oraCmd.Parameters.Add("Email", OracleDbType.Varchar2).Value = employee.Email;
                oraCmd.Parameters.Add("EmployeeId", OracleDbType.Int32).Value = employee.EmployeeId;


                int rowsAffected = oraCmd.ExecuteNonQuery();
                connect.Disconnect();

                MessageBox.Show("Cập nhật thông tin nhân viên thành công");
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật nhân viên: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                connect.Disconnect();
                return false;
            }
        }

        public bool deleteEmployee(int employeeId) 
        {
            try
            {
                connect.ConnectDB();

                string query = "UPDATE AGRICULTURAL_AGENT.Employee SET IsActive = 0 WHERE employeeId = :employeeId";

                using (OracleCommand oraCmd = new(query, connect.oraCon))
                {
                    oraCmd.Parameters.Add("employeeId", employeeId);

          
                    int rowsAffected = oraCmd.ExecuteNonQuery();
                    connect.Disconnect();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật trạng thái nhân viên: " + ex.Message);
                connect.Disconnect();
                return false;
            }
        }

        //public bool deleteAccount(int id)
        //{
        //    try
        //    {
        //        connect.ConnectDB();

        //        string query = "UPDATE AGRICULTURAL_AGENT.Account SET IsActive = 0 WHERE Id = :id";

        //        using (OracleCommand oraCmd = new(query, connect.oraCon))
        //        {
        //            oraCmd.Parameters.Add("Id", id);


        //            int rowsAffected = oraCmd.ExecuteNonQuery();
        //            connect.Disconnect();
        //            return rowsAffected > 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Lỗi khi cập nhật trạng thái tài khoản: " + ex.Message);
        //        connect.Disconnect();
        //        return false;
        //    }
        //}
        public bool deleteAccount(int id)
        {
            string username = null;
            OracleConnection oraCon = connect.oraCon;
            OracleTransaction transaction = null;

            try
            {
                connect.ConnectDB();
                transaction = oraCon.BeginTransaction();

                string querySelect = "SELECT username FROM AGRICULTURAL_AGENT.Account WHERE Id = :id";
                using (OracleCommand cmdSelect = new OracleCommand(querySelect, oraCon))
                {
                    cmdSelect.Transaction = transaction;
                    cmdSelect.Parameters.Add("id", id);
                    object result = cmdSelect.ExecuteScalar();
                    if (result != null)
                    {
                        username = result.ToString();
                    }
                }

                string queryUpdate = "UPDATE AGRICULTURAL_AGENT.Account SET IsActive = 0 WHERE Id = :id";
                int rowsAffected = 0;
                using (OracleCommand cmdUpdate = new OracleCommand(queryUpdate, oraCon))
                {
                    cmdUpdate.Transaction = transaction;
                    cmdUpdate.Parameters.Add("id", id);
                    rowsAffected = cmdUpdate.ExecuteNonQuery();
                }


                if (!string.IsNullOrEmpty(username))
                {
                    string lockQuery = "BEGIN AGRICULTURAL_AGENT.LockUserAccount(:p_username); END;";
                    using (OracleCommand cmdLock = new OracleCommand(lockQuery, oraCon))
                    {
                        cmdLock.Transaction = transaction;
                        cmdLock.Parameters.Add("p_username", username.ToUpper());
                        cmdLock.CommandType = CommandType.Text;
                        cmdLock.ExecuteNonQuery();
                    }
                }


                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception exRollback)
                    {
                        MessageBox.Show("Lỗi khi Rollback giao dịch: " + exRollback.Message);
                    }
                }
                MessageBox.Show("Lỗi khi vô hiệu hóa tài khoản và khóa Oracle: " + ex.Message);
                return false;
            }
            finally
            {
                connect.Disconnect();
            }
        }



        public bool UnlockAccount(int id)
        {
            string username = null;
            OracleConnection oraCon = null;
            OracleTransaction transaction = null;

            try
            {
                connect.ConnectDB();
                oraCon = connect.oraCon;

                if (oraCon == null || oraCon.State != ConnectionState.Open)
                {
                    throw new Exception("Không thể thiết lập kết nối cơ sở dữ liệu (Kết nối null hoặc chưa mở).");
                }

                transaction = oraCon.BeginTransaction();

                string querySelect = "SELECT username FROM AGRICULTURAL_AGENT.Account WHERE Id = :id";
                using (OracleCommand cmdSelect = new OracleCommand(querySelect, oraCon))
                {
                    cmdSelect.Transaction = transaction;
                    cmdSelect.Parameters.Add("id", id);
                    object result = cmdSelect.ExecuteScalar();
                    if (result != null)
                    {
                        username = result.ToString();
                    }
                }

                if (string.IsNullOrEmpty(username))
                {
                    throw new Exception("Không tìm thấy tài khoản để mở khóa.");
                }

                string queryUpdateEmployee = "UPDATE AGRICULTURAL_AGENT.Employee SET IsActive = 1 WHERE employeeId = :id";
                using (OracleCommand cmdUpdateEmployee = new OracleCommand(queryUpdateEmployee, oraCon))
                {
                    cmdUpdateEmployee.Transaction = transaction;
                    cmdUpdateEmployee.Parameters.Add("id", id);
                    cmdUpdateEmployee.ExecuteNonQuery();
                }

                string queryUpdateAccount = "UPDATE AGRICULTURAL_AGENT.Account SET IsActive = 1 WHERE Id = :id";
                using (OracleCommand cmdUpdateAccount = new OracleCommand(queryUpdateAccount, oraCon))
                {
                    cmdUpdateAccount.Transaction = transaction;
                    cmdUpdateAccount.Parameters.Add("id", id);
                    cmdUpdateAccount.ExecuteNonQuery();
                }

                string unlockQuery = "BEGIN AGRICULTURAL_AGENT.UnlockUserAccount(:p_username); END;";
                using (OracleCommand cmdUnlock = new OracleCommand(unlockQuery, oraCon))
                {
                    cmdUnlock.Transaction = transaction;
                    cmdUnlock.Parameters.Add("p_username", username.ToUpper());
                    cmdUnlock.CommandType = CommandType.Text;
                    cmdUnlock.ExecuteNonQuery();
                }

                transaction.Commit();
                MessageBox.Show("Mở khóa tài khoản thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception exRollback)
                    {
                        MessageBox.Show("Lỗi khi Rollback giao dịch mở khóa: " + exRollback.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                throw new Exception("Lỗi khi mở khóa tài khoản và Oracle: " + ex.Message);
            }
            finally
            {
                connect.Disconnect();
            }
        }
    }
}