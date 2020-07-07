namespace LoanCalculator.Models
{
    public class LoanParameters
    {
        public decimal LoanAmount { get; set; }

        public int Term { get; set; }

        public decimal InterestRate { get; set; }

        public int PayDay { get; set; }
    }
}
