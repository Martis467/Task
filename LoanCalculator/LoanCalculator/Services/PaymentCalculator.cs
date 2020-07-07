using System;
using System.Collections.Generic;
using System.Linq;
using LoanCalculator.Models;

namespace LoanCalculator.Services
{
    public class PaymentCalculator
    {
        private readonly LoanParameters _parameters;
        private decimal _remainingLoanAmount;
        private decimal _remaindingTerm;
        private decimal _interestRate;
        private DateTime _currentDate;

        public PaymentCalculator(LoanParameters parameters)
        {
            _parameters = parameters;
            _remainingLoanAmount = parameters.LoanAmount;
            _remaindingTerm = parameters.Term;
            _interestRate = parameters.InterestRate / 100;

            var now = DateTime.Now;
            _currentDate = new DateTime(now.Year, now.Month, parameters.PayDay);
        }

        public PaymentGraph GetPaymentGraph()
        {
            var payments = new List<PaymentEntry>();
            var monthlyPayment = GetMonthlyPayment();

            for (int i = 0; i < _parameters.Term; i+=12)
            {
                CalculateYearlyPayments(payments, monthlyPayment);
            }

            // Adjusting the first payment date to today
            payments.FirstOrDefault().Date = DateTime.Now;

            return new PaymentGraph
            {
                PaymentEntries = payments
            };
        }

        private void CalculateYearlyPayments(List<PaymentEntry> payments, decimal monthlyPayment)
        {
            for (int i = 0; i < 12 && _remaindingTerm > 0; i++)
            {
                var interestPart = _remainingLoanAmount * (_interestRate / 12);
                var creditPart = monthlyPayment - interestPart;

                payments.Add(new PaymentEntry
                {
                    Date = GetPaymentDate(_currentDate),
                    RemainderBeforePayment = _remainingLoanAmount,
                    CreditPart = creditPart,
                    InterestPart = interestPart,
                    Payment = monthlyPayment,
                    RemainderAfterPayment = _remainingLoanAmount - creditPart
                });

                _currentDate = _currentDate.AddMonths(1);
                _remainingLoanAmount -= creditPart;
                _remaindingTerm--;
            }
        }

        private DateTime GetPaymentDate(DateTime date)
        {
            // Needed to adjust if the payment date let's say is the 31st.
            // And our month has less days, so we pay at the last day of month
            var lastDayOfMonth = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddSeconds(-1).Day;

            var day = lastDayOfMonth < _parameters.PayDay ?
                lastDayOfMonth :
                _parameters.PayDay;

            return new DateTime(date.Year, date.Month, day);
        }

        private decimal GetMonthlyPayment()
        {
            var numerator = 1 - Math.Pow((double)(1 + _interestRate / 12), -1 * _parameters.Term);
            var denominator = (double)_interestRate / 12;

            var mothlyPayment = (double)_parameters.LoanAmount / (numerator / denominator);

            // Some rounding
            return Math.Round(((decimal)(mothlyPayment * 10_000))) / 10_000; 
        }
    }
}
