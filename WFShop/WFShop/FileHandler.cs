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
        public static List<Product> ReadProductsFromFile()
        {
                List<Product> products = new List<Product>();
                // Implementera felhantering.
                string[] lines = File.ReadAllLines("productSortiment.csv");
                foreach (var line in lines)
                {
                    string[] v = line.Split('#');
                    // Implementera felhantering: 'v' kan ha fler eller färre element.
                    int serialNumber;
                    string name = v[1];
                    decimal price;
                    string description = v[3];
                    try
                    {
                        TryParse(v[0], out serialNumber);
                        TryParse(v[2], out price);

                        // Produkten läggs inte till om ett fel kastas.
                        products.Add(new Product(serialNumber, name, price, description));
                    }
                    catch (FormatException e)
                    {
                        // ...
                    }
                    catch (Exception e)
                    {
                        // ...
                    }
                }

                return products;
        }

        private static Image ReadImageFromFile()
        {
            throw new NotImplementedException();
        }

        private static void TryParse(string str, out int value)
        {
            bool isExceptionSafe = false;
            isExceptionSafe = int.TryParse(str, out value);
            if (!isExceptionSafe)
                throw new FormatException($"Värdet i '{nameof(str)}' hade ett felaktigt format.");
        }

        private static void TryParse(string str, out decimal value)
        {
            bool isExceptionSafe = false;
            isExceptionSafe = decimal.TryParse(str, out value);
            if (!isExceptionSafe)
                throw new FormatException($"Värdet i '{nameof(str)}' hade ett felaktigt format.");
        }

       
    }
}
