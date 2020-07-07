using System;

namespace LoanCalculator.Services
{
    public class BVKKMNCalculator
    {
        private const double cnL_IT_STEP = 0.00001;
        private const double cnL_IT_EPSILON = 0.0000001;

        private double _term;
        private double _monthlyPayment;
        private double _currentLoanAmount;

        public BVKKMNCalculator(int term, decimal monthlyPayment, decimal loanAmount)
        {
            _term = (double)term;
            _monthlyPayment = (double)monthlyPayment;
            _currentLoanAmount = (double)loanAmount * -1;
        }

        public decimal Calculate()
        {
            var rateOfInterest = Rate(_term, _monthlyPayment, _currentLoanAmount) * 12;
            var ear = Math.Pow(1.0d + rateOfInterest / 12, 12) - 1;

            return Math.Round((decimal)(ear * 10_000)) / 10_000;
        }

        private double Rate(double NPer, double Pmt, double PV, double FV = 0, DueDate Due = DueDate.EndOfPeriod, double Guess = 0.1)
        {
            double dTemp;
            double dRate0;
            double dRate1;
            double dY0;
            double dY1;
            int I;

            // Check for error condition
            if (NPer <= 0.0)
                throw new ArgumentException("NPer must by greater than zero");

            dRate0 = Guess;
            dY0 = LEvalRate(dRate0, NPer, Pmt, PV, FV, Due);
            if (dY0 > 0)
                dRate1 = (dRate0 / 2);
            else
                dRate1 = (dRate0 * 2);

            dY1 = LEvalRate(dRate1, NPer, Pmt, PV, FV, Due);

            for (I = 0; I <= 39; I++)
            {
                if (dY1 == dY0)
                {
                    if (dRate1 > dRate0)
                        dRate0 = dRate0 - cnL_IT_STEP;
                    else
                        dRate0 = dRate0 - cnL_IT_STEP * (-1);
                    dY0 = LEvalRate(dRate0, NPer, Pmt, PV, FV, Due);
                    if (dY1 == dY0)
                        throw new ArgumentException("Divide by zero");
                }

                dRate0 = dRate1 - (dRate1 - dRate0) * dY1 / (dY1 - dY0);

                // Secant method of generating next approximation
                dY0 = LEvalRate(dRate0, NPer, Pmt, PV, FV, Due);
                if (Math.Abs(dY0) < cnL_IT_EPSILON)
                    return dRate0;

                dTemp = dY0;
                dY0 = dY1;
                dY1 = dTemp;
                dTemp = dRate0;
                dRate0 = dRate1;
                dRate1 = dTemp;
            }

            throw new ArgumentException("Can not calculate rate");
        }

        private double LEvalRate(double Rate, double NPer, double Pmt, double PV, double dFv, DueDate Due)
        {
            double dTemp1;
            double dTemp2;
            double dTemp3;

            if (Rate == 0.0)
                return (PV + Pmt * NPer + dFv);
            else
            {
                dTemp3 = Rate + 1.0;
                // WARSI Using the exponent operator for pow(..) in C code of LEvalRate. Still got
                // to make sure that they (pow and ^) are same for all conditions
                dTemp1 = Math.Pow(dTemp3, NPer);

                if (Due != 0)
                    dTemp2 = 1 + Rate;
                else
                    dTemp2 = 1.0;
                return (PV * dTemp1 + Pmt * dTemp2 * (dTemp1 - 1) / Rate + dFv);
            }
        }

        enum DueDate
        {
            EndOfPeriod = 0,
            BegOfPeriod = 1
        }
    }
}
