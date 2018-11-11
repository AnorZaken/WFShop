namespace WFShop
{
    class CurrencySEK : ICurrencyFormatter
    {
        public static CurrencySEK Instance { get; } = new CurrencySEK(false);
        public static CurrencySEK InstanceInternational { get; } = new CurrencySEK(true);

        protected CurrencySEK(bool international)
            => UseInternationalDesignation = international;

        public string Format(decimal value)
            => $"{value:0.00;-0.00} {Symbol}";

        public string Symbol => UseInternationalDesignation ? "SEK" : "kr";

        public bool UseInternationalDesignation { get; }
    }
}
