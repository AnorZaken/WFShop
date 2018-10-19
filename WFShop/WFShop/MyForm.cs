using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Numerics;

namespace WFShop
{
    class MyForm : Form
    {
        public MyForm()
        {
            ShoppingCart entries = new ShoppingCart();
            List<Product> products = FileHandler.ReadProductsFromFile();
        }
    }
}
