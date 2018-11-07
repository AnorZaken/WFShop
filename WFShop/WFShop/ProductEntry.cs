using System;

namespace WFShop
{
    readonly struct ProductEntry : IEquatable<ProductEntry>
    {
        public readonly Product Product;
        public readonly int Amount;

        public int SerialNumber => Product?.SerialNumber ?? -1;

        public ProductEntry(Product product, int amount)
        {
            Product = product;
            Amount = amount;
        }

        public bool Equals(ProductEntry other)
            => Product == other.Product & Amount == other.Amount;

        public override bool Equals(object obj)
            => obj is Product other && this.Equals(other);

        public override int GetHashCode()
            => base.GetHashCode();

        public override string ToString()
            => Product.ToString() + " x" + Amount;

        public static bool operator ==(in ProductEntry a, in ProductEntry b)
            => a.Equals(b);

        public static bool operator !=(in ProductEntry a, in ProductEntry b)
            => !a.Equals(b);

        public static ProductEntry operator +(in ProductEntry pe, int amount)
            => new ProductEntry(pe.Product, pe.Amount + amount);

        public static ProductEntry operator -(in ProductEntry pe, int amount)
            => new ProductEntry(pe.Product, pe.Amount - amount);
    }
}
