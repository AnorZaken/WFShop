using System;
using System.Collections.Generic;
using System.Globalization;

namespace WFShop.Discounts
{
    sealed class BuyXPayForY : Discount
    {
        const string TYPE = "BXP4Y";

        private BuyXPayForY(string name, string desc, int productSerialNumber, int buyX, int payY)
            : base(name, desc, TYPE, productSerialNumber)
        {
            this.BuyX = buyX;
            this.PayY = payY;
        }

        public readonly int BuyX;
        public readonly int PayY;

        protected sealed override decimal CalculateImpl(IReadOnlyDictionary<Product, int> cart)
        {
            if (Product.TryGet(ProductSerialNumber, out Product p) &&
                cart.TryGetValue(p, out int cartAmount))
            {
                // Här borde vi assert cartAmount >= 0 egentligen!
                int rebateCount = Math.Max(0, cartAmount) / BuyX;
                return rebateCount * (BuyX - PayY) * p.Price;
            }
            return 0;
        }

        public sealed override bool IsApplicable(IReadOnlyDictionary<Product, int> cart)
            => Product.TryGet(ProductSerialNumber, out Product p)
            && cart.TryGetValue(p, out int amount)
            && amount >= BuyX;

        #region --- Parsing (static) ---
        /* Some third party must call the RegisterParser() function to make this
         * Discount type/class available for parsing from Discount.TryParse(...)
         */
        public static readonly IDiscountParser<BuyXPayForY> Parser = new DP();

        public static void RegisterParser()
            => RegisterParser(Parser);

        private class DP : IDiscountParser<BuyXPayForY>
        {
            private static readonly string[] keys =
            {
                "Type", "Name", "Desc", "ProductSN", "BuyX", "PayY"
            };

            public BuyXPayForY ParseOrNull(IDictionary<string, string> parsedValues)
            {
                var style = NumberStyles.Integer;
                var culture = CultureInfo.InvariantCulture;
                if (parsedValues.Count == keys.Length &&
                    parsedValues.TryGetValue(keys[0], out string type) &&
                    StringComparer.OrdinalIgnoreCase.Equals(type, TYPE) &&
                    parsedValues.TryGetValue(keys[1], out string name) &&
                    parsedValues.TryGetValue(keys[2], out string desc) &&
                    parsedValues.TryGetValue(keys[3], out string s_psn) &&
                    parsedValues.TryGetValue(keys[4], out string s_buy) &&
                    parsedValues.TryGetValue(keys[5], out string s_pay) &&
                    int.TryParse(s_psn, style, culture, out int psn) &&
                    int.TryParse(s_buy, style, culture, out int buy) &&
                    int.TryParse(s_pay, style, culture, out int pay))
                {
                    if (pay <= 0 || buy <= pay)
                        throw new FormatException("Requirement unmet: Buy quantity > Pay quantity > 0");
                    return new BuyXPayForY(name, desc, psn, buy, pay);
                }
                return null;
            }
        }
        #endregion
    }
}
