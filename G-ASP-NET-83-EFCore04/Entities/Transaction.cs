using G_ASP_NET_83_EFCore04.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_ASP_NET_83_EFCore04.Entities
{
    public class Transaction
    {
        public int TransactionNumber { get; set; }           // PK
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public string? Note { get; set; }

        public string AccountNumber { get; set; } = null!;   // FK
        public Account Account { get; set; } = null!;
    }
}
