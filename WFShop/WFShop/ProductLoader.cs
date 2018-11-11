using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace WFShop
{
    class ProductLoader : IProductLoader
    {
        public ProductLoader(string path)
            => Path = path;

        public string Path { get; }

        public IEnumerable<ProductLoadInfo> Load()
        {
            string[] lines;
            try
            {
                lines = File.ReadAllLines(Path);
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"Kunde inte hitta produktfilen: \"{Path}\"");
            }
            var products = new List<ProductLoadInfo>(lines.Length);
            var culture = CultureInfo.InvariantCulture;
            for (int iRow = 0; iRow < lines.Length; ++iRow)
            {
                string[] columns = lines[iRow].Split('#');
                try
                {
                    // Kasta undantag vid eventuell formateringsfel.
                    int serialNumber = int.Parse(columns[0], culture);
                    string name = columns[1];
                    decimal price = decimal.Parse(columns[2], culture);
                    string category = columns[3];
                    string description = columns[4];
                    // Ingen ny produkt läggs till om ett undantag kastas ovan.
                    var p = Product.Create(serialNumber, name, price, category, description);
                    products.Add(new ProductLoadInfo(p, $"Rad #{iRow + 1} i \"{Path}\""));
                }
                catch (FormatException)
                {
                    // Meddelande: "Textrad lästes inte in korrekt."
                    string errorMessage = $"Rad #{iRow + 1} kunde inte tolkas. Var god kontrollera formatteringen.";
                    Console.Error.WriteLine(errorMessage);
                }
                //catch (Product.DuplicateException e)
                //{
                //    string errorMessage = $"Produkten på rad #{iRow + 1} kunde inte registreras i systemet:\n{e}";
                //    Console.Error.WriteLine(errorMessage);
                //}
                catch (Exception e)
                {
                    // Om undantag kastas av oförväntad anledning.
                    // Meddelande: "Kunde inte läsa in textrad."
                    string errorMessage = $"Oväntat fel på rad #{iRow + 1}. Fördjupad felinformation:\n{e}";
                    Console.Error.WriteLine(errorMessage);
                }
            }
            return products;
        }
    }
}
