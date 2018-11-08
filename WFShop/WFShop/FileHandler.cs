﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

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
            for (int iRow = 0; iRow < lines.Length; ++iRow)
            {
                string[] columns = lines[iRow].Split('#');
                try
                {
                    // Kasta undantag vid eventuell formateringsfel.
                    int serialNumber = int.Parse(columns[0]);
                    string name = columns[1];
                    decimal price = decimal.Parse(columns[2]);
                    string category = columns[3];
                    string description = columns[4];
                    // Ingen ny produkt läggs till om ett undantag kastas ovan.
                    Product.RegisterNew(serialNumber, name, price, category, description);
                }
                catch (FormatException)
                {
                    // Meddelande: "Textrad lästes inte in korrekt."
                    string errorMessage =
                        $"Rad #{iRow + 1} lästes inte in korrekt. Var god kontrollera källan.";
                    Console.Error.WriteLine(errorMessage);
                }
                catch (Product.DuplicateException e)
                {
                    string errorMessage =
                        $"Produkten på rad #{iRow + 1} kunde inte registreras i systemet:\n{e}";
                    Console.Error.WriteLine(errorMessage);
                }
                catch (Exception e)
                {
                    // Om undantag kastas av oförväntad anledning.
                    // Meddelande: "Kunde inte läsa in textrad."
                    string errorMessage =
                        $"Fångade oväntat fel på rad #{iRow + 1}. Fördjupad felinformation:\n{e}";
                    Console.Error.WriteLine(errorMessage);
                }
            }
            HasProductsLoaded = true;
        }

        // Metoder som kan användas till att skapa ett kvitto och spara som en fil på datorn.
        public static void CreateReceipt(ShoppingCart cart)
        {
            List<string> lines = new List<string> { "Kvitto\n" };
            foreach (ProductEntry productEntry in cart)
            {
                lines.Add($"{productEntry.Product.SerialNumber}: {productEntry.Amount} x {productEntry.Product.Name} à {productEntry.Product.Price} kr\n{productEntry.Product.Price * productEntry.Amount} kr\n");
            }
            lines.Add($"Totalt: {cart.FinalPrice} kr");
            File.WriteAllLines(PathToReceipt, lines);
        }

        public static void CreateReceipt(ShoppingCart cart, string path) => throw new NotImplementedException();

        // Metoder som kan användas till att spara varukorgen som en fil på datorn.
        public static void SaveShoppingCart(ShoppingCart cart)
        {
            // Felhantereing kanske inte behövs?
            if (!File.Exists(PathToCart))
                throw new FileNotFoundException($"Kunde inte hitta fil: {PathToCart}");
            List<string> lines = new List<string>();
            foreach (ProductEntry productEntry in cart)
                lines.Add($"{productEntry.Product.SerialNumber}#{productEntry.Amount}");
            File.WriteAllLines(PathToCart, lines);            
        }

        public static void SaveShoppingCart(ShoppingCart cart, string path) => throw new NotImplementedException();

        // Kan användas till att läsa in den sparade varukorgen när en ny instans av programmet skapas eller på användarens begäran.
        public static ShoppingCart GetShoppingCart()
        {
            ShoppingCart cart = new ShoppingCart();
            foreach (ProductEntry productEntry in GetProductEntries())
                try
                {
                    cart.Add(productEntry.Product, productEntry.Amount);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            return cart;
        }

        private static List<ProductEntry> GetProductEntries()
        {
            string[] lines = Array.Empty<string>();
            if (!File.Exists(PathToCart))
                throw new FileNotFoundException($"Kunde inte hitta varukorgen:\n{PathToCart}");
            else
                lines = File.ReadAllLines(PathToCart);

            List<ProductEntry> productEntries = new List<ProductEntry>();
            foreach (string line in lines)
            {
                string[] separatedValues = line.Split('#');
                try
                {
                    int serialNumber = int.Parse(separatedValues[0]);
                    int amount = int.Parse(separatedValues[1]);
                    // GetProduct kan kasta ArgumentNullException om serienummret inte matchar någon produkt.
                    productEntries.Add(new ProductEntry(GetProduct(serialNumber), amount));
                }
                catch (FormatException)
                {
                    string errorMessage = $"Rad \"{line}\" lästes inte in korrekt. Var god kontrollera källan.";
                    Console.WriteLine(errorMessage);
                }
                catch (ArgumentNullException)
                {
                    string errorMessage = $"Serienummret '{separatedValues[0]}' refererar inte till någon produkt.";
                    Console.WriteLine(errorMessage);
                }
                catch (Exception e)
                {
                    string errorMessage = $"Fångade oväntat fel på rad \"{line}\". Fördjupad felinformation:\n{e}";
                    Console.WriteLine(errorMessage);
                }
            }

            return productEntries;
        }

        // Omvandlar int till Product.
        private static Product GetProduct(int serialNumber)
        {
            if (!HasProductsLoaded)
                LoadProducts();
            return Product.TryGet(serialNumber, out Product p) ? p : null;
        }
    }
}
