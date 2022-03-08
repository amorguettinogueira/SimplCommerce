using System;
using Microsoft.AspNetCore.Http;

namespace SimplCommerce.Module.PaymentERede.Models
{
    public static class ERedeExtensions
    {
        public static readonly byte MONTH = 0;
        public static readonly byte YEAR = 1;

        public static readonly char CREDIT = 'C';
        public static readonly char DEBIT = 'D';

        public static string OnlyDigits(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var charValues = value.ToCharArray();
                int p = -1, i;
                for (i = 0; i < charValues.Length; i++)
                {
                    if (charValues[i] >= '0' && charValues[i] <= '9')
                    {
                        p++;
                        if (p != i)
                            charValues[p] = charValues[i];
                    }
                }
                if (p > -1)
                {
                    if (i == p)
                    {
                        return value;
                    }
                    else
                    {
                        Array.Resize(ref charValues, p + 1);
                        return new string(charValues);
                    }
                }
            }
            return string.Empty;
        }

        public static string[] MonthYearSplit(this string value, string delimiter = "/")
        {
            string Month = string.Empty, Year = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                int p = value.IndexOf(delimiter);
                if (p > 0 && value.Length > p + delimiter.Length)
                {
                    Month = value[..p];
                    Year = value[(p + 1)..];
                }
            }
            return new string[2] { Month.OnlyDigits(), Year.OnlyDigits() };
        }

        public static string GetEndpoint(this HttpRequest request, string endpoint) =>
            request != null && request.Host.HasValue && endpoint != null ? string.Format("{0}://{1}/{2}", request.Scheme, request.Host.Value, endpoint.TrimStart('/')) : string.Empty;
    }
}
