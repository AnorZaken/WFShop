using System;
using System.Collections.Generic;
using System.Linq;

namespace WFShop
{
    interface IDiscount
    {
        string Name { get; }
        string Description { get; }

        // Null if this is a fixed rebate and not a cupon-code.
        string CuponCode { get; }

        // Applies to [a] specific product[s] only?
        bool IsProductSpecific { get; }

        // Note: returns -1 if not product specific.
        int ProductSerialNumber { get; }

        bool DoesApply(List<Product> products);
        decimal Calculate(List<Product> products);
    }
}
