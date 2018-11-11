namespace WFShop
{
    // The purpose of this interface is to decouple IDiscount from ShoppingCart.
    interface IDiscountApplicableCartInfo
    {
        // Returns 0 if the cart does not contain that product.
        int GetAmount(int serialNumber);
        bool Contains(int serialNumber);

        // Antalet unika produkter, dvs har man 4st äpplen och 6st ägg så är antalet unika produkter 2.
        int UniqueProductCount { get; }

        // Antalet varor, dvs har man 4st äpplen och 6st ägg så är antalet varor 10.
        int ArticleCount { get; }

        // Totala värdet av varorna i kundvagnen, exklusive rabatter.
        decimal ArticleValue { get; }

        bool TryGet(int serialNumber, out ProductAmount productAmount);
    }
}
