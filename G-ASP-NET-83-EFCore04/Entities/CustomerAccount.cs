using G_ASP_NET_83_EFCore04.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_ASP_NET_83_EFCore04.Entities
{
    public class CustomerAccount
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public string AccountNumber { get; set; } = null!;
        public Account Account { get; set; } = null!;

        public OwnershipType OwnershipType { get; set; }
        public DateTime OwnershipStartDate { get; set; }
        public AccountStatus AccountStatus { get; set; }
    }
}
