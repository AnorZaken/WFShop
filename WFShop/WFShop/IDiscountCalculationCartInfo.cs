namespace WFShop
{
    // The purpose of this interface is to decouple IDiscount from ShoppingCart.
    interface IDiscountCalculationCartInfo : IDiscountApplicableCartInfo
    {
        // Returns 0 if no rebate has been calculated for this product *yet*.
        decimal AccumulatedDiscount(int serialNumber);

        // Returns the sum of rebates calculated *so far*.
        decimal AccumulatedDiscount();
    }
}
