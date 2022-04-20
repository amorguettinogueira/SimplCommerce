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
                        return value;
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

        public static bool Int32InRange(this string value, int min, int max) =>
            string.IsNullOrEmpty(value) ? false : int.TryParse(value, out var result) && (result >= min && result <= max);

        public static void Log2Console(this HttpRequest request, string start = "In", string stop = "Out")
        {
            Console.WriteLine(start);
            foreach (var item in request.Query)
            {
                Console.WriteLine(
                    $"Query {item.Key} = {item.Value}"
                    );
            }
            foreach (var item in request.Headers)
            {
                Console.WriteLine(
                    $"Headers {item.Key} = {item.Value}"
                    );
            }
            foreach (var item in request.RouteValues)
            {
                Console.WriteLine(
                    $"RouteValues {item.Key} = {item.Value}"
                    );
            }
            foreach (var item in request.Form)
            {
                Console.WriteLine(
                    $"Form {item.Key} = {item.Value}"
                    );
            }
            Console.WriteLine($"QueryString {request.QueryString}");
            Console.WriteLine($"ToString() {request}");
            Console.WriteLine(stop);
        }
    }
}
