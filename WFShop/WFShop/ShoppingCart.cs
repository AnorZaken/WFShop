using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFShop
{
    /* För att läsa ut varorna:
     * foreach (var entry in shoppingCart)
     * {
     *     var product = entry.Product;
     *     var amount = entry.Amount;
     * }
     */
    partial class ShoppingCart
    {
        private readonly Dictionary<int, ProductAmount> cart = new Dictionary<int, ProductAmount>();
        private readonly HashSet<Discount> appliedCoupons = new HashSet<Discount>();
        private readonly DisCalcInfo disCalcInfo;

        public ShoppingCart()
            => disCalcInfo = new DisCalcInfo(this);

        // Antalet unika produkter, dvs har man 4st äpplen och 6st ägg så är antalet unika produkter 2.
        public int UniqueProductCount => cart.Count;

        // dessa variabler är cachade / lazy, och uppdateras vid behov!
        private bool isDirtyArticles = true; //TODO: dessa behöver splittas!
        private bool isDirtyDiscounts = true; //TODO: dessa sätts aldrig false!
        private decimal p_articleValue, p_discountValue, p_rebateValue;
        private int p_articleCount;
        private IReadOnlyCollection<DiscountAmount> p_appliedRebates = Array.Empty<DiscountAmount>();

        // Antalet varor, dvs har man 4st äpplen och 6st ägg så är antalet varor 10.
        public int ArticleCount
            => isDirtyArticles
            ? p_articleCount = cart.Values.Sum(pe => pe.Amount)
            : p_articleCount;

        // Totala värdet av varorna i kundvagnen, exklusive rabatter.
        public decimal ArticleValue
            => isDirtyArticles
            ? p_articleValue = CalculateArticleValue()
            : p_articleValue;

        public decimal RebateValue
            => isDirtyArticles | isDirtyDiscounts
            ? p_rebateValue = CalculateRebatesValue()
            : p_rebateValue;

        // Totala rabatten på varorna i kundvagnen.
        public decimal DiscountValue
            => isDirtyArticles | isDirtyDiscounts
            ? p_discountValue = CalculateDiscountValue()
            : p_discountValue;

        // Totala kostnaden för varorna i kundvagnen, inklusive rabatter.
        public decimal FinalPrice
            => Math.Round(ArticleValue - DiscountValue, 2, MidpointRounding.AwayFromZero);

        protected decimal CalculateArticleValue()
            => cart.Sum(kvp => kvp.Value.Cost);

        protected IEnumerable<Discount> FindApplicableRebates()
            => Discount.AllRebates.Where(d => d.IsApplicable(disCalcInfo));

        protected decimal CalculateRebatesValue()
            => AppliedRebates.Sum(de => de.Amount);

        protected decimal CalculateDiscountValue()
            => RebateValue + appliedCoupons.Sum(d => d.Calculate(disCalcInfo));

        public bool AddCoupon(Discount coupon)
        {
            if (coupon == null)
                throw new ArgumentNullException();
            if (coupon.CouponCode != null && appliedCoupons.Add(coupon))
            {
                isDirtyDiscounts = true;
                return true;
            }
            return false;
        }

        public bool RemoveCoupon(Discount coupon)
        {
            if (coupon == null)
                throw new ArgumentNullException();
            bool b = appliedCoupons.Remove(coupon);
            isDirtyDiscounts |= b;
            return b;
        }

        public void ClearCoupons()
        {
            if (appliedCoupons.Count != 0)
            {
                appliedCoupons.Clear();
                isDirtyDiscounts = true;
            }
        }

        public bool AddCoupon(string couponCode)
            => Discount.TryGetCoupon(couponCode, out Discount d) && AddCoupon(d);

        public bool RemoveCoupon(string couponCode)
            => Discount.TryGetCoupon(couponCode, out Discount d) && RemoveCoupon(d);

        public void Add(ProductAmount productEntry)
            => Add(productEntry.Product, productEntry.Amount);

        public void Add(Product product, int amount = 1)
        {
            if (amount < 1)
                throw new ArgumentOutOfRangeException(nameof(amount), "Cannot add less than one product.");

            if (cart.TryGetValue(product.SerialNumber, out ProductAmount existing))
                cart[product.SerialNumber] = existing + amount;
            else
                cart[product.SerialNumber] = new ProductAmount(product, amount);
            isDirtyArticles = true;
        }

        public bool RemoveAll(Product product)
        {
            bool b = cart.Remove(product.SerialNumber);
            isDirtyArticles |= b;
            return b;
        }

        public void Clear()
        {
            cart.Clear();
            isDirtyArticles = true;
        }

        public bool Remove(Product product, int amount = 1)
        {
            if (amount < 1)
                throw new ArgumentOutOfRangeException(nameof(amount), "Cannot remove less than one product.");

            if (cart.TryGetValue(product.SerialNumber, out ProductAmount existing))
            {
                existing -= amount;
                if (existing.Amount > 0)
                    cart[product.SerialNumber] = existing;
                else
                    cart.Remove(product.SerialNumber);
                isDirtyArticles = true;
                return true;
            }
            return false;
        }

        public IEnumerable<ProductAmount> Products
            => cart.Values;

        public IReadOnlyCollection<DiscountAmount> AppliedCoupons
            => appliedCoupons.Select(c => new DiscountAmount(c, c.Calculate(disCalcInfo))).ToArray();

        public IReadOnlyCollection<DiscountAmount> AppliedRebates
            => isDirtyArticles
            ? p_appliedRebates = FindApplicableRebates().Select(r => new DiscountAmount(r, r.Calculate(disCalcInfo))).ToArray()
            : p_appliedRebates;

        public bool HasRebate(int productSerialNumber) // TODO - fixa bättre!
            => AppliedRebates.Any(d => d.Discount.ProductSerialNumber == productSerialNumber);

        public bool TryGetRebate(int productSerialNumber, out DiscountAmount discountEntry)
        {
            // TODO - fixa bättre!
            discountEntry = AppliedRebates.FirstOrDefault(d => d.Discount.ProductSerialNumber == productSerialNumber);
            return discountEntry.Discount != null;
        }

        public IEnumerator<ProductAmount> GetEnumerator()
            => Products
            .OrderBy(pe => pe.Product.Name, StringComparer.CurrentCulture)
            .GetEnumerator();
    }
}
