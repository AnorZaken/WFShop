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
            InitializeFilePaths();
            InitializeDiscountParsers();
            LoadDiscounts();

            ImageHandler.PathToFolder = Path.Combine(Environment.CurrentDirectory, "Images");

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
            setter.Set(RecieptSaver());
            setter.Set(ProductProvider());
            setter.Set(CartStorage());
            return shop;

            RecieptSaver RecieptSaver()
            {
                var desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var path = Path.Combine(desktopFolder, "receipt.txt");
                return new RecieptSaver(path, new RecieptFormatter(CurrencySEK.Instance));
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

        private static void InitializeFilePaths()
        {
            FileHandler.PathToDiscounts = "discounts.kvg";
        }

        private static void LoadDiscounts()
        {
            try
            {
                if (!FileHandler.HasDiscountsLoaded)
                    FileHandler.LoadDiscounts();
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message);
                Environment.Exit(0);
            }
        }
    }
}
