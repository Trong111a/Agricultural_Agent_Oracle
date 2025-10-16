using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agricultural_Distributor.Common
{
    internal static class RoleManager
    {
        public static List<string> GetUserRoles(Connect connect)
        {
            List<string> roles = new List<string>();

            try
            {
                if (connect.oraCon == null || connect.oraCon.State != System.Data.ConnectionState.Open)
                    throw new InvalidOperationException("❌ Chưa có kết nối Oracle.");

                string sql = "SELECT ROLE FROM SESSION_ROLES";

                using (var cmd = new OracleCommand(sql, connect.oraCon))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roles.Add(reader.GetString(0));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi khi lấy role: " + ex.Message);
            }

            return roles;
        }
    }
}
