using System;

namespace WFShop
{
    readonly struct DiscountAmount : IEquatable<DiscountAmount>
    {
        public readonly IDiscount Discount;
        public readonly decimal Amount;

        public DiscountAmount(IDiscount discount, decimal amount)
        {
            Discount = discount;
            Amount = amount;
        }

        public bool Equals(DiscountAmount other)
            => Discount == other.Discount & Amount == other.Amount;

        public override bool Equals(object obj)
            => obj is DiscountAmount other && this.Equals(other);

        public override int GetHashCode()
            => base.GetHashCode();

        public override string ToString()
            => Discount.Name + " (-" + Amount + ")";

        public static bool operator ==(in DiscountAmount a, in DiscountAmount b)
            => a.Equals(b);

        public static bool operator !=(in DiscountAmount a, in DiscountAmount b)
            => !a.Equals(b);
    }
}
