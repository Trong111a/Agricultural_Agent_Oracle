using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace Agricultural_Distributor.Entity
{
    internal class Customer
    {
        private int customerId;
        private string customerName;
        private string customerAddress;
        private string phoneNumber;
        private string email;

        public int CustomerId { get => customerId; set => customerId = value; }
        public string CustomerName { get => customerName; set => customerName = value; }
        public string CustomerAddress { get => customerAddress; set => customerAddress = value; }
        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }
        public string Email { get => email; set => email = value; }

        public Customer() { }

        public Customer (int customerId, string customerName, string customerAddress, string phoneNumber, string email)
        {
            CustomerId = customerId;
            CustomerName = customerName;
            CustomerAddress = customerAddress;
            PhoneNumber = phoneNumber;
            Email = email;
        }
    }
}
