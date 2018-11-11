using System;
using System.Collections.Generic;

namespace WFShop
{
    class ProductProvider : IProductProvider
    {
        public ProductProvider(IProductLoader loader)
        {
            Loader = loader;
        }

        public IProductLoader Loader { get; }
        public bool HasLoaded { get; private set; }

        private readonly Dictionary<int /*SerialNumber*/, Product> products
            = new Dictionary<int, Product>();

        public void Load() => Load(out _);

        public void Load(out int registrationFailures)
        {
            if (HasLoaded)
                throw new InvalidOperationException("Products already loaded!");
            registrationFailures = 0;
            var loaded = Loader.Load();
            foreach (var pli in loaded)
                if (!TryAddProduct(pli.Product))
                {
                    ++registrationFailures;
                    string errorMessage =
                        $"Kan ej registrera inläst produkt; en produkt med serienummer {pli.SerialNumber} är redan registrerad." +
                        $"(Extra info från IProductLoader: {pli.LoadInfo})";
                    Console.Error.WriteLine(errorMessage);
                }
            HasLoaded = true;
        }

        public void AddProduct(Product product)
        {
            if (!TryAddProduct(product))
                throw new Product.DuplicateException(products[product.SerialNumber], product);
        }

        public bool TryAddProduct(Product product)
        {
            if (products.ContainsKey(product.SerialNumber))
                return false;
            products.Add(product.SerialNumber, product);
            return true;
        }

        public Product Get(int serialNumber)
            => TryGet(serialNumber, out Product p)
            ? p : throw new KeyNotFoundException($"No such product (SN:{serialNumber})");

        public bool TryGet(int serialNumber, out Product product)
        {
            if (!HasLoaded)
                Load(out _);
            return products.TryGetValue(serialNumber, out product);
        }

        public IReadOnlyCollection<Product> All
        {
            get
            {
                if (!HasLoaded)
                    Load(out _);
                return products.Values;
            }
        }
    }
}
