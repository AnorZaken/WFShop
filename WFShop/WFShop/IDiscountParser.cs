using System;
using System.Collections.Generic;
using System.Linq;

namespace WFShop
{
    interface IDiscountParser<T> where T : IDiscount
    {
        bool TryParse(IDictionary<string, string> parsedValues, out T discount);
    }
}
