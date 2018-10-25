using System;
using System.Collections.Generic;
using System.Linq;

namespace WFShop
{
    class Product : IEquatable<Product>
    {
        public int SerialNumber { get; }
        public string Name { get; }
        public decimal Price { get; }
        public ProductCategory Category { get; }
        public string Description { get; }

        public Product(int serialNumber, string name, decimal price, string category , string desc)
        {
            SerialNumber = serialNumber;
            Name = name;
            Price = price;
            Category = new ProductCategory(category);
            Description = desc;
        }

        // Beteende: det som avgör likhet är serienummer, pris, och namn - inget annat!
        public bool Equals(Product other)
            => other != null && (SerialNumber == other.SerialNumber & Price == other.Price & Name == other.Name);

        public override bool Equals(object obj)
            => obj is Product other && this.Equals(other);

        public static bool operator ==(Product a, Product b)
            => a != null && a.Equals(b);
        public static bool operator !=(Product a, Product b)
            => a == null || !a.Equals(b);

        public override int GetHashCode()
            => Price.GetHashCode() + (Name.GetHashCode() + SerialNumber * 29) * 23; // 23 and 29 are prime.

        public override string ToString()
            => $"{Name ?? "<Unknown Product>"} ({SerialNumber})";

        // TODO ...
    }
}
