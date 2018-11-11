namespace WFShop
{
    readonly struct ProductLoadInfo
    {
        readonly public Product Product;
        readonly public string LoadInfo;
        public int SerialNumber => Product.SerialNumber;

        public ProductLoadInfo(Product product, string loadInfo)
        {
            Product = product;
            LoadInfo = loadInfo;
        }
    }
}
