using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using Agricultural_Distributor.Entity;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Agricultural_Distributor.DAO
{
    internal class TransactionsDAO
    {
        ConnectOracle connectOracle = new();
        Transactions transactions;

        public TransactionsDAO() { }

        public TransactionsDAO(Transactions transactions)
        {
            this.transactions = transactions;
        }

        public bool CreateTrans()
        {
            connectOracle.Connect();
            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "INSERT INTO Transactions(employeeId, receiptId, customerId, deliveryAddress, DateOfImplementation, repayment) " +
                "VALUES (:employeeId, :receiptId, :customerId, :deliveryAddress, SYSDATE, :repayment)";

            oraCmd.Parameters.Add(":employeeId", OracleDbType.Varchar2).Value = transactions.EmployeeId;
            oraCmd.Parameters.Add(":receiptId", OracleDbType.Varchar2).Value = transactions.ReceiptId;
            oraCmd.Parameters.Add(":customerId", OracleDbType.Varchar2).Value = transactions.CustomerId;
            oraCmd.Parameters.Add(":deliveryAddress", OracleDbType.Varchar2).Value = transactions.DeliveryAddress;
            oraCmd.Parameters.Add(":repayment", OracleDbType.Int32).Value = transactions.Repayment;

            oraCmd.Connection = connectOracle.oraCon;

            try
            {
                int result = oraCmd.ExecuteNonQuery();
                connectOracle.Close();
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                connectOracle.Close();
                return false;
            }
        }

        public List<Transactions> LoadTrans(string typeOfReceipt, DateTime? dt)
        {
            if (dt == null)
            {
                List<Transactions> transactionsList = new();
                connectOracle.Connect();
                OracleCommand oraCmd = new();
                oraCmd.CommandType = CommandType.Text;
                oraCmd.CommandText = "SELECT t.transactionId, t.employeeId, t.receiptId, t.customerId FROM Transactions t " +
                    "JOIN Receipt r ON r.receiptId = t.receiptId WHERE r.typeOfReceipt = :typeOfReceipt ORDER BY DateOfImplementation DESC";

                oraCmd.Parameters.Add(":typeOfReceipt", OracleDbType.Varchar2).Value = typeOfReceipt;
                oraCmd.Connection = connectOracle.oraCon;

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
                connectOracle.Close();
                return transactionsList;
            }
            else
            {
                List<Transactions> transactionsList = new();
                connectOracle.Connect();
                OracleCommand oraCmd = new();
                oraCmd.CommandType = CommandType.Text;

    
                oraCmd.CommandText = "SELECT t.transactionId, t.employeeId, t.receiptId, t.customerId FROM Transactions t " +
                    "JOIN Receipt r ON r.receiptId = t.receiptId WHERE TRUNC(t.DateOfImplementation) = TRUNC(:date) AND r.typeOfReceipt = :typeOfReceipt ORDER BY DateOfImplementation DESC";

                oraCmd.Parameters.Add(":typeOfReceipt", OracleDbType.Varchar2).Value = typeOfReceipt;
                oraCmd.Parameters.Add(":date", OracleDbType.Date).Value = dt.Value.Date;
                oraCmd.Connection = connectOracle.oraCon;

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
                connectOracle.Close();
                return transactionsList;
            }

        }

        public int GetRepayment(int transId)
        {
            int repay = 0;
            connectOracle.Connect();
            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "SELECT repayment FROM Transactions WHERE transactionId = :transId";

            oraCmd.Parameters.Add(":transId", OracleDbType.Int32).Value = transId;
            oraCmd.Connection = connectOracle.oraCon;

            OracleDataReader reader = oraCmd.ExecuteReader();
            if (reader.Read()) repay = reader.GetInt32(0);
            reader.Close();
            connectOracle.Close();
            return repay;
        }

        public bool ConfirmTrans(int transId, double repay)
        {
            connectOracle.Connect();
            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "UPDATE Transactions SET repayment = :repay WHERE transactionId = :transId";

    
            oraCmd.Parameters.Add(":repay", OracleDbType.Double).Value = repay;
            oraCmd.Parameters.Add(":transId", OracleDbType.Int32).Value = transId;

            oraCmd.Connection = connectOracle.oraCon;

            try
            {
                int result = oraCmd.ExecuteNonQuery();
                connectOracle.Close();
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                connectOracle.Close();
                return false;
            }
        }
    }
}