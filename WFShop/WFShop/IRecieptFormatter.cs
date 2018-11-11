using System.Collections.Generic;

namespace WFShop
{
    interface IRecieptFormatter
    {
        IEnumerable<string> Format(ShoppingCart cart);
    }
}
