using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agricultural_Distributor.Entity
{
    internal class Transactions
    {
        private int transactionId;
        private int? employeeId;
        private int receiptId;
        private int customerId;
        private string deliveryAddress;
        private DateTime DateOfImplementation;
        private int repayment;

        public int TransactionId { get => transactionId; set => transactionId = value; }
        public int? EmployeeId { get => employeeId; set => employeeId = value; }
        public int ReceiptId { get => receiptId; set => receiptId = value; }
        public int CustomerId { get => customerId; set => customerId = value; }
        public string DeliveryAddress { get => deliveryAddress; set => deliveryAddress = value; }
        public DateTime DateOfImplementation1 { get => DateOfImplementation; set => DateOfImplementation = value; }
        public int Repayment { get => repayment; set => repayment = value; }

        public Transactions() { }

        public Transactions(int transactionId, int? employeeId, int receiptId, int customerId, string deliveryAddress, DateTime dateOfImplementation, int repayment)
        {
            TransactionId = transactionId;
            EmployeeId = employeeId;
            ReceiptId = receiptId;
            CustomerId = customerId;
            DeliveryAddress = deliveryAddress;
            DateOfImplementation = dateOfImplementation;
            Repayment = repayment;
        }

        public Transactions(int transactionId, int? employeeId, int receiptId, int customerId)
        {
            TransactionId = transactionId;
            EmployeeId = employeeId;
            ReceiptId = receiptId;
            CustomerId = customerId;
        }
    }
}
