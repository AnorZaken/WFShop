using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Numerics;
using System.IO;

namespace WFShop
{
    class MyForm : Form
    {
        public MyForm()
        {
            ShoppingCart entries = new ShoppingCart();
            List<Product> products = new List<Product>();
            try
            {
                products = FileHandler.ReadProductsFromFile("productSortiment.csv", 4);
            }
            catch (FileNotFoundException e)
            {
                throw e;
            }
            Controls.Add(new ProductBox(new Product(1234, "Äpple", 299, "Beskrivning av ett äpple.")));
        }
    }
}
