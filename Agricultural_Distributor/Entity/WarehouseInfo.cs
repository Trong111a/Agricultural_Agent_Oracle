using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agricultural_Distributor.Entity
{
    internal class WarehouseInfo
    {
        int productId;
        int quantity;
        string measurementUnit;

        public int ProductId { get => productId; set => productId = value; }
        public int Quantity { get => quantity; set => quantity = value; }
        public string MeasurementUnit { get => measurementUnit; set => measurementUnit = value; }

        public WarehouseInfo() { }

        public WarehouseInfo(int productId, int quantity, string measurementUnit)
        {
            ProductId = productId;
            Quantity = quantity;
            MeasurementUnit = measurementUnit;
        }
        //public string ProductId { get; set; } = null!;

        //public int Quantity { get; set; }

        //public string MeasurementUnit { get; set; } = null!;

        public virtual Product Product { get; set; } = null!;
    }
}
