using SimplCommerce.Module.PaymentERede.Models;
using Xunit;

namespace SimplCommerce.Module.PaymentERede.Tests
{

    public class StringExtensionTests
    {
        private readonly string[] inputOnlyDigits = new string[9] { "AA123AA000", "AA123AA000BB", "00AA2121BB11", "00AA2121BB11CC", "A1B2C3D4E5F6", "1111", null, string.Empty, "ABC" };
        private readonly string[] outputOnlyDigits = new string[9] { "123000", "123000", "00212111", "00212111", "123456", "1111", string.Empty, string.Empty, string.Empty };

        [Fact]
        public void OnlyDigits()
        {
            for (int i = 0; i < inputOnlyDigits.Length; i++)
            {
                Assert.Equal(outputOnlyDigits[i], inputOnlyDigits[i].OnlyDigits());
            }
        }

        private readonly string[] inputMonthYearSplit = new string[9] { "02/20", "  /  ", "  0  /  0 ", null, string.Empty, "A1A", "A/B", "01/", "/01" };
        private readonly object[] outputMonthYearSplit = new object[9] { new string[2] { "02", "20" }, new string[2] { string.Empty, string.Empty }, new string[2] { "0", "0" }, new string[2] { string.Empty, string.Empty }, new string[2] { string.Empty, string.Empty }, new string[2] { string.Empty, string.Empty }, new string[2] { string.Empty, string.Empty }, new string[2] { string.Empty, string.Empty }, new string[2] { string.Empty, string.Empty } };

        [Fact]
        public void MonthYearSplit()
        {
            for (int i = 0; i < inputMonthYearSplit.Length; i++)
            {
                var my = inputMonthYearSplit[i].MonthYearSplit();
                Assert.Equal(((string[])outputMonthYearSplit[i])[0], my[0]);
                Assert.Equal(((string[])outputMonthYearSplit[i])[1], my[1]);
            }
        }
    }
}
