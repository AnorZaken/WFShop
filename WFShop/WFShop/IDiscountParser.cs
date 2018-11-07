using System;
using System.Collections.Generic;
using System.Linq;

namespace WFShop
{
    interface IDiscountParser<out T> where T : class, IDiscount
    {
        // Parses and returns an appropriate discount instance if possible; otherwise returns null.
        T ParseOrNull(IDictionary<string, string> parsedValues);
    }
}
