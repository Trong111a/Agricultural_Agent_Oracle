using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agricultural_Distributor.Entity
{
    internal class ProductPrediction
    {
        int productId;
        string productName;
        int predictedQuantity;

        public int ProductId { get => productId; set => productId = value; }
        public string ProductName { get => productName; set => productName = value; }
        public int PredictedQuantity { get => predictedQuantity; set => predictedQuantity = value; }

        public ProductPrediction(int productId, string productName, int predictedQuantity)
        {
            ProductId = productId;
            ProductName = productName;
            PredictedQuantity = predictedQuantity;
        }

        public ProductPrediction() { }
    }
}
