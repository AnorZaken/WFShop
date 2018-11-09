using System;
using System.Collections.Generic;
using System.Linq;

namespace WFShop
{
    /* Förklaring av termer:
     * - Rebates - passiva rabatter, t.ex. "Erbjudande, köp 3 risifrutti betala för 2"
     * - Coupons - aktiva rabatter, t.ex. "20% off on your next purchase"
     */
    abstract class Discount : IDiscount
    {
        protected Discount(string name, string desc, string type, int productSerialNumber = -1, string couponCode = null)
        {
            Name = name;
            Description = desc;
            CouponCode = string.IsNullOrWhiteSpace(couponCode) ? null : couponCode; // normalisera tomma strängar till null.
            Type = type;
            ProductSerialNumber = productSerialNumber;
            IsRegistered = RegisterDiscount(this);
        }

        // dessa är readonly basklass properties: vi vill inte att de mystiskt ändras under run-time.
        public string Name { get; }
        public string Description { get; }
        public string CouponCode { get; }
        public string Type { get; }
        public bool IsSingleProduct => ProductSerialNumber != -1;
        public int ProductSerialNumber { get; }
        protected bool IsRegistered { get; }

        public abstract bool IsApplicable(IReadOnlyDictionary<Product, int> cart);

        public decimal Calculate(IReadOnlyDictionary<Product, int> cart)
            => Math.Round(CalculateImpl(cart), 2, MidpointRounding.AwayFromZero);

        public decimal Calculate(IReadOnlyDictionary<Product, int> cart, decimal totalAppliedRebate)
            => Math.Round(CalculateImpl(cart, totalAppliedRebate), 2, MidpointRounding.AwayFromZero);

        protected abstract decimal CalculateImpl(IReadOnlyDictionary<Product, int> cart);
        protected virtual decimal CalculateImpl(IReadOnlyDictionary<Product, int> cart, decimal totalAppliedRebate)
            => CalculateImpl(cart);

        public override string ToString() => Name;

        #region --- All Discounts (static) ---
        /* Alla discounts ligger efter konstruktion statiskt tillgängliga i Discount klassen:
         * - AllCoupons: Alla aktiva rabatter (kräver kupongkod - se även AllCouponCodes).
         * - AllRebates: Alla passiva rabatter (appliceras automatiskt om de matchar varukorgen).
         */

        private static readonly HashSet<Discount> rebates = new HashSet<Discount>();
        private static readonly Dictionary<string, Discount> coupons = new Dictionary<string, Discount>(); // <CouponCode, Discount>

        public static IReadOnlyCollection<Discount> AllRebates => rebates.ToArray();
        public static IReadOnlyCollection<Discount> AllCoupons => coupons.Values;
        public static IReadOnlyCollection<string> AllCouponCodes => coupons.Keys;

        public static bool TryGetCoupon(string couponCode, out Discount discount)
            => coupons.TryGetValue(couponCode, out discount);

        protected static bool RegisterDiscount(Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException();
            if (discount.CouponCode == null)
                return rebates.Add(discount);
            else if (coupons.ContainsKey(discount.CouponCode))
                return false;

            coupons.Add(discount.CouponCode, discount);
            return true;
        }
        #endregion

        #region --- Parsing (static) ---
        /* Alla discount klasser ska registrera sin parser här, så att de kan läsas in.
         * (Någon trejde part behöver göra själva registreringen, t.ex. Main-metoden.)
         * Själva inläsningen involverar 3 parter:
         * - FileHandler: läser in rabatterna från en fil i form av en dictionary per rabatt.
         * - Discount: tar emot dictionary och försöker parsa m.h.a. registrerade parsers.
         * - Tredje part: koden som gör anropen till FileHandler och skickar vidare till Discount.
         * Alla discounts ligger sedan statiskt tillgängliga (se ovan).
         */

        private static readonly HashSet<IDiscountParser<Discount>> parsers = new HashSet<IDiscountParser<Discount>>();

        public static IReadOnlyCollection<IDiscountParser<Discount>> Parsers => parsers.ToArray();

        protected static bool RegisterParser(IDiscountParser<Discount> parser)
            => parsers.Add(parser);

        public static bool TryParse(IDictionary<string, string> parsedValues, out Discount discount)
        {
            foreach (var p in parsers)
                if ((discount = p.ParseOrNull(parsedValues)) != null)
                    return true;
            discount = null;
            return false;
        }
        #endregion
    }
}
