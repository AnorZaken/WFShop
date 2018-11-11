namespace WFShop
{
    class Shop
    {
        public Shop(out ISetter setter, string name = "Shop")
        {
            Name = name;
            setter = new Setter(this);
        }

        readonly public string Name;
        
        public ReceiptSaver Receipt { get; private set; }
        public IProductProvider Products { get; private set; }
        public IShoppingCartStorage CartStorage { get; private set; }

        // Gör så att bara den kod som skapar en Shop instansen får access till setters!
        public interface ISetter
        {
            void Set(ReceiptSaver receiptSaver);
            void Set(IProductProvider productProvider);
            void Set(IShoppingCartStorage cartLoader);
        }
        protected class Setter : ISetter
        {
            internal Setter(Shop shop)
                => this.shop = shop;

            readonly Shop shop;

            public void Set(ReceiptSaver rs) => shop.Receipt = rs;
            public void Set(IProductProvider pp) => shop.Products = pp;
            public void Set(IShoppingCartStorage ss) => shop.CartStorage = ss;
        }
    }
}
