using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFShop
{
    abstract class Discount : IDiscount
    {
        protected Discount(string name, string desc, string type, int productSerialNumber = -1, string cuponCode = null, bool register = true)
        {
            Name = name;
            Description = desc;
            CuponCode = string.IsNullOrWhiteSpace(cuponCode) ? null : cuponCode; // normalisera tomma strängar till null.
            Type = type;
            ProductSerialNumber = productSerialNumber;

            RegisterDiscount(this);
        }

        // dessa är readonly basklass properties: vi vill inte att de mystiskt ändras under run-time.
        public string Name { get; }
        public string Description { get; }
        public string CuponCode { get; }
        public string Type { get; }
        public bool IsProductSpecific => ProductSerialNumber != -1;
        public int ProductSerialNumber { get; }

        public abstract bool DoesApply(IReadOnlyDictionary<Product, int> cart);
        public abstract decimal Calculate(IReadOnlyDictionary<Product, int> cart);

        #region --- All Discounts (static) ---
        private static HashSet<Discount> rebates = new HashSet<Discount>();
        private static Dictionary<string, Discount> cupons = new Dictionary<string, Discount>(); // <CuponCode, Discount>

        public static IReadOnlyCollection<Discount> AllRebates => rebates.ToArray();
        public static IReadOnlyCollection<Discount> AllCupons => cupons.Values;
        public static IReadOnlyCollection<string> AllCuponCodes => cupons.Keys;

        public static bool TryGetCupon(string cuponCode, out Discount discount)
            => cupons.TryGetValue(cuponCode, out discount);

        protected static bool RegisterDiscount(Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException();
            if (discount.CuponCode == null)
                return rebates.Add(discount);
            else if (cupons.ContainsKey(discount.CuponCode))
                return false;

            cupons.Add(discount.CuponCode, discount);
            return true;
        }
        #endregion

        #region --- Parsing (static) ---
        private static HashSet<IDiscountParser<Discount>> parsers = new HashSet<IDiscountParser<Discount>>();

        public static IReadOnlyCollection<IDiscountParser<Discount>> Parsers => parsers.ToArray();

        protected static bool RegisterParser(IDiscountParser<Discount> parser)
            => parsers.Add(parser);

        public static bool TryParse(IDictionary<string, string> parsedValues, out Discount discount)
        {
            foreach (var p in parsers)
                if (p.TryParse(parsedValues, out discount))
                    return true;
            discount = null;
            return false;
        }
        #endregion
    }
}
