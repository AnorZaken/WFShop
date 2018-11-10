using System;

namespace WFShop
{
    readonly struct ProductAmount : IEquatable<ProductAmount>
    {
        public readonly Product Product;
        public readonly int Amount;

        public int SerialNumber => Product?.SerialNumber ?? -1;

        public ProductAmount(Product product, int amount)
        {
            Product = product;
            Amount = amount;
        }

        public bool Equals(ProductAmount other)
            => Product == other.Product & Amount == other.Amount;

        public override bool Equals(object obj)
            => obj is Product other && this.Equals(other);

        public override int GetHashCode()
            => base.GetHashCode();

        public override string ToString()
            => Product.ToString() + " x" + Amount;

        public static bool operator ==(in ProductAmount a, in ProductAmount b)
            => a.Equals(b);

        public static bool operator !=(in ProductAmount a, in ProductAmount b)
            => !a.Equals(b);

        public static ProductAmount operator +(in ProductAmount pe, int amount)
            => new ProductAmount(pe.Product, pe.Amount + amount);

        public static ProductAmount operator -(in ProductAmount pe, int amount)
            => new ProductAmount(pe.Product, pe.Amount - amount);
    }
}
