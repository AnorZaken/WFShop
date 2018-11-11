using System.Collections.Generic;

namespace WFShop
{
    interface IProductProvider
    {
        bool HasLoaded { get; }
        void Load();
        // Gets a product; or throws a KeyNotFoundException.
        Product Get(int serialNumber);
        bool TryGet(int serialNumber, out Product product);
        IReadOnlyCollection<Product> All { get; }
    }
}
