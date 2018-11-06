using System;
using System.Collections.Generic;

namespace WFShop
{
    interface IDiscount
    {
        // User friendly name that could be shown in the shopping cart summary. 
        string Name { get; }

        // User friendly description, e.g. what the discount applies to / conditions.
        // ?? Redundant ??
        string Description { get; }

        // Discount "categoty" (non-user friendly)
        //string Type { get; } // Probably redundant??

        // Null if this is a rebate and not a coupon-code.
        string CouponCode { get; }

        // Applies to [a] specific product[s] only?
        bool IsSingleProduct { get; }

        // Note: returns -1 if not product specific.
        int ProductSerialNumber { get; }

        // Is this discount applicable to a cart containing the specified products (and amounts)?
        bool IsApplicable(IReadOnlyDictionary<Product, int> cart);

        // Calculate how large of a discount this applies to a cart with the specified products (and amounts).
        decimal Calculate(IReadOnlyDictionary<Product, int> cart);
    }
}
