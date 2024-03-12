using Api.Services;
using System;
using Xunit;

namespace ApiTests
{
    public class PaycheckUnitTests
    {
        // This isn't a great unit test since it relies on Datetime.Now, but it was useful for a manual validation
        //[Theory]
        [InlineData(2020, 1, 1, 4)]
        [InlineData(2020, 12, 1, 3)]
        [InlineData(2025, 1, 1, -1)]
        public void CalculateAgeTest(int year, int month, int day, int expectedResult)
        {
            var birthDate = new DateTime(year, month, day);
            var result = PaycheckService.CalculateAge(birthDate);
            Assert.True(expectedResult == result);
        }

        [Theory]
        [InlineData("26.00", "1")]
        [InlineData("123.45", "4.75")]
        [InlineData("0", "0")]
        [InlineData("-123.45", "-4.75")]
        public void CalculateGrossCompensationPerPayPeriod(string annualSalary, string expectedResult)
        {
            var s = Convert.ToDecimal(annualSalary);
            var er = Convert.ToDecimal(expectedResult);
            var result = PaycheckService.CalculateGrossCompensationPerPayPeriod(s);
            Assert.True(er == result);
        }

        [Theory]
        [InlineData("0", 0, 0, "461.54")]
        [InlineData("0", 1, 0, "738.46")]
        [InlineData("0", 1, 1, "830.77")]
        [InlineData("100000", 0, 0, "538.46")]
        [InlineData("100000", 1, 0, "815.38")]
        [InlineData("100000", 1, 1, "907.69")]
        [InlineData("123456.78", 3, 1, "1479.58")]
        public void CalculateDeductionPerPayPeriod(string annualSalary, int dependentCount, int dependentsOver50Count, string expectedResult)
        {
            var s = Convert.ToDecimal(annualSalary);
            var er = Convert.ToDecimal(expectedResult);
            var result = PaycheckService.CalculateDeductionPerPayPersion(s, dependentCount, dependentsOver50Count);
            Assert.True(er == result);
        }
    }
}
