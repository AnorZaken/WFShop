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
            LoadProductsAndDiscounts();

            var shop = CreateShop();
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
            var desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var pathToReciept = Path.Combine(desktopFolder, "receipt.txt");
            var shop = new Shop(); // TODO: give name argument?
            shop.InitializeRecieptSaver(pathToReciept, new RecieptFormatter(CurrencySEK.Instance));
            return shop;
        }

        private static void InitializeFilePaths()
        {
            var tempFolder = Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Machine);
            FileHandler.PathToProducts = "productSortiment.csv";
            FileHandler.PathToDiscounts = "discounts.kvg";
            FileHandler.PathToCart = Path.Combine(tempFolder, "cart.csv");
            ImageHandler.PathToFolder = Path.Combine(Environment.CurrentDirectory, "Images");
        }

        private static void LoadProductsAndDiscounts()
        {
            //if (!File.Exists(FileHandler.PathToCart))
            //{
            //    using (StreamWriter streamWriter = new StreamWriter(FileHandler.PathToCart))
            //    {
            //        streamWriter.Write(string.Empty);
            //        streamWriter.Close(); // <- Lyckas inte stänga med File-klassen.
            //    }
            //    //MessageBox.Show($"Skapade filen {FileHandler.PathToCart}.");
            //}
            try
            {
                if (!FileHandler.HasProductsLoaded)
                    FileHandler.LoadProducts();
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
