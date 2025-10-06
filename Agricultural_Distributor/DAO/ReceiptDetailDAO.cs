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

    internal class ReceiptDetailDAO
    {
        ConnectOracle connectOracle = new();

        public ReceiptDetailDAO() { }

        public bool CheckProductExist(int productId)
        {
            connectOracle.Connect();
            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "SELECT productId FROM ReceiptDetail WHERE productId = :productId";

            oraCmd.Parameters.Add("productId", productId);
            oraCmd.Connection = connectOracle.oraCon;

            OracleDataReader reader = oraCmd.ExecuteReader();
            if (reader.Read())
            {
                reader.Close();
                connectOracle.Disconnect();
                return true;
            }
            connectOracle.Disconnect();
            return false;
        }

        public void UpdateQuanReceiptDetail(ReceiptDetail receiptDetail, int quan)
        {
            try
            {
                connectOracle.Connect();
                using (OracleCommand oraCmd = new("proc_UpdateQuanReceiptDetail", connectOracle.oraCon))
                {
                    oraCmd.CommandType = CommandType.StoredProcedure;

                    oraCmd.Parameters.Add("recpId", OracleDbType.Varchar2).Value = receiptDetail.ReceiptId;
                    oraCmd.Parameters.Add("prodId", OracleDbType.Varchar2).Value = receiptDetail.ProductId;
                    oraCmd.Parameters.Add("quantity", OracleDbType.Int32).Value = quan;

                    oraCmd.ExecuteNonQuery();
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
        }
    }
}