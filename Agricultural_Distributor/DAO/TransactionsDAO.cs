using Agricultural_Distributor.Common;
using Agricultural_Distributor.Entity;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Windows;

namespace Agricultural_Distributor.DAO
{
    internal class TransactionsDAO
    {
        // connect connect = new();
        Connect connect = SessionManager.Connect;
        Transactions transactions;

        public TransactionsDAO() { }

        public TransactionsDAO(Transactions transactions)
        {
            this.transactions = transactions;
        }

        public bool CreateTrans()
        {
            connect.ConnectDB();
            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "INSERT INTO AGRICULTURAL_AGENT.Transactions(employeeId, receiptId, customerId, deliveryAddress, DateOfImplementation, repayment) " +
                "VALUES (:employeeId, :receiptId, :customerId, :deliveryAddress, SYSDATE, :repayment)";

            oraCmd.Parameters.Add(":employeeId", OracleDbType.Int32).Value = transactions.EmployeeId;
            oraCmd.Parameters.Add(":receiptId", OracleDbType.Int32).Value = transactions.ReceiptId;
            oraCmd.Parameters.Add(":customerId", OracleDbType.Int32).Value = transactions.CustomerId;
            oraCmd.Parameters.Add(":deliveryAddress", OracleDbType.Varchar2).Value = transactions.DeliveryAddress;
            oraCmd.Parameters.Add(":repayment", OracleDbType.Int32).Value = transactions.Repayment;

            oraCmd.Connection = connect.oraCon;

            try
            {
                int result = oraCmd.ExecuteNonQuery();
                connect.Close();
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                connect.Close();
                return false;
            }
        }

        public List<Transactions> LoadTrans(string typeOfReceipt, DateTime? dt)
        {
            if (dt == null)
            {
                List<Transactions> transactionsList = new();
                connect.ConnectDB();
                OracleCommand oraCmd = new();
                oraCmd.CommandType = CommandType.Text;
                oraCmd.CommandText = "SELECT t.TRANSACTIONID, t.EMPLOYEEID, t.RECEIPTID, t.CUSTOMERID FROM AGRICULTURAL_AGENT.TRANSACTIONS t " +
                 "JOIN AGRICULTURAL_AGENT.RECEIPT r ON r.RECEIPTID = t.RECEIPTID WHERE r.TYPEOFRECEIPT = :typeOfReceipt ORDER BY DATEOFIMPLEMENTATION DESC";

                oraCmd.Parameters.Add("typeOfReceipt", OracleDbType.NVarchar2).Value = typeOfReceipt;
                oraCmd.Connection = connect.oraCon;

                OracleDataReader reader = oraCmd.ExecuteReader();
                while (reader.Read())
                {
                    int transId = reader.GetInt32(0);
                    int empId = reader.GetInt32(1);
                    int receiptId = reader.GetInt32(2);
                    int cusId = reader.GetInt32(3);

                    Transactions trans = new(transId, empId, receiptId, cusId);

                    transactionsList.Add(trans);
                }
                reader.Close();
                connect.Close();
                return transactionsList;
            }
            else
            {
                List<Transactions> transactionsList = new();
                connect.ConnectDB();
                OracleCommand oraCmd = new();
                oraCmd.CommandType = CommandType.Text;


                oraCmd.CommandText = "SELECT t.TRANSACTIONID, t.EMPLOYEEID, t.RECEIPTID, t.CUSTOMERID FROM AGRICULTURAL_AGENT.TRANSACTIONS t " +
                "JOIN AGRICULTURAL_AGENT.RECEIPT r ON r.RECEIPTID = t.RECEIPTID WHERE TRUNC(t.DATEOFIMPLEMENTATION) = TRUNC(:p_date) AND r.TYPEOFRECEIPT = :typeOfReceipt ORDER BY DATEOFIMPLEMENTATION DESC";
                oraCmd.Parameters.Add("p_date", OracleDbType.Date).Value = dt.Value.Date;
                oraCmd.Parameters.Add("typeOfReceipt", OracleDbType.NVarchar2).Value = typeOfReceipt;


                oraCmd.Connection = connect.oraCon;

                OracleDataReader reader = oraCmd.ExecuteReader();
                while (reader.Read())
                {
                    int transId = reader.GetInt32(0);
                    int empId = reader.GetInt32(1);
                    int receiptId = reader.GetInt32(2);
                    int cusId = reader.GetInt32(3);

                    Transactions trans = new(transId, empId, receiptId, cusId);

                    transactionsList.Add(trans);
                }
                reader.Close();
                connect.Close();
                return transactionsList;
            }

        }

        public int GetRepayment(int transId)
        {
            int repay = 0;
            connect.ConnectDB();
            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "SELECT repayment FROM AGRICULTURAL_AGENT.Transactions WHERE transactionId = :transId";

            oraCmd.Parameters.Add(":transId", OracleDbType.Int32).Value = transId;
            oraCmd.Connection = connect.oraCon;

            OracleDataReader reader = oraCmd.ExecuteReader();
            if (reader.Read()) repay = reader.GetInt32(0);
            reader.Close();
            connect.Close();
            return repay;
        }

        public bool ConfirmTrans(int transId, double repay)
        {
            connect.ConnectDB();
            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "UPDATE AGRICULTURAL_AGENT.Transactions SET repayment = :repay WHERE transactionId = :transId";


            oraCmd.Parameters.Add(":repay", OracleDbType.Double).Value = repay;
            oraCmd.Parameters.Add(":transId", OracleDbType.Int32).Value = transId;

            oraCmd.Connection = connect.oraCon;

            try
            {
                int result = oraCmd.ExecuteNonQuery();
                connect.Close();
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                connect.Close();
                return false;
            }
        }

    }
}