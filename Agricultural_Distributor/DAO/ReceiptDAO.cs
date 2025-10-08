using Agricultural_Distributor.Entity;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Agricultural_Distributor.DAO
{
    internal class ReceiptDAO
    {
        ConnectOracle connectOracle = new();
        Receipt receipt;

        public ReceiptDAO() { }

        public ReceiptDAO(Receipt receipt)
        {
            this.receipt = receipt;
        }

        public int? CreateReceipt(Receipt receipt)
        {
            int receiptIdValue = 0;
            connectOracle.Connect();

            OracleCommand oraCmd = new("proc_CreateOrder", connectOracle.oraCon);
            oraCmd.CommandType = CommandType.StoredProcedure;

            oraCmd.Parameters.Add("p_priceTotal", OracleDbType.Double).Value = receipt.PriceTotal;
            oraCmd.Parameters.Add("p_typeOfReceipt", OracleDbType.NVarchar2).Value = receipt.TypeOfReceipt;

            oraCmd.Parameters.Add("p_discount", OracleDbType.Double).Value = receipt.Discount;

            if (string.IsNullOrEmpty(receipt.Note))
            {
                oraCmd.Parameters.Add("p_note", OracleDbType.NVarchar2).Value = DBNull.Value;
            }
            else
            {
                oraCmd.Parameters.Add("p_note", OracleDbType.NVarchar2).Value = receipt.Note;
            }

            OracleParameter outputReceiptId = new("p_receiptId", OracleDbType.Int32, ParameterDirection.Output);
            oraCmd.Parameters.Add(outputReceiptId);

            try
            {
                oraCmd.ExecuteNonQuery();

                if (outputReceiptId.Value != DBNull.Value)
                {
                    // receiptIdValue = Convert.ToInt32(outputReceiptId.Value);
                    OracleDecimal oracleDecimalValue = (OracleDecimal)outputReceiptId.Value;
                    receiptIdValue = oracleDecimalValue.ToInt32();
                }
                connectOracle.Disconnect();

                return receiptIdValue > 0 ? (int?)receiptIdValue : null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                connectOracle.Disconnect();
                return null;
            }
        }

        public bool AddReceiptDetail(ReceiptDetail receiptDetail)
        {
            connectOracle.Connect();
            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;

            oraCmd.CommandText = "INSERT INTO ReceiptDetail VALUES(:receiptId, :productId, :productName, :quantity, :unitPrice)";

            oraCmd.Parameters.Add("receiptId", receiptDetail.ReceiptId);
            oraCmd.Parameters.Add("productId", receiptDetail.ProductId);
            oraCmd.Parameters.Add("productName", receiptDetail.ProductName);
            oraCmd.Parameters.Add("quantity", receiptDetail.Quantity);
            oraCmd.Parameters.Add("unitPrice", receiptDetail.UnitPrice);

            oraCmd.Connection = connectOracle.oraCon;

            try
            {
                int result = oraCmd.ExecuteNonQuery();
                connectOracle.Disconnect();
                return result > 0;
            }
            catch (Exception)
            {
                connectOracle.Disconnect();
                return false;
            }
        }

        public List<ReceiptDetail> GetReceiptDetailList(int receiptId)
        {
            List<ReceiptDetail> receiptDetails = new();
            connectOracle.Connect();

            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "SELECT receiptId, productId, productName, quantity, unitPrice FROM ReceiptDetail WHERE receiptId = :receiptId";

            oraCmd.Parameters.Add("receiptId", receiptId);
            oraCmd.Connection = connectOracle.oraCon;

            OracleDataReader reader = oraCmd.ExecuteReader();
            while (reader.Read())
            {

                int proId = reader.GetInt32(1);
                string proName = reader.GetString(2); 
                int quantity = reader.GetInt32(3);
                double unitPrice = reader.GetDouble(4);

                ReceiptDetail receiptDetail = new(proId, proName, quantity, unitPrice);
                receiptDetails.Add(receiptDetail);
            }
            reader.Close();
            connectOracle.Disconnect();
            return receiptDetails;
        }

        public Receipt GetReceipt(int receiptId)
        {
            Receipt receipt = new();
            connectOracle.Connect();

            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "SELECT receiptId, typeOfReceipt, priceTotal, discount, note FROM Receipt WHERE receiptId = :receiptId";

            oraCmd.Parameters.Add("receiptId", receiptId);
            oraCmd.Connection = connectOracle.oraCon;

            OracleDataReader reader = oraCmd.ExecuteReader();
            if (reader.Read())
            {

                int recId = reader.GetInt32(0); 
                string typeOfReceipt = reader.GetString(1); 
                double priceTotal = reader.GetDouble(2); 
                double dis = reader.GetDouble(3);
                string note = reader.IsDBNull(4) ? null : reader.GetString(4); 

                receipt.ReceiptId = recId;
                receipt.TypeOfReceipt = typeOfReceipt;
                receipt.PriceTotal = priceTotal;
                receipt.Note = note;
                receipt.Discount = dis;
            }
            reader.Close();
            connectOracle.Disconnect();
            return receipt;
        }

        public DataTable GetDailyRevenueReport(DateTime date)
        {
            connectOracle.Connect();

            OracleCommand cmd = new("proc_DailyRevenueReport", connectOracle.oraCon);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_ReportDate", OracleDbType.Date).Value = date;
            cmd.Parameters.Add("result_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            OracleDataAdapter adapter = new(cmd);
            DataTable dt = new();
            adapter.Fill(dt);
            connectOracle.Disconnect();
            return dt;
        }

        public DataTable GetDailyExpenseReport(DateTime date)
        {
            connectOracle.Connect();

            OracleCommand cmd = new("proc_DailyExpenseReport", connectOracle.oraCon);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_ReportDate", OracleDbType.Date).Value = date;
            cmd.Parameters.Add("result_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            OracleDataAdapter adapter = new(cmd);
            DataTable dt = new();
            adapter.Fill(dt);
            connectOracle.Disconnect();
            return dt;
        }

        public DataTable GetDailyDebtReportReport(DateTime date)
        {
            connectOracle.Connect();

            OracleCommand cmd = new("proc_DailyDebtReport", connectOracle.oraCon);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_ReportDate", OracleDbType.Date).Value = date;
            cmd.Parameters.Add("result_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            OracleDataAdapter adapter = new(cmd);
            DataTable dt = new();
            adapter.Fill(dt);
            connectOracle.Disconnect();
            return dt;
        }
    }
}