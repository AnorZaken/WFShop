using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFShop
{
    abstract class Discount : IDiscount
    {
        protected Discount(string name, string desc, string type, int productSerialNumber = -1, string CuponCode = null)
        {
            Name = name;
            Description = desc;
            Type = type;
            ProductSerialNumber = productSerialNumber;
        }

        // dessa är readonly basklass properties: vi vill inte att de mystiskt ändras under run-time.
        public string Name { get; }
        public string Description { get; }
        public string CuponCode { get; }
        public string Type { get; }
        public bool IsProductSpecific => ProductSerialNumber != -1;
        public int ProductSerialNumber { get; }

        public abstract bool DoesApply(List<Product> products);
        public abstract decimal Calculate(List<Product> products);

        #region -- Parsing (static) --
        private static HashSet<IDiscountParser<Discount>> parsers;

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
