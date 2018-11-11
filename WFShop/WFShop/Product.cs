using System;

namespace WFShop
{
    partial class Product : IEquatable<Product>
    {
        public int SerialNumber { get; }
        public string Name { get; }
        public decimal Price { get; }
        public ProductCategory Category { get; }
        public string Description { get; }

        private Product(int serialNumber, string name, decimal price, string category , string desc)
        {
            SerialNumber = serialNumber;
            Name = name;
            Price = price;
            Category = new ProductCategory(category);
            Description = desc;
        }

        // Public Create method instead of public ctor (remnant of old design).
        public static Product Create(int serialNumber, string name, decimal price, string category, string desc)
        {
            var pNew = new Product(serialNumber, name, price, category, desc);
            return pNew;
        }

        // Likhet ignorerar Description!
        public bool Equals(Product other)
            => !(other is null) &&
            ( SerialNumber == other.SerialNumber
            & Price == other.Price
            & Name == other.Name
            & Category == other.Category );

        public override bool Equals(object obj)
            => obj is Product other && this.Equals(other);

        public static bool operator ==(Product a, Product b)
            => a is null ? b is null : a.Equals(b);
        public static bool operator !=(Product a, Product b)
            => !(a is null ? b is null : a.Equals(b));

        public override int GetHashCode()
            => Price.GetHashCode() + (Name.GetHashCode() + SerialNumber * 29) * 23; // 23 and 29 are prime.

        public override string ToString()
            => $"{Name ?? "<Unknown Product>"} ({SerialNumber})";
    }
}
