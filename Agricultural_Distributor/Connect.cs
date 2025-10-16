using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agricultural_Distributor
{
    internal class Connect
    {
        private string _dataSource = "localhost:1521/ORCLPDB";
        private string? _username;
        private string? _password;
        public OracleConnection? oraCon = null;

        public Connect(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public void ConnectDB()
        {
            string strCon = $"User Id={_username};Password={_password};Data Source={_dataSource};";

            if (oraCon == null)
                oraCon = new OracleConnection(strCon);

            if (oraCon.State == ConnectionState.Closed)
            {
                try
                {
                    oraCon.Open();
                    Console.WriteLine("✅ Kết nối Oracle thành công với user: " + _username);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ Lỗi kết nối Oracle: " + ex.Message);
                    throw;
                }
            }
        }

        public void Disconnect()
        {
            if (oraCon != null && oraCon.State == ConnectionState.Open)
                oraCon.Close();
        }
    }
}

