using System;
using System.Collections.Generic;
using System.IO;

namespace WFShop
{
    static class DiscountLoader
    {
        public static bool HasDiscountsLoaded { get; private set; }

        public static void Load(string path)
        {
            if (HasDiscountsLoaded) // TODO: implementera ReloadDiscounts?
                throw new InvalidOperationException("Discounts have already been loaded.");

            foreach(var dict in ParseKeyGroups(path))
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

        private static IEnumerable<Dictionary<string, string>> ParseKeyGroups(string filePath)
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
