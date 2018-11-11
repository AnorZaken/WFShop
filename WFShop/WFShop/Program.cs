using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace WFShop
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            ImageHandler.PathToFolder = Path.Combine(Environment.CurrentDirectory, "Images");

            InitializeDiscountParsers();
            LoadDiscounts();

            var shop = CreateShop();
            shop.Products.Load();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MyForm(shop));
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
            DiscountLoader.PathToDiscounts = "discounts.kvg";
            try
            {
                if (!DiscountLoader.HasDiscountsLoaded)
                    DiscountLoader.LoadDiscounts();
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message);
                Environment.Exit(0);
            }
        }
    }
}
