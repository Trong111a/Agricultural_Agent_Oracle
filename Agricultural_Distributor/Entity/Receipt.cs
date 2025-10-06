using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agricultural_Distributor.Entity
{
    internal class Receipt
    {
        private int receiptId;
        private string typeOfReceipt;
        private string productList;
        private double priceTotal;
        private double discount;
        private string note;

        public string TypeOfReceipt { get => typeOfReceipt; set => typeOfReceipt = value; }
        public double PriceTotal { get => priceTotal; set => priceTotal = value; }
        public double Discount { get => discount; set => discount = value; }
        public string Note { get => note; set => note = value; }
        public int ReceiptId { get => receiptId; set => receiptId = value; }
        public string ProductList { get => productList; set => productList = value; }

        public Receipt() { }

        public Receipt(string productList, string typeOfReceipt, double discount, string note)
        {
            ProductList = productList;
            TypeOfReceipt = typeOfReceipt;
            Discount = discount;
            Note = note;
        }
    }
}
