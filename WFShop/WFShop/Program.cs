using System;
using System.Windows.Forms;
using System.IO;

namespace WFShop
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            ImageLoader.PathToFolder = Path.Combine(Environment.CurrentDirectory, "Images");

            InitializeDiscountParsers();
            LoadDiscounts();

            var shop = CreateShop();
            shop.Products.Load();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ShopForm(shop));
        }

        private static void InitializeDiscountParsers()
        {
            Discounts.BuyXPayForY.RegisterParser();
            Discounts.TotalPercentageCoupon.RegisterParser();
        }

        private static Shop CreateShop()
        {
            // Order is important!
            var shop = new Shop(out Shop.ISetter setter); // TODO: give name argument?
            setter.Set(ReceiptSaver());
            setter.Set(ProductProvider());
            setter.Set(CartStorage());
            return shop;

            ReceiptSaver ReceiptSaver()
            {
                var desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var path = Path.Combine(desktopFolder, "receipt.txt");
                return new ReceiptSaver(path, new ReceiptFormatter(CurrencySEK.Instance));
            }

            IProductProvider ProductProvider()
                => new ProductProvider(new ProductLoader("productSortiment.csv"));

            IShoppingCartStorage CartStorage()
            {
                var tempFolder = Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Machine);
                var path = Path.Combine(tempFolder, "cart.csv");
                return new ShoppingCartFileStorage(path, shop.Products);
            }
        }

        private static void LoadDiscounts()
        {
            try
            {
                if (!DiscountLoader.HasDiscountsLoaded)
                    DiscountLoader.Load("discounts.kvg");
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message);
                Environment.Exit(0);
            }
        }
    }
}
