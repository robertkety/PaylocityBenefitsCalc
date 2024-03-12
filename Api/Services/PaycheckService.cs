namespace Api.Services
{
    public static class PaycheckService
    {
        public static decimal CalculateGrossCompensationPerPayPeriod(decimal annualSalary)
        {
            return Math.Round(annualSalary / 26, 2);
        }

        // todo: Lots of magic numbers here. These would probably be better stored as data. Data schema should allow for regional cost variation
        public static decimal CalculateDeductionPerPayPersion(decimal annualSalary, int dependentCount, int dependentsOver50Count)
        {
            // compute fixed individual benefit cost as 1k/month
            var monthlyBenefitCost = 1000m;

            // compute fixed dependent benefit cost as 600/month/per dependent
            monthlyBenefitCost += dependentCount * 600m;

            // compute conditional benefit cost as annual salary * 2%, when salary > 80k
            monthlyBenefitCost += (annualSalary > 80000 ? annualSalary * 0.02m : 0m) / 12;

            // compute conditional dependent benefit cost as 200/month/per dependent over 50
            monthlyBenefitCost += dependentsOver50Count * 200;

            // convert monthly cost to pay period
            return Math.Round(monthlyBenefitCost * 12 / 26, 2);
        }

        internal static int CalculateAge(DateTime birthDate)
        {
            var age = DateTime.Now.Year - birthDate.Year;
            if (DateTime.Now.DayOfYear < birthDate.DayOfYear)
                age--;
            return age;
        }
    }
}
