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
        public static List<Product> ReadProductsFromFile(string path, int textFileParameterCount)
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
                    // Kasta undantag ifall parameterantalet i textfilen understigs eller överstigs.
                    if (commaSeparatedValues.Length < textFileParameterCount || commaSeparatedValues.Length > textFileParameterCount)
                        throw new ArgumentOutOfRangeException(nameof(commaSeparatedValues), "Olagligt antal parametrar lästes in från textfil.");
                    // Kasta undantag vid eventuell formateringsfel.
                    int serialNumber = int.Parse(commaSeparatedValues[0]);
                    string name = commaSeparatedValues[1];
                    decimal price = decimal.Parse(commaSeparatedValues[2]);
                    string description = commaSeparatedValues[3];
                    // Ingen ny produkt läggs till om ett fel kastas.
                    products.Add(new Product(serialNumber, name, price, description));
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

        private static Image ReadImageFromFile() => throw new NotImplementedException();
       
    }
}
