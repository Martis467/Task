using System;

namespace LoanCalculator.Models
{
    public class PaymentEntry
    {
        public DateTime Date { get; set; }
        
        public decimal RemainderBeforePayment { get; set; }
        
        public decimal CreditPart { get; set; }
        
        public decimal InterestPart { get; set; }

        public decimal Payment { get; set; }

        public decimal RemainderAfterPayment { get; set; }
    }
}
