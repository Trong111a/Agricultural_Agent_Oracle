using Agricultural_Distributor.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agricultural_Distributor.DTO
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string QualityStandard { get; set; }
        public double PurchasePrice { get; set; }
        public double SellingPrice { get; set; }
        public int Quantity { get; set; }
        public string MeasurementUnit { get; set; }
        public byte[] Photo { get; set; }

        public ProductDTO() { }

        internal static ProductDTO FromProduct(Product product)
        {
            return new ProductDTO {
                ProductId = product.ProductId,
                Name = product.Name,
                QualityStandard = product.QualityStandard,
                PurchasePrice = product.PurchasePrice,
                SellingPrice = product.SellingPrice,
                Quantity = product.Quantity,
                MeasurementUnit = product.MeasurementUnit,
                Photo = product.Photo,
            };
        }

        public ProductDTO(int ProductId, string Name)
        {
            this.ProductId = ProductId;
            this.Name = Name;
        }


    }

}
