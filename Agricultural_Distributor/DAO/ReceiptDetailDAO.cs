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

    internal class ReceiptDetailDAO
    {
        //connect connect = new();
        Connect connect = SessionManager.Connect;
        public ReceiptDetailDAO() { }

        public bool CheckProductExist(int productId)
        {
            connect.ConnectDB();
            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "SELECT productId FROM AGRICULTURAL_AGENT.ReceiptDetail WHERE productId = :productId";

            oraCmd.Parameters.Add("productId", productId);
            oraCmd.Connection = connect.oraCon;

            OracleDataReader reader = oraCmd.ExecuteReader();
            if (reader.Read())
            {
                reader.Close();
                connect.Disconnect();
                return true;
            }
            connect.Disconnect();
            return false;
        }
    }
}