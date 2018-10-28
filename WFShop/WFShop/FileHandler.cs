using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace WFShop
{
    class FileHandler
    {
        public static List<Product> ReadProductsFromFile(string path, int textFileValueCount)
        { 
            List<Product> products = new List<Product>();
            string[] lines = { };
            // Kasta undantag ifall filen inte existerar.
            if (File.Exists(path))
                lines = File.ReadAllLines(path);
            else
                throw new FileNotFoundException($"Kunde inte hitta fil: {path}");

            foreach (var line in lines)
            {
                string[] commaSeparatedValues = line.Split('#');
                try
                {
                    // Kasta undantag ifall antalet värden per rad i textfilen understigs eller överstigs.
                    if (commaSeparatedValues.Length < textFileValueCount || commaSeparatedValues.Length > textFileValueCount)
                        throw new ArgumentOutOfRangeException(nameof(commaSeparatedValues), "Olagligt antal värden lästes in från textfil.");
                    // Kasta undantag vid eventuell formateringsfel.
                    int serialNumber = int.Parse(commaSeparatedValues[0]);
                    string name = commaSeparatedValues[1];
                    decimal price = decimal.Parse(commaSeparatedValues[2]);
                    string category = commaSeparatedValues[3];
                    string description = commaSeparatedValues[4];
                    // Ingen ny produkt läggs till om ett undantag kastas.
                    products.Add(new Product(serialNumber, name, price, category, description));
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw e;
                }
                catch (FormatException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    // Om undantag kastas av oförväntad anledning.
                    throw e;
                }
            }

            return products;
        }

        // Kanske inte behövs?
        private static Image ReadImageFromFile() => throw new NotImplementedException();

        // Metoder som kan användas till att skapa ett kvitto och spara som en fil på datorn.
        public static void CreateReceipt(List<Product> cart) => throw new NotImplementedException();

        public static void CreateReceipt(List<Product> cart, string path) => throw new NotImplementedException();

        // Metoder som kan användas till att spara varukorgen som en fil på datorn.
        public static void SaveShoppingCart(List<Product> cart) => throw new NotImplementedException();

        public static void SaveShoppingCart(List<Product> cart, string path) => throw new NotImplementedException();

        // Kan användas till att läsa in den sparade varukorgen när en ny instans av programmet skapas eller på användarens begäran.
        public static List<Product> LoadShoppingCart(string path) => throw new NotImplementedException();
    }
}
