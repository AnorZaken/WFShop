using System.Collections.Generic;

namespace WFShop
{
    interface IReceiptFormatter
    {
        IEnumerable<string> Format(ShoppingCart cart);
    }
}
