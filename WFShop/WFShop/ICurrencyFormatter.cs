namespace WFShop
{
    interface ICurrencyFormatter
    {
        string Format(decimal currency);
        string Symbol { get; }
    }
}
