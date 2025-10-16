using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agricultural_Distributor.Common
{
    public static class SessionManager
    {
        //public static string Username { get; set; }
        //public static bool? IsAdmin { get; set; }
        //public static int? AccountId { get; set; }

        public static string Username { get; set; }
        public static bool? IsAdmin { get; set; }
        public static int? AccountId { get; set; }

        internal static Connect? Connect { get; set; }
        public static List<string> Roles { get; set; } = new List<string>();
    }
}
