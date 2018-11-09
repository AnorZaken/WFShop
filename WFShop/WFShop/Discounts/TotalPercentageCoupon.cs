using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace WFShop.Discounts
{
    sealed class TotalPercentageCoupon : Discount
    {
        const string TYPE = @"T%C";

        private TotalPercentageCoupon(string name, string desc, string couponCode, decimal percentage, decimal minimumCartValue)
            : base(name, desc, TYPE, couponCode: couponCode)
        {
            this.Percentage = percentage;
            this.MinCartValue = minimumCartValue;
        }

        // Percentage off ]0.00 - 1.00[.
        public readonly decimal Percentage;

        // Minimum required cart value for coupon to be applicable.
        public readonly decimal MinCartValue;

        private decimal Sum(IReadOnlyDictionary<Product, int> cart)
            => cart.Sum(kvp => kvp.Key.Price * kvp.Value);

        protected sealed override decimal CalculateImpl(IReadOnlyDictionary<Product, int> cart)
        {
            var sum = Sum(cart);
            return sum >= MinCartValue ? sum * Percentage : 0;
        }

        protected sealed override decimal CalculateImpl(IReadOnlyDictionary<Product, int> cart, decimal totalAppliedRebate)
        {
            var sum = Sum(cart) - totalAppliedRebate;
            return sum >= MinCartValue ? sum * Percentage : 0;
        }

        public sealed override bool IsApplicable(IReadOnlyDictionary<Product, int> cart)
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
                var style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowTrailingWhite;
                var culture = CultureInfo.InvariantCulture;
                if (parsedValues.Count == keys.Length &&
                    parsedValues.TryGetValue(keys[0], out string type) &&
                    StringComparer.OrdinalIgnoreCase.Equals(type, TYPE) &&
                    parsedValues.TryGetValue(keys[1], out string name) &&
                    parsedValues.TryGetValue(keys[2], out string desc) &&
                    parsedValues.TryGetValue(keys[3], out string couponCode) &&
                    parsedValues.TryGetValue(keys[4], out string s_percent) &&
                    parsedValues.TryGetValue(keys[5], out string s_minValue) &&
                    decimal.TryParse(s_percent, style, culture, out decimal percentage) &&
                    decimal.TryParse(s_minValue, style, culture, out decimal minValue))
                {
                    couponCode = couponCode?.Trim();
                    if (!(couponCode?.Length >= 3))
                        throw new FormatException("Coupon codes must be at least 3 characters long (ignoring leading / trailing whitespace)");
                    if (percentage <= 0 || percentage >= 100)
                        throw new FormatException("Percentage must be greater than 0.0 and less than 100.0.");
                    if (minValue < 0)
                        throw new FormatException("MinCartValue cannot be negative.");
                    return new TotalPercentageCoupon(name, desc, couponCode, percentage / 100, minValue);
                }
                return null;
            }
        }
        #endregion
    }
}
