using System.Collections.Generic;

namespace LoanCalculator.Models
{
    public class PaymentGraph
    {
        public IEnumerable<PaymentEntry> PaymentEntries { get; set; }
    }
}
