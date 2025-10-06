using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agricultural_Distributor.Entity
{
    internal class ReceiptDetail
    {
        private int receiptId;
        private int productId;
        private string productName;
        private int quantity;
        private double unitPrice;

        public int ReceiptId { get => receiptId; set => receiptId = value; }
        public int ProductId { get => productId; set => productId = value; }
        public string ProductName { get => productName; set => productName = value; }
        public int Quantity { get => quantity; set => quantity = value; }
        public double UnitPrice { get => unitPrice; set => unitPrice = value; }

        public ReceiptDetail() { }

        public ReceiptDetail(int productId, string productName, int quantity, double unitPrice)
        {
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
