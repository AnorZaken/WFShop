using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFShop.Discounts
{
    class TotalPercentageCoupon : Discount
    {
        const string TYPE = @"T%C";

        private TotalPercentageCoupon(string name, string desc, string couponCode, decimal percentage, decimal minimumCartValue)
            : base(name, desc, TYPE, couponCode: couponCode)
        {
            this.Percentage = percentage;
            this.MinCartValue = minimumCartValue;
        }

        public readonly decimal Percentage;
        public readonly decimal MinCartValue;

        private decimal Sum(IReadOnlyDictionary<Product, int> cart)
            => cart.Sum(kvp => kvp.Key.Price * kvp.Value);

        public override decimal Calculate(IReadOnlyDictionary<Product, int> cart)
        {
            var sum = Sum(cart);
            return sum >= MinCartValue ? sum * Percentage : 0;
        }

        public override bool IsApplicable(IReadOnlyDictionary<Product, int> cart)
            => Sum(cart) >= MinCartValue;

        #region --- Parsing (static) ---
        /* Some third party must call the RegisterParser() function to make this
         * Discount type/class available for parsing from Discount.TryParse(...)
         */
        public static readonly IDiscountParser<TotalPercentageCoupon> Parser = new DP();

        public static void RegisterParser()
            => RegisterParser(Parser);

        private class DP : IDiscountParser<TotalPercentageCoupon>
        {
            private static readonly string[] keys =
            {
                "Type", "Name", "Desc", "CouponCode", "Percentage", "MinCartValue"
            };

            public TotalPercentageCoupon ParseOrNull(IDictionary<string, string> parsedValues)
            {
                if (parsedValues.Count == keys.Length &&
                    parsedValues.TryGetValue(keys[0], out string type) &&
                    StringComparer.OrdinalIgnoreCase.Equals(type, TYPE) &&
                    parsedValues.TryGetValue(keys[1], out string name) &&
                    parsedValues.TryGetValue(keys[2], out string desc) &&
                    parsedValues.TryGetValue(keys[3], out string couponCode) &&
                    parsedValues.TryGetValue(keys[4], out string s_percent) &&
                    parsedValues.TryGetValue(keys[5], out string s_minValue) &&
                    decimal.TryParse(s_percent, out decimal percentage) &&
                    decimal.TryParse(s_minValue, out decimal minValue))
                {
                    couponCode = couponCode?.Trim();
                    if (!(couponCode?.Length >= 3))
                        throw new FormatException("Coupon codes must be at least 3 characters long (ignoring leading / trailing whitespace)");
                    if (percentage <= 0 || percentage >= 1)
                        throw new FormatException("Percentage must be greater than 0.0 and less than 1.0.");
                    if (minValue < 0)
                        throw new FormatException("MinCartValue cannot be negative.");
                    return new TotalPercentageCoupon(name, desc, couponCode, percentage, minValue);
                }
                return null;
            }
        }
        #endregion
    }
}
