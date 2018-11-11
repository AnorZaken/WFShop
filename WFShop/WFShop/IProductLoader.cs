using System.Collections.Generic;

namespace WFShop
{
    interface IProductLoader
    {
        IEnumerable<ProductLoadInfo> Load();
    }
}
