namespace WFShop
{
    interface IShoppingCartStorage
    {
        bool HasSave { get; }
        void Save(ShoppingCart cart);
        bool TryLoad(out ShoppingCart cart, out int errorCount);
    }
}
