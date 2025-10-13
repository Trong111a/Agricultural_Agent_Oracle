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
        ConnectOracle connectOracle = new ConnectOracle();
        Customer customer;

        public CustomerDAO() { }

        public CustomerDAO(Customer customer)
        {
            this.customer = customer;
        }

        public Customer? GetCustomer(string phone)
        {
            Customer customer = new();
            connectOracle.Connect();

            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;

            oraCmd.CommandText = "select customerId, customerName, customerAddress, phoneNumber, email from Customer where phoneNumber=:phoneNumber";

            oraCmd.Parameters.Add("phoneNumber", phone);

            oraCmd.Connection = connectOracle.oraCon;

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
            connectOracle.Disconnect();
            return customer;
        }

        public int AddCustomer(Customer customer)
        {
            connectOracle.Connect();

            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;

            oraCmd.CommandText =
                "INSERT INTO Customer (customerName, customerAddress, phoneNumber, email) " +
                "VALUES (:name, :address, :phone, :email) RETURNING customerId INTO :customerId";

            oraCmd.Parameters.Add("name", OracleDbType.NVarchar2).Value =  customer.CustomerName;
            oraCmd.Parameters.Add("address", OracleDbType.Varchar2).Value =  customer.CustomerAddress;
            oraCmd.Parameters.Add("phone", OracleDbType.Varchar2).Value =  customer.PhoneNumber;
            oraCmd.Parameters.Add("email", OracleDbType.Varchar2).Value =  customer.Email;

            OracleParameter outputIdParam = new OracleParameter("customerId", OracleDbType.Decimal, ParameterDirection.Output);
            oraCmd.Parameters.Add(outputIdParam);

            oraCmd.Connection = connectOracle.oraCon;

            try
            {
                oraCmd.ExecuteNonQuery();

                int newCustomerId = 0;
                if (outputIdParam.Value != DBNull.Value)
                {
                    newCustomerId = ((OracleDecimal)outputIdParam.Value).ToInt32();
                }

                connectOracle.Disconnect();
                return newCustomerId; 

            }
            catch (OracleException ex)
            {
                MessageBox.Show("Lỗi khi thêm khách hàng: " + ex.Message);
                connectOracle.Disconnect();
                return 0; 
            }
        }

        public bool UpdateCustomer(Customer customer)
        {
            connectOracle.Connect();

            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "update Customer set customerName = :name, customerAddress = :address, email = :email where customerId = :id";

            oraCmd.Parameters.Add("name", customer.CustomerName);
            oraCmd.Parameters.Add("address", customer.CustomerAddress);
            oraCmd.Parameters.Add("email", customer.Email);
            oraCmd.Parameters.Add("id", customer.CustomerId);

            oraCmd.Connection = connectOracle.oraCon;

            try
            {
                int result = oraCmd.ExecuteNonQuery();
                connectOracle.Disconnect();
                return result > 0;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                connectOracle.Disconnect();
                return false;
            }
        }

        public List<Customer> LoadCustomer()
        {
            List<Customer> listCustomer = new List<Customer>();
            connectOracle.Connect();

            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "select customerId, customerName, customerAddress, phoneNumber, email from Customer";
            oraCmd.Connection = connectOracle.oraCon;

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
            connectOracle.Disconnect();
            return listCustomer;
        }

        public Customer GetCustomerByID(int customerId) 
        {
            Customer customer = new Customer();
            connectOracle.Connect();

            OracleCommand oraCmd = new OracleCommand();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "select customerId, customerName, customerAddress, phoneNumber, email from Customer where customerId = :customerId";
            oraCmd.Connection = connectOracle.oraCon;
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
            connectOracle.Disconnect();
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
            connectOracle.Connect();
            OracleCommand oraCmd = new OracleCommand();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "select count (*) from Customer where phoneNumber = :phone and customerId != :id";
            oraCmd.Connection = connectOracle.oraCon;

            oraCmd.Parameters.Add("phone", customer.PhoneNumber);
            oraCmd.Parameters.Add("id", customer.CustomerId);

            object result = oraCmd.ExecuteScalar();
            int count = Convert.ToInt32(result);

            connectOracle.Disconnect();
            if (count > 0)
            {
                return "Số điện thoại đã tồn tại cho 1 khách hàng khác";
            }
            return null;
        }

        public string CheckExistEmail(Customer customer)
        {
            if (customer.Email == null) return null; 

            connectOracle.Connect();
            OracleCommand oraCmd = new OracleCommand();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "select count (*) from Customer where email = :email and customerId != :id";
            oraCmd.Connection = connectOracle.oraCon;

            oraCmd.Parameters.Add("email", customer.Email);
            oraCmd.Parameters.Add("id", customer.CustomerId);

            object result = oraCmd.ExecuteScalar();
            int count = Convert.ToInt32(result);

            connectOracle.Disconnect();
            if (count > 0)
            {
                return "Email đã tồn tại cho 1 khách hàng khác";
            }
            return null;
        }

        public List<Customer> SearchCustomer(string keyword)
        {
            List<Customer> list = new List<Customer>();
            try
            {
                connectOracle.Connect();

                using (OracleCommand oraCmd = new OracleCommand())
                {
                    oraCmd.Connection = connectOracle.oraCon;
                    oraCmd.CommandType = CommandType.Text;

 
                    oraCmd.CommandText =
                        "SELECT customerId, customerName, customerAddress, phoneNumber, email FROM Customer " +
                        "WHERE UPPER(customerName) LIKE UPPER('%' || :keyword || '%') " +
                        "OR phoneNumber LIKE '%' || :keyword || '%' " +
                        "OR customerId = TRY_CAST(:keyword AS NUMBER)"; 

                    oraCmd.Parameters.Add("keyword", keyword);

                    OracleDataReader reader = oraCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Customer newCustomer = new Customer()
                        {
                            CustomerId = reader.GetInt32(0),
                            CustomerName = reader["customerName"].ToString(),
                            CustomerAddress = reader["customerAddress"].ToString(),
                            PhoneNumber = reader["phoneNumber"].ToString(),
                            Email = reader.IsDBNull(4) ? null : reader.GetString(4),
                        };
                        list.Add(newCustomer);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connectOracle.Close();
            }
            return list;
        }

        public bool CheckExist(string email)
        {
            connectOracle.Connect();

            OracleCommand oraCmd = new OracleCommand();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "select 1 from Customer where email = :email";
            oraCmd.Parameters.Add("email", email);
            oraCmd.Connection = connectOracle.oraCon;

            OracleDataReader reader = oraCmd.ExecuteReader();
            bool exists = reader.Read();
            reader.Close();
            connectOracle.Disconnect();
            return exists;
        }
    }
}