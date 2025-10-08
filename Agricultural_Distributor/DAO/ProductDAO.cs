using Agricultural_Distributor.DTO;
using Agricultural_Distributor.Entity;
using Microsoft.Win32;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Agricultural_Distributor.DAO
{
    internal class ProductDAO
    {
        ConnectOracle connectOracle = new();
        Product product;
        private byte[]? imageBytes = null;

        public ProductDAO(Product product)
        {
            this.product = product;
        }

        public ProductDAO() { }

        public double Total => product.IsSelected ? product.PurchasePrice * product.QuantitySelect : 0;

        public List<Product> LoadProduct()
        {
            List<Product> products = new();
            connectOracle.Connect();

            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "SELECT p.ProductId, p.productName, p.qualityStandard, p.purchasePrice, p.sellingPrice, w.quantity, w.measurementUnit " +
                                 "FROM Product p JOIN WarehouseInfo w ON p.ProductId = w.productId WHERE IsActive = 1 ORDER BY productName";

            oraCmd.Connection = connectOracle.oraCon;

            OracleDataReader reader = oraCmd.ExecuteReader();
            while (reader.Read())
            {
                int productId = reader.GetInt32(0);
                string name = reader.GetString(1);
                string quanlityStandard = reader.GetString(2);
                double purchasePrice = reader.GetDouble(3);
                double sellingPrice = reader.GetDouble(4);
                int quantity = reader.GetInt32(5);
                string meaUnit = reader.GetString(6);

                Product product = new(productId, name, quanlityStandard, purchasePrice, sellingPrice, quantity, meaUnit);
                products.Add(product);
            }
            reader.Close();
            connectOracle.Disconnect();
            return products;
        }

        public List<Product> GetProductList()
        {
            List<Product> products = new();
            connectOracle.Connect();

            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "SELECT p.ProductId, p.productName, p.qualityStandard, p.purchasePrice, p.sellingPrice, p.photo, w.quantity, w.measurementUnit " +
                                 "FROM Product p JOIN WarehouseInfo w ON p.ProductId = w.productId WHERE p.IsActive = 1";
            oraCmd.Connection = connectOracle.oraCon;

            try
            {
                OracleDataReader reader = oraCmd.ExecuteReader();

                while (reader.Read())
                {
                    int productId = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    string quanlityStandard = reader.GetString(2);
                    double purchasePrice = reader.GetDouble(3);
                    double sellingPrice = reader.GetDouble(4);

                    byte[] photo = null;
                    if (!reader.IsDBNull(5))
                    {
                        long bytesRead = reader.GetBytes(5, 0, null, 0, 0);
                        photo = new byte[bytesRead];
                        reader.GetBytes(5, 0, photo, 0, (int)bytesRead);
                    }

                    int quantity = reader.GetInt32(6);
                    string meaUnit = reader.GetString(7);

                    Product product = new(productId, name, quanlityStandard, purchasePrice, sellingPrice, photo, quantity, meaUnit);
                    products.Add(product);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("LỖI KHI TẢI SẢN PHẨM" + ex.Message);
            }
            finally
            {
                connectOracle.Close();
            }
            return products;
        }

        public string CheckInfo(Product product)
        {
            if (!Regex.IsMatch(product.MeasurementUnit, @"^[a-zA-Z]+$"))
            {
                return "Đơn vị chỉ được chứa các chữ cái (không chứa số hoặc ký tự đặc biệt)";
            }

            if (product.PurchasePrice <= 0 || product.SellingPrice <= 0)
            {
                return "Vui lòng nhập giá lớn hơn 0";
            }

            if (product.Quantity < 0)
            {
                return "Vui lòng nhập số lượng lớn hơn 0";
            }
            return null;
        }


        public int? AddProduct(Product product)
        {
            int? proId = null;
            try
            {
                connectOracle.Connect();

                OracleCommand oraCmd = new("PROC_ADDPRODUCT", connectOracle.oraCon);
                oraCmd.CommandType = CommandType.StoredProcedure;
                oraCmd.Parameters.Add("p_productName", OracleDbType.NVarchar2).Value = product.Name;
                oraCmd.Parameters.Add("p_purchasePrice", OracleDbType.Double).Value = product.PurchasePrice;
                oraCmd.Parameters.Add("p_sellingPrice", OracleDbType.Double).Value = product.SellingPrice;
                oraCmd.Parameters.Add("p_qualityStandard", OracleDbType.NVarchar2).Value = product.QualityStandard;
                oraCmd.Parameters.Add("p_quantityInStock", OracleDbType.Int32).Value = product.Quantity;

                object photoValue;
                if (product.Photo == null || product.Photo.Length == 0)
                {
                    photoValue = DBNull.Value;
                }
                else
                {
                    photoValue = product.Photo;
                }
                oraCmd.Parameters.Add("p_photo", OracleDbType.Blob).Value = photoValue;

                oraCmd.Parameters.Add("p_measurementUnit", OracleDbType.NVarchar2).Value = product.MeasurementUnit;

                OracleParameter outputProId = new("p_newProductId", OracleDbType.Int32, ParameterDirection.Output);
                oraCmd.Parameters.Add(outputProId);

                oraCmd.ExecuteNonQuery();

                if (outputProId.Value != DBNull.Value)
                {
                    proId = Convert.ToInt32(outputProId.Value.ToString());
                }

                return proId;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm sản phẩm: {ex.Message}");
                return null;
            }
            finally
            {
                connectOracle.Close();
            }
        }

        public Product GetProductByID(int productId)
        {
            Product product = new();
            try
            {
                connectOracle.Connect();
                OracleCommand oraCmd = new("proc_GetProductById", connectOracle.oraCon);
                {
                    oraCmd.CommandType = CommandType.Text;
                    oraCmd.CommandText = "SELECT p.productName, p.purchasePrice, p.sellingPrice, p.qualityStandard, w.measurementUnit, w.quantity, p.photo " +
                                         "FROM Product p JOIN WarehouseInfo w ON p.ProductId = w.productId WHERE p.ProductId = :ProductId";
                    oraCmd.Parameters.Clear();

                    oraCmd.Parameters.Add("ProductId", OracleDbType.Int32).Value = productId;

                    OracleDataReader reader = oraCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        byte[] photo = null;
                        if (!reader.IsDBNull(6))
                        {
                            long bytesRead = reader.GetBytes(6, 0, null, 0, 0);
                            photo = new byte[bytesRead];
                            reader.GetBytes(6, 0, photo, 0, (int)bytesRead);
                        }

                        product.ProductId = productId;
                        product.Name = reader["productName"].ToString();
                        product.PurchasePrice = Convert.ToDouble(reader["purchasePrice"]);
                        product.SellingPrice = Convert.ToDouble(reader["sellingPrice"]);
                        product.QualityStandard = reader["qualityStandard"].ToString();
                        product.MeasurementUnit = reader["measurementUnit"].ToString();
                        product.Quantity = Convert.ToInt32(reader["quantity"]);
                        product.Photo = photo;
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("LỖI: " + ex.Message);
            }
            finally
            {
                connectOracle.Close();
            }
            return product;
        }


        public byte[] InsertImage(Image photo)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ImageFiles|*.jpg;*.jpeg;*.png;*.webp";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                imageBytes = Utils.ConvertImageToBytes(filePath);

                photo.Source = Utils.ConvertBytesToImage(imageBytes);

            }
            return imageBytes;
        }



        public void UpdateProduct(Product product)
        {
            try
            {
                connectOracle.Connect();
                OracleCommand oraCmd = new("proc_UpdateProduct", connectOracle.oraCon);
                {
                    oraCmd.CommandType = CommandType.StoredProcedure;
                        
                    oraCmd.Parameters.Add("p_productId", OracleDbType.Int32).Value = product.ProductId;
                    oraCmd.Parameters.Add("p_productName", OracleDbType.NVarchar2).Value = product.Name;
                    oraCmd.Parameters.Add("p_purchasePrice", OracleDbType.Double).Value = product.PurchasePrice;
                    oraCmd.Parameters.Add("p_sellingPrice", OracleDbType.Double).Value = product.SellingPrice;
                    oraCmd.Parameters.Add("p_qualityStandard", OracleDbType.NVarchar2).Value = product.QualityStandard;
                    oraCmd.Parameters.Add("p_quantityInStock", OracleDbType.Int32).Value = product.Quantity;
                    

                    if (product.Photo != null)
                    {
                        oraCmd.Parameters.Add("p_photo", OracleDbType.Blob).Value = product.Photo;
                    }
                    else
                    {
                        oraCmd.Parameters.Add("p_photo", OracleDbType.Blob).Value = DBNull.Value;
                    }
                    oraCmd.Parameters.Add("p_measurementUnit", OracleDbType.NVarchar2).Value = product.MeasurementUnit;
                    oraCmd.ExecuteNonQuery();
                    MessageBox.Show("CHỈNH SỬA THÀNH CÔNG!!!");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connectOracle.Close();
            }
        }

        //public void DeleteProduct(int productId)
        //{
        //    try
        //    {
        //        connectOracle.Connect();
        //        OracleCommand oraCmd = new("DELETE FROM Product WHERE ProductId = :productId", connectOracle.oraCon);
        //        {
        //            oraCmd.CommandType = CommandType.Text;
        //            oraCmd.Parameters.Add("productId", OracleDbType.Int32).Value = productId;

        //            oraCmd.ExecuteNonQuery();
        //            MessageBox.Show("XOÁ SẢN PHẨM THÀNH CÔNG.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Lỗi khi xoá " + ex.Message);
        //        //MessageBox.Show("LỖI KHI XOÁ SẢN PHẨM");
        //    }
        //    finally
        //    {
        //        connectOracle.Close();
        //    }
        //}
        public void DeleteProduct(int productId)
        {
            try
            {
                connectOracle.Connect();
                OracleCommand oraCmd = new OracleCommand("proc_DeleteProduct", connectOracle.oraCon);
                oraCmd.CommandType = CommandType.StoredProcedure;

                oraCmd.Parameters.Add("p_productId", OracleDbType.Int32).Value = productId;

                oraCmd.ExecuteNonQuery();
                MessageBox.Show("Xoá sản phẩm thành công khỏi hệ thống.");
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Lỗi Oracle: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khác: " + ex.Message);
            }
            finally
            {
                connectOracle.Close();
            }
        }


        public List<Product> SearchProduct(string sql, string keyword)
        {
            List<Product> products = new();
            try
            {
                connectOracle.Connect();
                OracleCommand oraCommand = new(sql, connectOracle.oraCon);
                {
                    oraCommand.CommandType = CommandType.Text;

                    oraCommand.Parameters.Add("Keyword", "%" + keyword + "%");

                    OracleDataReader reader = oraCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        byte[] photo = null;
                        object photoValue = reader["photo"];
                        if (photoValue != DBNull.Value)
                        {
                            if (photoValue is Oracle.ManagedDataAccess.Types.OracleBlob oraBlob)
                            {
                                if (oraBlob.Length > 0)
                                {
                                    photo = new byte[oraBlob.Length];
                                    oraBlob.Read(photo, 0, (int)oraBlob.Length);
                                }
                            }
                            else
                            {
                                int photoIndex = reader.GetOrdinal("photo");
                                if (!reader.IsDBNull(photoIndex))
                                {
                                    long bytesRead = reader.GetBytes(photoIndex, 0, null, 0, 0);
                                    photo = new byte[bytesRead];
                                    reader.GetBytes(photoIndex, 0, photo, 0, (int)bytesRead);
                                }
                            }
                        }

                        Product product = new()
                        {
                            ProductId = Convert.ToInt32(reader["ProductId"]),
                            Name = reader["productName"].ToString(),
                            PurchasePrice = Convert.ToDouble(reader["purchasePrice"]),
                            SellingPrice = Convert.ToDouble(reader["sellingPrice"]),
                            QualityStandard = reader["qualityStandard"].ToString(),
                            MeasurementUnit = reader["measurementUnit"].ToString(),
                            Quantity = Convert.ToInt32(reader["quantity"]),
                            Photo = photo,
                        };
                        products.Add(product);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connectOracle.Close();
            }
            return products;
        }

        public List<QuanMonth> GetQuanMonth(int productId)
        {
            List<QuanMonth> quanMonths = new();
            connectOracle.Connect();

            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;

            oraCmd.CommandText = "SELECT TO_CHAR(t.DateOfImplementation, 'YYYY-MM') AS \"Month\", SUM(rd.quantity) " +
                                 "FROM Transactions t " +
                                 "JOIN Receipt r ON r.receiptId = t.receiptId " +
                                 "JOIN ReceiptDetail rd ON rd.receiptId = r.receiptId " +
                                 "WHERE rd.productId = :proId AND r.typeOfReceipt = 'Bán' " +
                                 "GROUP BY TO_CHAR(t.DateOfImplementation, 'YYYY-MM') ORDER BY \"Month\"";

            oraCmd.Parameters.Add("proId", OracleDbType.Int32).Value = productId;

            oraCmd.Connection = connectOracle.oraCon;

            OracleDataReader reader = oraCmd.ExecuteReader();
            while (reader.Read())
            {
                string dt = reader.GetString(0);
                int quan = reader.GetInt32(1);
                QuanMonth quanMonth = new()
                {
                    Month = dt,
                    Quan = quan
                };
                quanMonths.Add(quanMonth);
            }
            reader.Close();
            connectOracle.Disconnect();
            return quanMonths;
        }

        public void UpdateQuanPurPriceProduct(Product product)
        {
            try
            {
                connectOracle.Connect();
                OracleCommand oraCmd = new("proc_UpdateQuanPurPriceProduct", connectOracle.oraCon);
                {
                    oraCmd.CommandType = CommandType.StoredProcedure;

                    oraCmd.Parameters.Add("p_ProductId", OracleDbType.Int32).Value = product.ProductId;
                    oraCmd.Parameters.Add("p_purchasePrice", OracleDbType.Int32).Value = product.PurchasePrice;
                    oraCmd.Parameters.Add("p_quantityInStock", OracleDbType.Int32).Value = product.Quantity;

                    oraCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connectOracle.Close();
            }
        }
    }
}