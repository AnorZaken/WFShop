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
    class ShoppingCart : IEnumerable<ShoppingCart.Entry>
    {
        private Dictionary<Product, int> cart = new Dictionary<Product, int>();

        // Antalet unika produkter, dvs har man 4st äpplen och 6st ägg så är antalet unika produkter 2.
        public int UniqueCount => cart.Count;

        // Antalet varor, dvs har man 4st äpplen och 6st ägg så är antalet varor 10.
        public int TotalCount => cart.Values.Sum();

        // Totala kostnaden för varorna i kundvagnen.
        public decimal TotalCost => cart.Sum(kvp => kvp.Key.Price * kvp.Value);
        // (skulle istället kunna uppdatera detta värdet varje gång vi lägger till och tar bort produkter)

        public void Add(Product product, int amount = 1)
        {
            if (amount < 1)
                throw new ArgumentOutOfRangeException(nameof(amount), "Cannot add less than one product.");

            if (cart.TryGetValue(product, out int existingAmount))
                amount += existingAmount;
            cart[product] = amount;
        }

        public void RemoveAll(Product product)
        {
            cart.Remove(product);
        }

        public void Remove(Product product, int amount)
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
            }
        }

        public IEnumerator<Entry> GetEnumerator()
            => cart
            .OrderBy(kvp => kvp.Key.Name, StringComparer.CurrentCulture)
            .Select(kvp => new Entry(kvp.Key, kvp.Value))
            .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public readonly struct Entry
        {
            public readonly Product Product;
            public readonly int Amount;

            public Entry(Product product, int amount)
            {
                Product = product;
                Amount = amount;
            }
        }
    }
}
