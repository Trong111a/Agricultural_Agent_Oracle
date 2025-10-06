using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Windows; 

namespace Agricultural_Distributor
{
    internal class ConnectOracle
    {
        private string strCon = "User Id=AGRICULTURAL_AGENT;Password=123456;Data Source=localhost:1521/ORCLPDB;";

        public OracleConnection? oraCon = null;

        public void Connect()
        {
            if (oraCon == null)
            {
                oraCon = new OracleConnection(strCon);
            }

            if (oraCon.State == ConnectionState.Closed)
            {
                try
                {
                    oraCon.Open();
                    Console.WriteLine("✅ Kết nối Oracle thành công.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("LỖI KẾT NỐI ORACLE (gốc): " + ex.ToString());
                    MessageBox.Show("Chi tiết lỗi Oracle:\n" + ex.ToString(),
                                    "Lỗi kết nối Oracle",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    throw;
                }
            }
        }


        public void Disconnect()
        {
            if (oraCon != null && oraCon.State == ConnectionState.Open)
            {
                oraCon.Close();
                Console.WriteLine("Đã đóng kết nối Oracle.");
            }
        }

        public void Close()
        {
            this.Disconnect();
        }
    }
}