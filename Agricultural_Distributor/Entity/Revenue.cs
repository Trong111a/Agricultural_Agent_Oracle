using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agricultural_Distributor.Entity
{
    internal class Revenue
    {
        private int productId;
        private string productName;
        private float purchasePrice;
        private float sellPrice;
        private DateTime transactionDate;
        private int quantity;

        public int ProductId { get => productId; set => productId = value; }
        public string ProductName { get => productName; set => productName = value; }
        public float PurchasePrice { get => purchasePrice; set => purchasePrice = value; }
        public float SellPrice { get => sellPrice; set => sellPrice = value; }
        public DateTime TransactionDate { get => transactionDate; set => transactionDate = value; }
        public int Quantity { get => quantity; set => quantity = value; }
        public bool IsSelected { get; set; }
        public Revenue()
        {

        }
        public Revenue(int productId, string productName, float purchasePrice, float sellPrice, DateTime transactionDate, int quantity)
        {
            ProductId = productId;
            ProductName = productName;
            PurchasePrice = purchasePrice;
            SellPrice = sellPrice;
            TransactionDate = transactionDate;
            Quantity = quantity;
        }
    }
}
