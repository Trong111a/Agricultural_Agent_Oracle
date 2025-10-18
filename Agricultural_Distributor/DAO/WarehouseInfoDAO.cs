using System;
using System.Data;
using Agricultural_Distributor.Common;
using Agricultural_Distributor.Entity;
using Oracle.ManagedDataAccess.Client;

namespace Agricultural_Distributor.DAO
{
    internal class WarehouseInfoDAO
    {
        //connect connect = new();
        Connect connect = SessionManager.Connect;

        public WarehouseInfoDAO() { }
        public WarehouseInfo GetInfoProduct(int productId)
        {
            WarehouseInfo warehouseInfo = new();
            connect.ConnectDB();

            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "SELECT quantity, measurementUnit FROM AGRICULTURAL_AGENT.WarehouseInfo WHERE productId = :proId";

            oraCmd.Parameters.Add(":proId", OracleDbType.Int32).Value = productId;

            oraCmd.Connection = connect.oraCon;

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
                connect.Close();
            }

            return warehouseInfo;
        }


        public DataTable GetInventoryReport()
        {
            connect.ConnectDB();

            OracleCommand cmd = new("AGRICULTURAL_AGENT.proc_InventoryReport", connect.oraCon);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_report_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            try
            {
                OracleDataAdapter adapter = new(cmd);
                DataTable dt = new();
                adapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi truy vấn báo cáo tồn kho: {ex.Message}", "Lỗi DAO", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return new DataTable(); 
            }
            finally
            {
                connect.Close();
            }
        }
    }
}