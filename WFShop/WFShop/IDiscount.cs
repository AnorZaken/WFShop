using System;
using System.Collections.Generic;
using System.Linq;

namespace WFShop
{
    interface IDiscount
    {
        string Name { get; }
        string Description { get; }

        // Null if this is a rebate and not a cupon-code.
        string CuponCode { get; }

        // Applies to [a] specific product[s] only?
        bool IsProductSpecific { get; }

        // Note: returns -1 if not product specific.
        int ProductSerialNumber { get; }

        bool DoesApply(IReadOnlyDictionary<Product, int> cart);
        decimal Calculate(IReadOnlyDictionary<Product, int> cart);
    }
}
