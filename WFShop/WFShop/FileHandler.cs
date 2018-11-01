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
        public static List<Product> ReadProductsFromFile(string path)
        { 
            List<Product> products = new List<Product>();
            string[] lines = Array.Empty<string>();
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
                    // Kasta undantag vid eventuell formateringsfel.
                    int serialNumber = int.Parse(commaSeparatedValues[0]);
                    string name = commaSeparatedValues[1];
                    decimal price = decimal.Parse(commaSeparatedValues[2]);
                    string category = commaSeparatedValues[3];
                    string description = commaSeparatedValues[4];
                    // Ingen ny produkt läggs till om ett undantag kastas.
                    products.Add(new Product(serialNumber, name, price, category, description));
                }
                catch (FormatException)
                {
                    // Meddelande: "Textrad lästes inte in korrekt."
                    string errorMessage = $"Rad \"{line}\" lästes inte in korrekt. Var god kontrollera källan.";
                    Console.WriteLine(errorMessage);
                }
                catch (Exception e)
                {
                    // Om undantag kastas av oförväntad anledning.
                    // Meddelande: "Kunde inte läsa in textrad."
                    string errorMessage = $"Fångade oväntat fel på rad \"{line}\". Fördjupad felinformation:\n{e}";
                    Console.WriteLine(errorMessage);
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

        // Läser från textfil med rabattkoder.
        // Ska rabattkod dra av totalkostnaden eller av specifika produkter?
        // Hur ska rabattkoder sammankopplas med dessa produkter?

        // public static ??? ReadCouponCodes(string path) => throw new NotImplementedException();
    }
}
