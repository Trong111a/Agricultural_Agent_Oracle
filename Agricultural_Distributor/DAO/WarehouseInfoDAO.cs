using System;
using System.Data;
using Agricultural_Distributor.Entity;
using Oracle.ManagedDataAccess.Client;

namespace Agricultural_Distributor.DAO
{
    internal class WarehouseInfoDAO
    {
        ConnectOracle connectOracle = new();

        public WarehouseInfoDAO() { }
        public WarehouseInfo GetInfoProduct(int productId)
        {
            WarehouseInfo warehouseInfo = new();
            connectOracle.Connect();

            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "SELECT quantity, measurementUnit FROM WarehouseInfo WHERE productId = :proId";

            oraCmd.Parameters.Add(":proId", OracleDbType.Int32).Value = productId;

            oraCmd.Connection = connectOracle.oraCon;

            try
            {
                OracleDataReader reader = oraCmd.ExecuteReader();
                if (reader.Read())
                {
                    int quan = reader.GetInt32(0);
                    string mea = reader.GetString(1);

                    warehouseInfo.ProductId = productId;
                    warehouseInfo.Quantity = quan;
                    warehouseInfo.MeasurementUnit = mea;
                }
                reader.Close();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                connectOracle.Close();
            }

            return warehouseInfo;
        }

        public DataTable GetInventoryReport()
        {
            connectOracle.Connect();

            OracleCommand cmd = new("proc_InventoryReport", connectOracle.oraCon);
            cmd.CommandType = CommandType.StoredProcedure;

            OracleDataAdapter adapter = new(cmd);
            DataTable dt = new();
            adapter.Fill(dt);
            connectOracle.Close();
            return dt;
        }
    }
}