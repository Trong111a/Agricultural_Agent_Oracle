using Agricultural_Distributor.DAO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Agricultural_Distributor.Entity
{
    internal class Product
    {
        private int productId;
        private string name;
        private string qualityStandard;
        private double purchasePrice;
        private double sellingPrice;
        private int quantity; 
        private string measurementUnit;
        private byte[] photo;
        private int isActive;

        public int QuantitySelect { get; set; }
        public bool IsSelected { get; set; }

        public double PurchasePriceSelect { get; set; }
        public double PurPriceOriginal { get; set; }

        public int ProductId { get => productId; set => productId = value; }
        public string Name { get => name; set => name = value; }
        public string QualityStandard { get => qualityStandard; set => qualityStandard = value; }
        public double PurchasePrice { get => purchasePrice; set => purchasePrice = value; }
        public double SellingPrice { get => sellingPrice; set => sellingPrice = value; }
        public int Quantity { get => quantity; set => quantity = value; }
        public string MeasurementUnit { get => measurementUnit; set => measurementUnit = value; }
        public byte[] Photo { get => photo; set => photo = value; }

        public int IsActive { get => isActive; set => isActive = value; }

        public Product(int productId, string name, string qualityStandard, double purchasePrice, double sellingPrice, int quantity, string measurementUnit)
        {
            ProductId = productId;
            Name = name;
            QualityStandard = qualityStandard;
            PurchasePrice = purchasePrice;
            SellingPrice = sellingPrice;
            Quantity = quantity;
            MeasurementUnit = measurementUnit;
        }

        public Product(int productId, string name, string qualityStandard, double purchasePrice, double sellingPrice, byte[] photo, int quantity, string measurementUnit)
        {
            ProductId = productId;
            Name = name;
            QualityStandard = qualityStandard;
            PurchasePrice = purchasePrice;
            SellingPrice = sellingPrice;
            Quantity = quantity;
            MeasurementUnit = measurementUnit;
            Photo = photo;
        }

        public Product(int productId, string name, string qualityStandard, double purchasePrice, double sellingPrice)
        {
            ProductId = productId;
            Name = name;
            QualityStandard = qualityStandard;
            PurchasePrice = purchasePrice;
            SellingPrice = sellingPrice;
        }

        public Product()
        {

        }
        public BitmapImage ImageSource
        {
            get
            {
                return Utils.ConvertBytesToImage(Photo);
            }
        }
    }
}