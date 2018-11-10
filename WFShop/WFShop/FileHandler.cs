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
    abstract class FileHandler // TODO: denna klassen har för mycket coupling...
    {
        public static string PathToProducts { get; set; }
        public static string PathToCart { get; set; }
        public static string PathToReceipt { get; set; }
        public static string PathToDiscounts { get; set; }
        public static bool HasProductsLoaded { get; private set; }
        public static bool HasDiscountsLoaded { get; private set; }

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

        public static void LoadDiscounts()
        {
            if (HasDiscountsLoaded) // TODO: implementera ReloadDiscounts?
                throw new InvalidOperationException("Discounts have already been loaded.");

            foreach(var dict in ParseKeyGroups(PathToDiscounts))
            {
                if (!Discount.TryParse(dict, out _))
                {
                    Console.Error.WriteLine("No registered discount parser was able to parse discount data:");
                    foreach (var kvp in dict)
                    {
                        Console.Error.Write(kvp.Key + ": ");
                        Console.Error.WriteLine(kvp.Value);
                    }
                }
            }
            HasDiscountsLoaded = true;
        }
        
        private static IEnumerable<string> CreateReceipt(ShoppingCart cart, string currency = "kr")
        {
            yield return "Kvitto";
            yield return "";
            decimal sumDiscount = 0;
            foreach (ProductAmount pe in cart)
            {
                yield return $"({pe.SerialNumber})";
                yield return $"{pe.Amount} x {pe.Product.Name} à {pe.Product.Price} {currency}";
                if (cart.TryGetRebate(pe.SerialNumber, out DiscountAmount de))
                {
                    sumDiscount += de.Amount;
                    yield return $"\tRabatt: -{de.Amount} {currency} ({de.Discount})";
                    yield return $"\t{pe.Product.Price * pe.Amount - de.Amount} {currency}";
                }
                else
                {
                    yield return $"\t{pe.Product.Price * pe.Amount} {currency}";
                }
                yield return "";
            }
            var coupons = cart.AppliedCoupons;
            foreach (var de in coupons)
            {
                sumDiscount += de.Amount;
                yield return $"Rabattkod: \"{de.Discount.CouponCode}\" ({de.Discount})";
                yield return $"\t-{de.Amount} {currency}";
                yield return "";
            }
            if (sumDiscount != 0)
                yield return $"Total rabatt: -{sumDiscount} {currency}";
            decimal fp = cart.FinalPrice;
            decimal fpRounded = Math.Round(fp);
            if (fp != fpRounded)
                yield return $"Öresavrundning: {(fpRounded - fp):+0.00;-0.00} {currency}";
            yield return $"Att betala: {fpRounded:0.00} {currency}";
        }

        // Metoder som kan användas till att skapa ett kvitto och spara som en fil på datorn.
        public static void SaveReceipt(ShoppingCart cart)
            => File.WriteAllLines(PathToReceipt, CreateReceipt(cart));

        // Metoder som kan användas till att spara varukorgen som en fil på datorn.
        public static void SaveShoppingCart(ShoppingCart cart)
            => File.WriteAllLines(PathToCart, cart.Select(pe => $"{pe.Product.SerialNumber}#{pe.Amount}"));

        // Kan användas till att läsa in den sparade varukorgen när en ny instans av programmet skapas eller på användarens begäran.
        public static bool TryLoadShoppingCart(out ShoppingCart cart, out int errorCount)
        {
            if (TryLoadCartContents(out IReadOnlyList<ProductAmount> contents, out errorCount))
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

        private static bool TryLoadCartContents(out IReadOnlyList<ProductAmount> productEntries, out int errorCount)
        {
            string[] lines;
            try
            {
                lines = File.ReadAllLines(PathToCart);
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
                    pe.Add(new ProductAmount(GetProduct(serialNumber), amount));
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

        // Omvandlar int till Product.
        private static Product GetProduct(int serialNumber)
        {
            if (!HasProductsLoaded)
                LoadProducts();
            return Product.TryGet(serialNumber, out Product p) ? p
                : throw new KeyNotFoundException($"No such product (SN:{serialNumber})");
        }

        public static IEnumerable<Dictionary<string, string>> ParseKeyGroups(string filePath)
        {
            const char SPLIT_CHAR = ':';
            const char QUOTE_CHAR = '"';
            Dictionary<string, string> keyGroup = null;
            int rowNum = 0;
            string multiRowKey = null;
            string multiRowValue = null;
            foreach (var line in File.ReadLines(filePath))
            {
                ++rowNum;
                if (multiRowKey != null) // - Row is a value continuation: no key expected!
                {
                    throw new NotImplementedException("Multi-row values not supported yet!");
                }
                else if (string.IsNullOrWhiteSpace(line)) // - Row is empty.
                {
                    if (keyGroup != null) // - Previous row was not empty, yield data!
                    {
                        yield return keyGroup;
                        keyGroup = null;
                    }
                }
                else
                {
                    if (keyGroup == null)
                        keyGroup = new Dictionary<string, string>();
                    int iSplit = line.IndexOf(SPLIT_CHAR);
                    if (iSplit < 0)
                        throw new FormatException($"Rad #{rowNum} är inte i key-value format.");
                    var key = line.Substring(0, iSplit);
                    int iQuote = line.IndexOf(QUOTE_CHAR, iSplit + 1);
                    if (iQuote < 0 || !line.RangeIsWhiteSpace(iSplit, iQuote, Range.Option.Exclusive_Exclusive))
                    {
                        keyGroup.Add(key, line.Substring(iSplit + 1).Trim());
                    }
                    else // - Row begins with a QUOTE: *possible* multi-row value.
                    {
                        // Move index of the first QUOTE to iSplit.
                        iSplit = iQuote;
                        // Put index of the last QUOTE into iQuote.
                        iQuote = line.LastIndexOf(QUOTE_CHAR);
                        if (iQuote == iSplit)
                        {
                            // - First and Last QUOTE are the same: "open" row ending.
                            multiRowKey = key;
                            multiRowValue = line.Substring(iQuote + 1);
                        }
                        else if (line.RangeIsWhiteSpace(iQuote, line.Length, Range.Option.Exclusive_Exclusive))
                        {
                            // - Value begins and ends with QUOTE (sans leading / trailing whitespace).
                            Range.Normalize(ref iSplit, ref iQuote, Range.Option.Exclusive_Exclusive);
                            keyGroup.Add(key, line.Substring(iSplit, iQuote));
                        }
                        else
                        {
                            throw new FormatException($"Rad #{rowNum} innehåller en oväntade konfiguration av citationstecken.");
                        }
                    }
                }
            }
            if (multiRowKey != null) // - File ended with "open" QUOTE value.
                throw new FormatException("Unexpected end-of-file: Value closing quotes expected.");
            if (keyGroup != null) // - Previous row was not empty, yield data!
                yield return keyGroup;
        }
    }
}
