using System;
using System.Collections.Generic;

namespace WFShop
{
    class RecieptFormatter : IRecieptFormatter
    {
        public static RecieptFormatter Default { get; } = new RecieptFormatter();

        public RecieptFormatter(ICurrencyFormatter currencyFormatter = null)
        {
            Currency = currencyFormatter ?? CurrencyFormatter.Default;
        }
        
        public ICurrencyFormatter Currency { get; }

        public IEnumerable<string> Format(ShoppingCart cart)
        {
            yield return "Kvitto";
            yield return "";
            decimal sumDiscount = 0;
            foreach (ProductEntry pe in cart)
            {
                yield return $"({pe.SerialNumber})";
                yield return $"{pe.Amount} x {pe.Product.Name} à {Currency.Format(pe.Product.Price)}";
                if (cart.TryGetRebate(pe.SerialNumber, out DiscountEntry de))
                {
                    sumDiscount += de.Amount;
                    yield return $"\tRabatt: {Currency.Format(-de.Amount)} ({de.Discount})";
                    yield return $"\t{Currency.Format(pe.Product.Price * pe.Amount - de.Amount)}";
                }
                else
                {
                    yield return $"\t{Currency.Format(pe.Product.Price * pe.Amount)}";
                }
                yield return "";
            }
            var coupons = cart.AppliedCoupons;
            foreach (var de in coupons)
            {
                sumDiscount += de.Amount;
                yield return $"Rabattkod: \"{de.Discount.CouponCode}\" ({de.Discount})";
                yield return $"\t{Currency.Format(-de.Amount)}";
                yield return "";
            }
            if (sumDiscount != 0)
                yield return $"Total rabatt: {Currency.Format(-sumDiscount)}";
            decimal fp = cart.FinalPrice;
            decimal fpRounded = Math.Round(fp);
            if (fp != fpRounded)
                yield return $"Öresavrundning: {Currency.Format(fpRounded - fp)}";
            yield return $"Att betala: {Currency.Format(fpRounded)}";
        }
    }
}
