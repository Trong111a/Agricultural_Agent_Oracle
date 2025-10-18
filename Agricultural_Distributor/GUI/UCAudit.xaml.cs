using Agricultural_Distributor.Common;
using Agricultural_Distributor.Entity;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Agricultural_Distributor.GUI
{
    /// <summary>
    /// Interaction logic for UCAudit.xaml
    /// </summary>
    public partial class UCAudit : UserControl
    {
        Connect connect = SessionManager.Connect;
        WDHome wDHome;
        public UCAudit(WDHome wDHome)
        {
            InitializeComponent();
            //this.uCAudit = uCAudit;
            this.wDHome = wDHome;
            LoadAuditLogs();
        }
        private void LoadAuditLogs()
        {
            //string query = @"
            //SELECT 
            //    DB_USER,
            //    OBJECT_NAME,
            //    SQL_TEXT,
            //    SQL_BIND,
            //    STATEMENT_TYPE,
            //    TO_CHAR(TIMESTAMP, 'YYYY-MM-DD HH24:MI:SS') AS AUDIT_TIME
            //FROM 
            //    DBA_FGA_AUDIT_TRAIL 
            //ORDER BY TIMESTAMP DESC";
            string query = @"
            SELECT 
                DB_USER,
                OBJECT_NAME,
                SQL_TEXT,
                SQL_BIND,
                STATEMENT_TYPE,
                TO_CHAR(TIMESTAMP, 'YYYY-MM-DD HH24:MI:SS') AS AUDIT_TIME
            FROM 
                DBA_FGA_AUDIT_TRAIL 
            ORDER BY TIMESTAMP DESC";


            try
            {
                connect.ConnectDB();
                OracleDataAdapter adapter = new OracleDataAdapter(query, connect.oraCon);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Add new column for user-friendly description
                dt.Columns.Add("AUDIT_DESCRIPTION", typeof(string));
                dt.Columns.Add("DATA_LOG", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    string sqlText = row["SQL_TEXT"].ToString();
                    string sqlBind = row["SQL_BIND"].ToString();

                    string description = GenerateFriendlyAuditDescription(sqlText, sqlBind);
                    row["AUDIT_DESCRIPTION"] = description;
                    Product? prod = GetProdLog(GetIdProdLog(sqlText, sqlBind));
                    string dataLog = $"Sản phẩm ID {prod.ProductId}: Tên = '{prod.Name}', Giá mua = {prod.PurchasePrice}, Giá bán = {prod.SellingPrice}, Tiêu chuẩn = '{prod.QualityStandard}'";
                    row["DATA_LOG"] = dataLog;
                }
                listviewLogs.ItemsSource = dt.DefaultView;


            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
        private string GenerateFriendlyAuditDescription(string sqlText, string sqlBind)
        {
            try
            {
                // Chỉ xử lý UPDATE sản phẩm
                if (sqlText.Contains("UPDATE AGRICULTURAL_AGENT.PRODUCT", StringComparison.OrdinalIgnoreCase))
                {
                    // Tách SQL_BIND theo thứ tự :B1, :B2, ...
                    var binds = sqlBind.Split('#')
                        .Where(b => !string.IsNullOrWhiteSpace(b))
                        .Select(b =>
                        {
                            var colonIndex = b.IndexOf(':');
                            return colonIndex >= 0 ? b.Substring(colonIndex + 1).Trim() : "";
                        }).ToArray();

                    string productId = binds.ElementAtOrDefault(5);
                    string photo = binds.ElementAtOrDefault(4);
                    string quality = binds.ElementAtOrDefault(3);
                    string selling = binds.ElementAtOrDefault(2);
                    string purchase = binds.ElementAtOrDefault(1);
                    string name = binds.ElementAtOrDefault(0);

                    return $"Sản phẩm ID {productId}: Tên = '{name}', Giá mua = {purchase}, Giá bán = {selling}, Tiêu chuẩn = '{quality}'";
                }

                // Có thể xử lý thêm các bảng khác nếu cần

                return "(Không xác định thay đổi)";
            }
            catch
            {
                return "(Lỗi phân tích audit)";
            }
        }

        private int? GetIdProdLog(string sqlText, string sqlBind)
        {
            try
            {
                // Chỉ xử lý UPDATE sản phẩm
                if (sqlText.Contains("UPDATE AGRICULTURAL_AGENT.PRODUCT", StringComparison.OrdinalIgnoreCase))
                {
                    // Tách SQL_BIND theo thứ tự :B1, :B2, ...
                    var binds = sqlBind.Split('#')
                        .Where(b => !string.IsNullOrWhiteSpace(b))
                        .Select(b =>
                        {
                            var colonIndex = b.IndexOf(':');
                            return colonIndex >= 0 ? b.Substring(colonIndex + 1).Trim() : "";
                        }).ToArray();
                    
                    string productId = binds.ElementAtOrDefault(5);
                    string photo = binds.ElementAtOrDefault(4);
                    string quality = binds.ElementAtOrDefault(3);
                    string selling = binds.ElementAtOrDefault(2);
                    string purchase = binds.ElementAtOrDefault(1);
                    string name = binds.ElementAtOrDefault(0);
                    return Convert.ToInt32(productId);
                    //return $"Sản phẩm ID {productId}: Tên = '{name}', Giá mua = {purchase}, Giá bán = {selling}, Tiêu chuẩn = '{quality}'";
                }

                // Có thể xử lý thêm các bảng khác nếu cần

                return null;
            }
            catch
            {
                return null;
            }
        }

        private Product? GetProdLog(int? Id)
        {
            connect.ConnectDB();
            OracleCommand oraCmd = new();
            oraCmd.CommandType = CommandType.Text;
            oraCmd.CommandText = "SELECT * FROM AGRICULTURAL_AGENT.PRODUCT_AUDIT WHERE PRODUCTID = :Id";

            oraCmd.Connection = connect.oraCon;
            oraCmd.Parameters.Add("Id", Id);
            OracleDataReader reader = oraCmd.ExecuteReader();
            if (reader.Read())
            {
                int productId = reader.GetInt32(1);
                string name = reader.GetString(2);
                string quanlityStandard = reader.GetString(3);
                double purchasePrice = reader.GetDouble(4);
                double sellingPrice = reader.GetDouble(5);
                Product product = new(productId, name, quanlityStandard, purchasePrice, sellingPrice);
                //MessageBox.Show(productId + " " + name + " " + quanlityStandard + " " + purchasePrice + " " + sellingPrice);
                return product;
            }
            reader.Close();
            return null;
        }

    }
}
