using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agricultural_Distributor.Entity
{
    internal class ProductStatistic
    {
        private int productId;
        private string productName;
        private List<int> monthlyQuantities;

        public int ProductId { get => productId; set => productId = value; }
        public string ProductName { get => productName; set => productName = value; }
        public List<int> MonthlyQuantities { get => monthlyQuantities; set => monthlyQuantities = value; }
        
        public ProductStatistic()
        {
            MonthlyQuantities = new List<int>();
        }

        public ProductStatistic(int productId, string productName, List<int> monthlyQuantities)
        {
            ProductId = productId;
            ProductName = productName;
            MonthlyQuantities = monthlyQuantities;
        }
    }
}
 