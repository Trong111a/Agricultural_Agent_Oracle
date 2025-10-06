using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agricultural_Distributor.Entity
{
    internal class QuanMonth
    {
        string month;
        int quan;

        public string Month { get => month; set => month = value; }
        public int Quan { get => quan; set => quan = value; }

        public QuanMonth() { }
        public QuanMonth(string month, int quan)
        {
            Month = month;
            Quan = quan;
        }
    }
}
