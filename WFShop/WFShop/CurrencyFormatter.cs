using System.Globalization;

namespace WFShop
{
    // This class exists because we can't stick a static Default property in an interface. 
    static class CurrencyFormatter
    {
        private static ICurrencyFormatter p_default = CurrentCulture.Instance;

        // This property is guaranteed to never be null.
        public static ICurrencyFormatter Default
        {
            get => p_default;
            set => p_default = value ?? CurrentCulture.Instance;
        }

        // Default currency formatter.
        sealed public class CurrentCulture : ICurrencyFormatter
        {
            readonly public static CurrentCulture Instance = new CurrentCulture();

            public string Format(decimal value) => value.ToString("C");
            public string Symbol => CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;

            private CurrentCulture() { }
        }
    }
}
