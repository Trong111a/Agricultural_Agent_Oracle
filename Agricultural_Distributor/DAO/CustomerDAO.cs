using Agricultural_Distributor.Common;
using Agricultural_Distributor.Entity;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace Agricultural_Distributor.DAO
{
    internal class CustomerDAO
    {
        //connect connect = new connect();
        Connect connect = SessionManager.Connect;
        Customer customer;

        public CustomerDAO() { }

        public CustomerDAO(Customer customer)
        {
            this.customer = customer;
        }

        public Customer? GetCustomer(string phone)
        {
            Customer customer = new();
            connect.ConnectDB();

            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;

            oraCmd.CommandText = "select customerId, customerName, customerAddress, phoneNumber, email from AGRICULTURAL_AGENT.Customer where phoneNumber=:phoneNumber";

            oraCmd.Parameters.Add("phoneNumber", phone);

            oraCmd.Connection = connect.oraCon;

            OracleDataReader reader = oraCmd.ExecuteReader();
            if (reader.Read())
            {

                int id = reader.GetInt32(0);

                string name = reader.GetString(1);
                string address = reader.GetString(2);
                string phoneCus = reader.GetString(3);

                string email = reader.IsDBNull(4) ? null : reader.GetString(4);

                customer.CustomerId = id;
                customer.CustomerName = name;
                customer.CustomerAddress = address;
                customer.PhoneNumber = phoneCus;
                customer.Email = email;
            }
            else customer = null;
            reader.Close();
            connect.Disconnect();
            return customer;
        }

        public int AddCustomer(Customer customer)
        {
            connect.ConnectDB();

            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;

            oraCmd.CommandText =
                "INSERT INTO AGRICULTURAL_AGENT.Customer (customerName, customerAddress, phoneNumber, email) " +
                "VALUES (:name, :address, :phone, :email) RETURNING customerId INTO :customerId";

            oraCmd.Parameters.Add("name", OracleDbType.NVarchar2).Value =  customer.CustomerName;
            oraCmd.Parameters.Add("address", OracleDbType.Varchar2).Value =  customer.CustomerAddress;
            oraCmd.Parameters.Add("phone", OracleDbType.Varchar2).Value =  customer.PhoneNumber;
            oraCmd.Parameters.Add("email", OracleDbType.Varchar2).Value =  customer.Email;

            OracleParameter outputIdParam = new OracleParameter("customerId", OracleDbType.Decimal, ParameterDirection.Output);
            oraCmd.Parameters.Add(outputIdParam);

            oraCmd.Connection = connect.oraCon;

            try
            {
                oraCmd.ExecuteNonQuery();

                int newCustomerId = 0;
                if (outputIdParam.Value != DBNull.Value)
                {
                    newCustomerId = ((OracleDecimal)outputIdParam.Value).ToInt32();
                }

                connect.Disconnect();
                return newCustomerId; 

            }
            catch (OracleException ex)
            {
                MessageBox.Show("Lỗi khi thêm khách hàng: " + ex.Message);
                connect.Disconnect();
                return 0; 
            }
        }

        public bool UpdateCustomer(Customer customer)
        {
            connect.ConnectDB();

            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "update AGRICULTURAL_AGENT.Customer set customerName = :name, customerAddress = :address, email = :email where customerId = :id";

            oraCmd.Parameters.Add("name", customer.CustomerName);
            oraCmd.Parameters.Add("address", customer.CustomerAddress);
            oraCmd.Parameters.Add("email", customer.Email);
            oraCmd.Parameters.Add("id", customer.CustomerId);

            oraCmd.Connection = connect.oraCon;

            try
            {
                int result = oraCmd.ExecuteNonQuery();
                connect.Disconnect();
                return result > 0;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                connect.Disconnect();
                return false;
            }
        }

        public List<Customer> LoadCustomer()
        {
            List<Customer> listCustomer = new List<Customer>();
            connect.ConnectDB();

            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "select customerId, customerName, customerAddress, phoneNumber, email from AGRICULTURAL_AGENT.Customer";
            oraCmd.Connection = connect.oraCon;

            OracleDataReader reader = oraCmd.ExecuteReader();
            while (reader.Read())
            {
                int customerId = reader.GetInt32(0);
                string customerName = reader.GetString(1);
                string customerAddress = reader.GetString(2);
                string phoneNumber = reader.GetString(3);
                string email = reader.IsDBNull(4) ? null : reader.GetString(4);

                Customer customer = new Customer(customerId, customerName, customerAddress, phoneNumber, email);
                listCustomer.Add(customer);
            }
            reader.Close();
            connect.Disconnect();
            return listCustomer;
        }

        public Customer GetCustomerByID(int customerId) 
        {
            Customer customer = new Customer();
            connect.ConnectDB();

            OracleCommand oraCmd = new OracleCommand();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "select customerId, customerName, customerAddress, phoneNumber, email from AGRICULTURAL_AGENT.Customer where customerId = :customerId";
            oraCmd.Connection = connect.oraCon;
            oraCmd.Parameters.Add("customerId", customerId);

            OracleDataReader reader = oraCmd.ExecuteReader();
            if (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                string address = reader.GetString(2);
                string phoneCus = reader.GetString(3);
                string email = reader.IsDBNull(4) ? null : reader.GetString(4);

                customer.CustomerId = id;
                customer.CustomerName = name;
                customer.CustomerAddress = address;
                customer.PhoneNumber = phoneCus;
                customer.Email = email;
            }
            reader.Close();
            connect.Disconnect();
            return customer;
        }

        public string ValidInput(Customer customer)
        {
            if (!Regex.IsMatch(customer.PhoneNumber, @"^\d{10}$"))
            {
                return "Số điện thoại phải gồm đúng 10 chữ số!";
            }
            if (customer.Email != null && !Regex.IsMatch(customer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return "Email không hợp lệ!";
            }

            string phoneExist = CheckExistPhone(customer);
            if (phoneExist != null)
            {
                return phoneExist;
            }
            string emailExist = CheckExistEmail(customer);
            if (emailExist != null)
            {
                return emailExist;
            }
            return null;
        }

        public string CheckExistPhone(Customer customer)
        {
            connect.ConnectDB();
            OracleCommand oraCmd = new OracleCommand();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "select count (*) from AGRICULTURAL_AGENT.Customer where phoneNumber = :phone and customerId != :id";
            oraCmd.Connection = connect.oraCon;

            oraCmd.Parameters.Add("phone", customer.PhoneNumber);
            oraCmd.Parameters.Add("id", customer.CustomerId);

            object result = oraCmd.ExecuteScalar();
            int count = Convert.ToInt32(result);

            connect.Disconnect();
            if (count > 0)
            {
                return "Số điện thoại đã tồn tại cho 1 khách hàng khác";
            }
            return null;
        }

        public string CheckExistEmail(Customer customer)
        {
            if (customer.Email == null) return null; 

            connect.ConnectDB();
            OracleCommand oraCmd = new OracleCommand();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "select count (*) from AGRICULTURAL_AGENT.Customer where email = :email and customerId != :id";
            oraCmd.Connection = connect.oraCon;

            oraCmd.Parameters.Add("email", customer.Email);
            oraCmd.Parameters.Add("id", customer.CustomerId);

            object result = oraCmd.ExecuteScalar();
            int count = Convert.ToInt32(result);

            connect.Disconnect();
            if (count > 0)
            {
                return "Email đã tồn tại cho 1 khách hàng khác";
            }
            return null;
        }

        public List<Customer> SearchCustomer(string keyword)
        {
            List<Customer> list = new List<Customer>();
            string searchParam = "%" + keyword.Trim() + "%";

            string sql =
                "SELECT CUSTOMERID, CUSTOMERNAME, CUSTOMERADDRESS, PHONENUMBER, EMAIL FROM AGRICULTURAL_AGENT.CUSTOMER " +
                "WHERE UPPER(CUSTOMERNAME) LIKE UPPER(:keyword) " +
                "OR PHONENUMBER LIKE :keyword " +
                "OR CUSTOMERID = TO_NUMBER(CASE WHEN REGEXP_LIKE(:keyword, '^[0-9]+$') THEN :keyword ELSE '-1' END)";

            try
            {
                connect.ConnectDB();

                using (OracleCommand oraCmd = new OracleCommand())
                {
                    oraCmd.Connection = connect.oraCon;
                    oraCmd.CommandType = CommandType.Text;
                    oraCmd.CommandText = sql;

                    oraCmd.Parameters.Add("keyword", searchParam);

                    OracleDataReader reader = oraCmd.ExecuteReader();

                    int customerNameOrd = reader.GetOrdinal("CUSTOMERNAME");
                    int customerAddressOrd = reader.GetOrdinal("CUSTOMERADDRESS");
                    int phoneNumberOrd = reader.GetOrdinal("PHONENUMBER");
                    int emailOrd = reader.GetOrdinal("EMAIL");

                    while (reader.Read())
                    {
                        Customer newCustomer = new Customer()
                        {
                            CustomerId = reader.GetInt32(0),
                            CustomerName = reader.IsDBNull(customerNameOrd)
                                ? string.Empty
                                : reader.GetString(customerNameOrd),

                            CustomerAddress = reader.IsDBNull(customerAddressOrd)
                                ? string.Empty
                                : reader.GetString(customerAddressOrd),

                            PhoneNumber = reader.IsDBNull(phoneNumberOrd)
                                ? string.Empty
                                : reader.GetString(phoneNumberOrd),

                            Email = reader.IsDBNull(emailOrd)
                                ? null
                                : reader.GetString(emailOrd),
                        };
                        list.Add(newCustomer);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tìm kiếm khách hàng: {ex.Message}");
            }
            finally
            {
                connect.Close();
            }
            return list;
        }

        public bool CheckExist(string email)
        {
            connect.ConnectDB();

            OracleCommand oraCmd = new OracleCommand();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "select 1 from AGRICULTURAL_AGENT.Customer where email = :email";
            oraCmd.Parameters.Add("email", email);
            oraCmd.Connection = connect.oraCon;

            OracleDataReader reader = oraCmd.ExecuteReader();
            bool exists = reader.Read();
            reader.Close();
            connect.Disconnect();
            return exists;
        }
    }
}