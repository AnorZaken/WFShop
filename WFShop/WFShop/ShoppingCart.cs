using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFShop
{
    /* För att läsa ut varorna:
     * foreach (var entry in shoppingCart.Products)
     * {
     *     var product = entry.Product;
     *     var amount = entry.Amount;
     * }
     */
    class ShoppingCart : IEnumerable<ProductEntry>
    {
        private readonly Dictionary<Product, int> cart = new Dictionary<Product, int>();
        private readonly HashSet<Discount> appliedCoupons = new HashSet<Discount>();

        // Antalet unika produkter, dvs har man 4st äpplen och 6st ägg så är antalet unika produkter 2.
        public int UniqueProductCount => cart.Count;

        // dessa variabler är cachade / lazy, och uppdateras vid behov!
        private bool isDirtyArticles = true;
        private bool isDirtyDiscounts = true;
        private decimal p_articleValue, p_discountValue;
        private int p_articleCount;
        private IReadOnlyCollection<Discount> p_appliedRebates = Array.Empty<Discount>();

        // Antalet varor, dvs har man 4st äpplen och 6st ägg så är antalet varor 10.
        public int ArticleCount
            => isDirtyArticles
            ? p_articleCount = cart.Values.Sum()
            : p_articleCount;

        // Totala värdet av varorna i kundvagnen, exklusive rabatter.
        public decimal ArticleValue
            => isDirtyArticles
            ? p_articleValue = CalculateArticleValue()
            : p_articleValue;

        // Totala rabatten på varorna i kundvagnen.
        public decimal DiscountValue
            => isDirtyArticles | isDirtyDiscounts
            ? p_discountValue = CalculateDiscountValue()
            : p_discountValue;

        // Totala kostnaden för varorna i kundvagnen, inklusive rabatter.
        public decimal FinalPrice => ArticleValue - DiscountValue;

        protected decimal CalculateArticleValue()
            => cart.Sum(kvp => kvp.Key.Price * kvp.Value);

        protected IEnumerable<Discount> FindApplicableRebates()
            => Discount.AllRebates.Where(d => d.IsApplicable(cart));

        protected decimal CalculateDiscountValue()
            => AppliedRebates.Sum(d => d.Calculate(cart)) + appliedCoupons.Sum(d => d.Calculate(cart));

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

        public bool AddCoupon(string couponCode)
            => Discount.TryGetCoupon(couponCode, out Discount d) && AddCoupon(d);

        public bool RemoveCoupon(string couponCode)
            => Discount.TryGetCoupon(couponCode, out Discount d) && RemoveCoupon(d);

        public void Add(Product product, int amount = 1)
        {
            if (amount < 1)
                throw new ArgumentOutOfRangeException(nameof(amount), "Cannot add less than one product.");

            if (cart.TryGetValue(product, out int existingAmount))
                amount += existingAmount;
            cart[product] = amount;
            isDirtyArticles = true;
        }

        public bool RemoveAll(Product product)
        {
            bool b = cart.Remove(product);
            isDirtyArticles |= b;
            return b;
        }

        public bool Remove(Product product, int amount)
        {
            if (amount < 1)
                throw new ArgumentOutOfRangeException(nameof(amount), "Cannot remove less than one product.");

            if (cart.TryGetValue(product, out int existingAmount))
            {
                existingAmount -= amount;
                if (existingAmount > 0)
                    cart[product] = existingAmount;
                else
                    cart.Remove(product);
                isDirtyArticles = true;
                return true;
            }
            return false;
        }

        public IEnumerable<ProductEntry> Products
            => this;

        public IReadOnlyCollection<IDiscount> AppliedCoupons
            => appliedCoupons.ToArray();

        public IReadOnlyCollection<IDiscount> AppliedRebates
            => isDirtyArticles
            ? p_appliedRebates = FindApplicableRebates().ToArray()
            : p_appliedRebates;

        IEnumerator<ProductEntry> IEnumerable<ProductEntry>.GetEnumerator()
            => cart
            .OrderBy(kvp => kvp.Key.Name, StringComparer.CurrentCulture)
            .Select(kvp => new ProductEntry(kvp.Key, kvp.Value))
            .GetEnumerator();

        //public IEnumerator<Entry> GetEnumerator()
        //{
        //    var sortedCart = cart.OrderBy(kvp => kvp.Key.Name, StringComparer.CurrentCulture);
        //    foreach (var kvp in sortedCart)
        //    {
        //        yield return new Entry(kvp.Key, kvp.Value);
        //    }
        //}

        //void Exempel()
        //{
        //    var shoppingCart = this;
        //    foreach(var entry in shoppingCart)
        //    {
        //        Console.WriteLine(entry.Product.Name + " x" + entry.Amount);
        //    }
        //}

        //void Exempél()
        //{
        //    var shoppingCart = this;
        //    using (IEnumerator<Entry> enumerator = shoppingCart.GetEnumerator())
        //    {
        //        while (enumerator.MoveNext())
        //        {
        //            var entry = enumerator.Current;
        //            Console.WriteLine(entry.Product.Name + " x" + entry.Amount);
        //        }
        //    }
        //}

        //public IEnumerator<Entry> GetEnumerator()
        //{
        //    var sortedEntryList = cart.OrderBy(kvp => kvp.Key.Name, StringComparer.CurrentCulture)
        //            .Select(kvp => new Entry(kvp.Key, kvp.Value)).ToList();
        //    return new MyEnumerator(sortedEntryList);
        //}

        //private class MyEnumerator : IEnumerator<Entry>
        //{
        //    public MyEnumerator(List<Entry> list)
        //    {
        //        entries = list;
        //    }

        //    List<Entry> entries;
        //    int index = 0;

        //    public Entry Current { get; private set; }

        //    object IEnumerator.Current { get { return Current; } }

        //    public void Dispose()
        //    { }

        //    public bool MoveNext()
        //    {
        //        if (index >= entries.Count)
        //        {
        //            Current = default;
        //            return false;
        //        }
        //        else
        //        {
        //            Current = entries[index];
        //            return true;
        //        }
        //    }

        //    public void Reset()
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<ProductEntry>)this).GetEnumerator();
    }
}
