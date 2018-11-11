using System.Linq;

namespace WFShop
{
    partial class ShoppingCart
    {
        private class DisCalcInfo : IDiscountCalculationCartInfo
        {
            readonly private ShoppingCart cart;

            internal DisCalcInfo(ShoppingCart cart)
                => this.cart = cart;

            public int UniqueProductCount => cart.UniqueProductCount;

            public int ArticleCount => cart.ArticleCount;

            public decimal ArticleValue => cart.ArticleValue;

            public bool Contains(int serialNumber)
                => cart.cart.ContainsKey(serialNumber);

            public int GetAmount(int serialNumber)
                => cart.cart.TryGetValue(serialNumber, out ProductAmount pa) ? pa.Amount : 0;

            public bool TryGet(int serialNumber, out ProductAmount productAmount)
                => cart.cart.TryGetValue(serialNumber, out productAmount);

            public decimal AccumulatedDiscount(int serialNumber)
                => cart.p_appliedRebates
                .Where(da => da.Discount.ProductSerialNumber == serialNumber)
                .Sum(da => da.Amount);

            public decimal AccumulatedDiscount()
                => cart.p_appliedRebates.Sum(da => da.Amount);
        }
    }
}
