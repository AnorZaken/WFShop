﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Globalization;

namespace WFShop
{
    abstract class FileHandler
    {
        public static string PathToProducts { get; set; }
        public static string PathToCart { get; set; }
        public static string PathToReceipt { get; set; }
        public static bool HasProductsLoaded { get; private set; }

        public static void LoadProducts()
        {
            if (HasProductsLoaded) // TODO: implementera ReloadProducts?
                throw new InvalidOperationException("Products have already been loaded.");

            string[] lines;
            try
            {
                lines = File.ReadAllLines(PathToProducts);
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"Kunde inte hitta produktfilen: \"{PathToProducts}\"");
            }
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
                    Product.RegisterNew(serialNumber, name, price, category, description);
                }
                catch (FormatException)
                {
                    // Meddelande: "Textrad lästes inte in korrekt."
                    string errorMessage = $"Rad #{iRow + 1} kunde inte tolkas. Var god kontrollera formatteringen.";
                    Console.Error.WriteLine(errorMessage);
                }
                catch (Product.DuplicateException e)
                {
                    string errorMessage = $"Produkten på rad #{iRow + 1} kunde inte registreras i systemet:\n{e}";
                    Console.Error.WriteLine(errorMessage);
                }
                catch (Exception e)
                {
                    // Om undantag kastas av oförväntad anledning.
                    // Meddelande: "Kunde inte läsa in textrad."
                    string errorMessage = $"Oväntat fel på rad #{iRow + 1}. Fördjupad felinformation:\n{e}";
                    Console.Error.WriteLine(errorMessage);
                }
            }
            HasProductsLoaded = true;
        }

        //private static IEnumerable<string> CreateReceipt(ShoppingCart cart)
        //{
        //    var lines = new List<string>(2 + 3 * cart.UniqueProductCount + 1);
        //    lines.Add("Kvitto");
        //    lines.Add("");
        //    //List<string> lines = new List<string> { "Kvitto", "" };
        //    foreach (ProductEntry productEntry in cart)
        //    {
        //        lines.Add($"{productEntry.Product.SerialNumber}: {productEntry.Amount} x {productEntry.Product.Name} à {productEntry.Product.Price} kr");
        //        lines.Add($"{productEntry.Product.Price * productEntry.Amount} kr");
        //        lines.Add("");
        //    }
        //    lines.Add($"Totalt: {cart.FinalPrice} kr");
        //    return lines;
        //}
        private static IEnumerable<string> CreateReceipt(ShoppingCart cart)
        {
            yield return "Kvitto";
            yield return "";
            foreach (ProductEntry productEntry in cart)
            {
                yield return $"{productEntry.Product.SerialNumber}: {productEntry.Amount} x {productEntry.Product.Name} à {productEntry.Product.Price} kr";
                yield return $"{productEntry.Product.Price * productEntry.Amount} kr";
                yield return "";
            }
            yield return $"Totalt: {cart.FinalPrice} kr";
        }

        // Metoder som kan användas till att skapa ett kvitto och spara som en fil på datorn.
        public static void SaveReceipt(ShoppingCart cart)
            => File.WriteAllLines(PathToReceipt, CreateReceipt(cart));

        //public static void CreateReceipt(ShoppingCart cart, string path) => throw new NotImplementedException();

        // Metoder som kan användas till att spara varukorgen som en fil på datorn.
        public static void SaveShoppingCart(ShoppingCart cart)
            => File.WriteAllLines(PathToCart, cart.Select(pe => $"{pe.Product.SerialNumber}#{pe.Amount}"));

        //public static void SaveShoppingCart(ShoppingCart cart, string path) => throw new NotImplementedException();

        // Kan användas till att läsa in den sparade varukorgen när en ny instans av programmet skapas eller på användarens begäran.
        public static ShoppingCart LoadShoppingCart()
        {
            ShoppingCart cart = new ShoppingCart();
            foreach (ProductEntry productEntry in LoadCartContents())
                try
                {
                    cart.Add(productEntry);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
            return cart;
        }

        private static List<ProductEntry> LoadCartContents()
        {
            string[] lines;
            try
            {
                lines = File.ReadAllLines(PathToCart);
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"Kunde inte hitta varukorgen: \"{PathToCart}\"");
            }
            var culture = CultureInfo.InvariantCulture;
            List<ProductEntry> productEntries = new List<ProductEntry>();
            foreach (string line in lines)
            {
                string[] columns = line.Split('#');
                try
                {
                    int serialNumber = int.Parse(columns[0], culture);
                    int amount = int.Parse(columns[1], culture);
                    // GetProduct kan kasta KeyNotFoundException om serienummret inte matchar någon produkt.
                    productEntries.Add(new ProductEntry(GetProduct(serialNumber), amount));
                }
                catch (FormatException)
                {
                    string errorMessage = $"Rad \"{line}\" lästes inte in korrekt. Var god kontrollera källan.";
                    Console.Error.WriteLine(errorMessage);
                }
                catch (KeyNotFoundException)
                {
                    string errorMessage = $"Serienummret '{columns[0]}' refererar inte till någon produkt.";
                    Console.Error.WriteLine(errorMessage);
                }
                catch (Exception e)
                {
                    string errorMessage = $"Oväntat fel på rad \"{line}\". Fördjupad felinformation:\n{e}";
                    Console.Error.WriteLine(errorMessage);
                }
            }
            return productEntries;
        }

        // Omvandlar int till Product.
        private static Product GetProduct(int serialNumber)
        {
            if (!HasProductsLoaded)
                LoadProducts();
            return Product.TryGet(serialNumber, out Product p) ? p
                : throw new KeyNotFoundException($"No such product (SN:{serialNumber})");
        }
    }
}
