using System;
using System.Collections.Generic;

namespace WFShop
{
    // gör lite fula trick kanske :P
    public readonly struct ProductCategory : IEquatable<ProductCategory>, IEquatable<string>
    {
        public string Name { get; }
        public string Description => GetDescription(Name);

        public ProductCategory(string name)
        {
            Name = name;
        }
        
        public ProductCategory(string name, string desc)
        {
            Name = name;
            descriptions[name] = desc;
        }

        private static readonly Dictionary<string, string> descriptions = new Dictionary<string, string>();

        public static string GetDescription(string categoryName)
            => descriptions.TryGetValue(categoryName, out string desc) ? desc : null;

        public static void SetDescription(string categoryName, string description)
            => descriptions[categoryName] = description;

        // Beteende: null != null då det antagligen inte är användbart att betrakta dem som equivalenta.
        public bool Equals(string categoryName)
            => Name != null & Name == categoryName;

        // Beteende: null != null då det antagligen inte är användbart att betrakta dem som equivalenta.
        public bool Equals(ProductCategory other)
            => Name != null & Name == other.Name;

        public override bool Equals(object obj)
            => obj is ProductCategory other && this.Equals(other);

        public static bool operator ==(ProductCategory a, ProductCategory b)
            => a.Equals(b);
        public static bool operator !=(ProductCategory a, ProductCategory b)
            => !a.Equals(b);

        public override int GetHashCode()
            => Name == null ? 0 : Name.GetHashCode();

        public override string ToString()
            => Name ?? "";
    }
}
