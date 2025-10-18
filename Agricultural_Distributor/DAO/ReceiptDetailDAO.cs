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

        //public void UpdateQuanReceiptDetail(ReceiptDetail receiptDetail, int quan)
        //{
        //    try
        //    {
        //        connect.Connect();
        //        using (OracleCommand oraCmd = new("proc_UpdateQuanReceiptDetail", connect.oraCon))
        //        {
        //            oraCmd.CommandType = CommandType.StoredProcedure;

        //            oraCmd.Parameters.Add("recpId", OracleDbType.Varchar2).Value = receiptDetail.ReceiptId;
        //            oraCmd.Parameters.Add("prodId", OracleDbType.Varchar2).Value = receiptDetail.ProductId;
        //            oraCmd.Parameters.Add("quantity", OracleDbType.Int32).Value = quan;

        //            oraCmd.ExecuteNonQuery();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    finally
        //    {
        //        connect.Close();
        //    }
        //}
    }
}