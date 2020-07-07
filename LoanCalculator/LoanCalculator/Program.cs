using System;
using System.Globalization;
using System.Linq;
using LoanCalculator.Models;
using LoanCalculator.Services;

namespace LoanCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            ValidateArguments(args);

            var parameters = new LoanParameters
            {
                LoanAmount = Decimal.Parse(args[0]),
                Term = Int32.Parse(args[1]),
                InterestRate = Decimal.Parse(args[2]),
                PayDay = Int32.Parse(args[3])
            };

            var calculator = new PaymentCalculator(parameters);
            var graph = calculator.GetPaymentGraph();
            var i = 1;

            Console.WriteLine("Įmokos Nr.\t | Data\t\t |  Likutis prieš įmoką\t | Kredito dalis\t | Palūkanos\t | Įmoka\t | Likutis po įmokos");
            foreach(var g in graph.PaymentEntries)
            {
                var str = $"{i}\t\t | {g.Date.ToString("yyyy-MM-dd")}\t | {g.RemainderBeforePayment:0.00}\t\t | {g.CreditPart:0.00}\t\t |" +
                    $" {g.InterestPart:0.00}\t | {g.Payment:0.00}\t | {g.RemainderAfterPayment:0.00}";

                Console.WriteLine(str);
                i++;
            }

            //PrintTotals(graph);
            var bv = new BVKKMNCalculator(parameters.Term, graph.PaymentEntries.FirstOrDefault().Payment, parameters.LoanAmount).Calculate() * 100;

            Console.WriteLine($"BVKKMN - {bv:0.00}");
        }

        private static void PrintTotals(PaymentGraph graph)
        {
            var totalPay = graph.PaymentEntries.Sum(g => g.Payment);
            var interestPart = graph.PaymentEntries.Sum(g => g.InterestPart);
            var creditPart = graph.PaymentEntries.Sum(g => g.CreditPart);

            Console.WriteLine($"Total amount paid: {totalPay:0.00}{Environment.NewLine}" +
                $"Total interest amount paid: {interestPart:0.00}{Environment.NewLine}" +
                $"Loan amount paid: {creditPart:0.00}");
        }

        private static void ValidateArguments(string[] args)
        {
            if (args.Length < 4)
            {
                throw new Exception($"Not all argumenrs were passed{Environment.NewLine}" +
                    $"args - Loan amount(Decimal) Loan term in months(int) Interest rate(decimal) Payment day(int)");
            }

            var loanAmountValid = Decimal.TryParse(args[0], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var loanAmount);

            if (!loanAmountValid || loanAmount < 0)
            {
                throw new Exception($"Loan amount is invalid, must be a positve decimal {args[0]}");
            }

            var termValid = Int32.TryParse(args[1], out var term);

            if (!termValid || term < 0)
            {
                throw new Exception($"Loan term is invalid, must be positive integer {args[1]}");
            }

            var interestRateValid = Decimal.TryParse(args[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var interestRate);

            if (!interestRateValid || interestRate < 0)
            {
                throw new Exception($"Interst rate is invalid, must be a positve decimal {args[2]}");
            }

            var payDayValid = Int32.TryParse(args[3], out var payDay);

            if (!payDayValid || payDay < 0)
            {
                throw new Exception($"Starting loan pay day is invalid, must be positive integer {args[3]}");
            }
        }
    }
}
