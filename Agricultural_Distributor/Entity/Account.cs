using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agricultural_Distributor.Entity
{
    internal class Account
    {
        public string Username { get; set; } = null!;

        public string Pass { get; set; } = null!;

        public string? Email { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsAdmin { get; set; }

        public int? Id { get; set; }

        public virtual Employee? IdNavigation { get; set; }
    }
}
