using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace WFShop
{
    // Klass som användas till att läsa / spara varukorgen som en fil på datorn.
    class ShoppingCartFileStorage : IShoppingCartStorage
    {
        public ShoppingCartFileStorage(string path, IProductProvider productProvider)
        {
            Path = path;
            ProductProvider = productProvider;
        }

        public string Path { get; }

        public IProductProvider ProductProvider { get; }

        public bool HasSave => File.Exists(Path);

        public void Save(ShoppingCart cart)
            => File.WriteAllLines(Path, cart.Products.Select(pe => $"{pe.Product.SerialNumber}#{pe.Amount}"));

        // Kan användas till att läsa in den sparade varukorgen när en ny instans av programmet skapas eller på användarens begäran.
        public bool TryLoad(out ShoppingCart cart, out int errorCount)
        {
            if (TryLoad(out IReadOnlyList<ProductAmount> contents, out errorCount))
            {
                cart = new ShoppingCart();
                foreach (ProductAmount productEntry in contents)
                    try
                    {
                        cart.Add(productEntry);
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e.Message);
                        ++errorCount;
                    }
                if (errorCount == 0 || cart.UniqueProductCount != 0)
                    return true;
            }
            cart = null;
            return false;
        }

        protected bool TryLoad(out IReadOnlyList<ProductAmount> productEntries, out int errorCount)
        {
            string[] lines;
            try
            {
                lines = File.ReadAllLines(Path);
            }
            catch (FileNotFoundException)
            {
                productEntries = Array.Empty<ProductAmount>();
                errorCount = 0; // File does not exist isn't an error.
                return false;
            }
            var culture = CultureInfo.InvariantCulture;
            var pe = new List<ProductAmount>(lines.Length);
            errorCount = 0;
            foreach (string line in lines)
            {
                string[] columns = line.Split('#');
                try
                {
                    int serialNumber = int.Parse(columns[0], culture);
                    int amount = int.Parse(columns[1], culture);
                    // GetProduct kan kasta KeyNotFoundException om serienummret inte matchar någon produkt.
                    pe.Add(new ProductAmount(ProductProvider.Get(serialNumber), amount));
                }
                catch (FormatException)
                {
                    string errorMessage = $"Rad \"{line}\" lästes inte in korrekt. Var god kontrollera källan.";
                    Console.Error.WriteLine(errorMessage);
                    ++errorCount;
                }
                catch (KeyNotFoundException)
                {
                    string errorMessage = $"Serienummret '{columns[0]}' refererar inte till någon produkt.";
                    Console.Error.WriteLine(errorMessage);
                    ++errorCount;
                }
                catch (Exception e)
                {
                    string errorMessage = $"Oväntat fel på rad \"{line}\". Fördjupad felinformation:\n{e}";
                    Console.Error.WriteLine(errorMessage);
                    ++errorCount;
                }
            }
            productEntries = pe;
            return errorCount == 0 || pe.Count != 0;
        }
    }
}
