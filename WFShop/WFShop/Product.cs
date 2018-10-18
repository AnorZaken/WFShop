using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFShop
{
    class Product
    {
        public int SerialNumber { get; }
        public string Name { get; }
        public decimal Price { get; }
        public string Description { get; }

        public Product(int serialNumber, string name, decimal price, string desc)
        {
            SerialNumber = serialNumber;
            Name = name;
            Price = price;
            Description = desc;
        }

        // TODO ...
    }
}
