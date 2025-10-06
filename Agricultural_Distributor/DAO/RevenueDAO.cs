using Agricultural_Distributor.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Agricultural_Distributor.DAO
{
    internal class RevenueDAO
    {
        ConnectOracle connectOracle = new();
        Revenue revenue;

        public RevenueDAO() { }

        public RevenueDAO(Revenue revenue)
        {
            this.revenue = revenue;
        }

        public List<Revenue> LoadProduct()
        {
            List<Revenue> productList = new();

            try
            {
                connectOracle.Connect();

                string query = @"
                    SELECT DISTINCT product.productId, product.productName, product.purchasePrice, product.sellingPrice
                    FROM Product product";

                using (OracleCommand cmd = new(query, connectOracle.oraCon))
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Revenue revenue = new()
                        {
                            ProductId = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            PurchasePrice = (float)reader.GetDouble(2),
                            SellPrice = (float)reader.GetDouble(3),
                            Quantity = 0,
                            TransactionDate = DateTime.MinValue
                        };

                        productList.Add(revenue);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải sản phẩm: " + ex.Message);
            }
            finally
            {
                connectOracle.Close();
            }

            return productList;
        }

        public List<List<float>> GetRevenueByYear(List<string> listProducts, string year)
        {
            List<List<float>> revenueByYear = new();
            try
            {
                connectOracle.Connect();

                string query = $@"
                SELECT 
                    p.productName,
                    EXTRACT(MONTH FROM t.DateOfImplementation) AS Monthly,
                    SUM((p.sellingPrice - p.purchasePrice) * rd.quantity) AS Revenue
                FROM Transactions t
                JOIN Receipt r ON t.receiptId = r.receiptId
                JOIN ReceiptDetail rd ON r.receiptId = rd.receiptId
                JOIN Product p ON rd.productId = p.productId
                WHERE TO_CHAR(t.DateOfImplementation, 'YYYY') = :year 
                    AND p.productName IN ({string.Join(",", listProducts.Select((_, i) => $":p{i}"))})
                GROUP BY p.productName, EXTRACT(MONTH FROM t.DateOfImplementation)
                ORDER BY Monthly";

                using (OracleCommand cmd = new(query, connectOracle.oraCon))
                {
                    cmd.Parameters.Add(":year", OracleDbType.Char).Value = year;

                    for (int i = 0; i < listProducts.Count; i++)
                    {
                        cmd.Parameters.Add($":p{i}", OracleDbType.Varchar2).Value = listProducts[i];
                    }

                    for (int i = 0; i < 12; i++)
                    {
                        revenueByYear.Add(new List<float>(new float[listProducts.Count]));
                    }

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string productName = reader.GetString(0);

                            int month = reader.GetInt32(1);

                            float revenue = (float)reader.GetDouble(2);

                            int productIndex = listProducts.IndexOf(productName);
                            if (productIndex >= 0 && month >= 1 && month <= 12)
                            {
                                revenueByYear[month - 1][productIndex] = revenue;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi truy vấn doanh thu theo năm: " + ex.Message);
            }
            finally
            {
                connectOracle.Close();
            }

            return revenueByYear;
        }

        public List<List<float>> GetRevenueAll(List<string> productNames, out string[] years)
        {
            List<List<float>> allRevenues = new();
            HashSet<int> yearSet = new();
            Dictionary<string, Dictionary<int, float>> revenueData = new();

            try
            {
                connectOracle.Connect();

                string query = $@"
                SELECT 
                    p.productName,
                    EXTRACT(YEAR FROM t.DateOfImplementation) AS Yearly,
                    SUM((p.sellingPrice - p.purchasePrice) * rd.quantity) AS revenue
                FROM Transactions t
                    JOIN Receipt r ON t.receiptId = r.receiptId
                    JOIN ReceiptDetail rd ON r.receiptId = rd.receiptId
                    JOIN Product p ON rd.productId = p.productId
                WHERE p.productName IN ({string.Join(",", productNames.Select((_, i) => $":p{i}"))})
                GROUP BY p.productName, EXTRACT(YEAR FROM t.DateOfImplementation)
                ORDER BY p.productName, Yearly";

                using (OracleCommand cmd = new(query, connectOracle.oraCon))
                {
                    for (int i = 0; i < productNames.Count; i++)
                    {
                        cmd.Parameters.Add($":p{i}", OracleDbType.Varchar2).Value = productNames[i];
                    }

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string productName = reader.GetString(0);
                            int year = reader.GetInt32(1); 
                            float revenue = (float)reader.GetDouble(2);

                            yearSet.Add(year);

                            if (!revenueData.ContainsKey(productName))
                                revenueData[productName] = new Dictionary<int, float>();

                            revenueData[productName][year] = revenue;
                        }
                    }
                }

                var sortedYears = yearSet.OrderBy(y => y).ToList();
                years = sortedYears.Select(y => y.ToString()).ToArray();

                foreach (var product in productNames)
                {
                    List<float> revenues = new();
                    foreach (var y in sortedYears)
                    {
                        if (revenueData.ContainsKey(product) && revenueData[product].ContainsKey(y))
                            revenues.Add(revenueData[product][y]);
                        else
                            revenues.Add(0f);
                    }
                    allRevenues.Add(revenues);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thống kê doanh thu theo năm: " + ex.Message);
                years = Array.Empty<string>();
            }
            finally
            {
                connectOracle.Close();
            }

            return allRevenues;
        }
    }
}