using System;

namespace WFShop
{
    readonly struct DiscountEntry : IEquatable<DiscountEntry>
    {
        public readonly IDiscount Discount;
        public readonly decimal Amount;

        public DiscountEntry(IDiscount discount, decimal amount)
        {
            Discount = discount;
            Amount = amount;
        }

        public bool Equals(DiscountEntry other)
            => Discount == other.Discount & Amount == other.Amount;

        public override bool Equals(object obj)
            => obj is DiscountEntry other && this.Equals(other);

        public override int GetHashCode()
            => base.GetHashCode();

        public override string ToString()
            => Discount.Name + " (-" + Amount + ")";

        public static bool operator ==(in DiscountEntry a, in DiscountEntry b)
            => a.Equals(b);

        public static bool operator !=(in DiscountEntry a, in DiscountEntry b)
            => !a.Equals(b);
    }
}
