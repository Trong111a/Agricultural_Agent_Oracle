using Agricultural_Distributor.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Windows;
using Agricultural_Distributor.Common;

namespace Agricultural_Distributor.DAO
{
    internal class AccountDAO
    {
        private ConnectOracle connectOracle = new();
        private Account account;
        public AccountDAO(Account account)
        {
            this.account = account;
        }

        public AccountDAO() { }

        public Account CheckLogin(string username, Connect userConnect)
        {
            OracleConnection connection = userConnect.oraCon;
            Account account = null;

            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    return null;
                }

                using (OracleCommand oraCmd = new OracleCommand())
                {
                    oraCmd.Connection = connection;
                    oraCmd.CommandType = CommandType.Text;

                    oraCmd.CommandText = @"
                 SELECT ID, USERNAME, PASS, EMAIL, ISACTIVE, ISADMIN 
                 FROM AGRICULTURAL_AGENT.ACCOUNT
                 WHERE TRIM(LOWER(USERNAME)) = LOWER(:username)";

                    oraCmd.Parameters.Add("username", OracleDbType.Varchar2, 50).Value = username.Trim();

                    using (OracleDataReader reader = oraCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            account = new Account
                            {
                                Username = reader.GetString(reader.GetOrdinal("USERNAME")),
                                Pass = reader.GetString(reader.GetOrdinal("PASS")),
                                Email = reader["EMAIL"] is DBNull ? null : reader["EMAIL"].ToString(),
                                IsActive = reader["ISACTIVE"] is DBNull ? (bool?)null : Convert.ToBoolean(reader["ISACTIVE"]),
                                IsAdmin = reader["ISADMIN"] is DBNull ? (bool?)null : Convert.ToBoolean(reader["ISADMIN"]),
                                Id = reader["ID"] is DBNull ? (int?)null : Convert.ToInt32(reader["ID"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi truy vấn chi tiết tài khoản: {ex.Message}", "Lỗi Truy Vấn", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return account;
        }




        public Account GetLoggedInAccount()
        {
            return account;
        }

   
        public bool ResetPassword(string email, string newPassword)
        {
            int rowsAffected = 0;
            try
            {
                connectOracle.Connect();

                using (OracleCommand oraCmd = new OracleCommand())
                {
                    oraCmd.Connection = connectOracle.oraCon;
                    oraCmd.CommandType = CommandType.Text;
                    oraCmd.CommandText = "UPDATE AGRICULTURAL_AGENT.Account SET pass = :newPassword WHERE email = :email AND IsActive = 1";

                    oraCmd.Parameters.Add("newPassword", OracleDbType.Varchar2).Value = newPassword;
                    oraCmd.Parameters.Add("email", OracleDbType.Varchar2).Value = email;

                    rowsAffected = oraCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đặt lại mật khẩu: {ex.Message}", "Lỗi Cập Nhật", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connectOracle.Disconnect();
            }
            return rowsAffected > 0;
        }


        public bool IsExistsEmail(string email)
        {
            try
            {
                connectOracle.Connect();

                using (OracleCommand oraCmd = new OracleCommand())
                {
                    oraCmd.Connection = connectOracle.oraCon;
                    oraCmd.CommandType = CommandType.Text;
                    oraCmd.CommandText = "SELECT COUNT(*) FROM AGRICULTURAL_AGENT.Account WHERE email = :email AND IsActive = 1";

                    oraCmd.Parameters.Add("email", OracleDbType.Varchar2).Value = email;

                    object result = oraCmd.ExecuteScalar();
                    int count = Convert.ToInt32(result);
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kiểm tra email: {ex.Message}", "Lỗi Truy Vấn", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                connectOracle.Disconnect();
            }
        }


        public List<string> GetAdminEmails()
        {
            List<string> emails = new List<string>();
            try
            {
                connectOracle.Connect();

                using (OracleCommand oraCmd = new OracleCommand())
                {
                    oraCmd.Connection = connectOracle.oraCon;
                    oraCmd.CommandType = CommandType.Text;
                    oraCmd.CommandText = "SELECT email FROM AGRICULTURAL_AGENT.Account WHERE IsAdmin = 1 AND IsActive = 1";

                    using (OracleDataReader reader = oraCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string email = reader.IsDBNull(0) ? null : reader.GetString(0);
                            if (!string.IsNullOrEmpty(email))
                            {
                                emails.Add(email);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lấy email Admin: {ex.Message}", "Lỗi Truy Vấn", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connectOracle.Disconnect();
            }
            return emails;
        }
    }
}