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
            Initialise();
        }

        // Rita upp GUI:t.
        private void Initialise()
        {
            ShoppingCart entries = new ShoppingCart();
            List<Product> products = new List<Product>();
            try
            {
                products = FileHandler.ReadProductsFromFile("productSortiment.csv", 5);
            }
            catch (FileNotFoundException e)
            {
                throw e;
            }

            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(flowLayoutPanel);
            foreach (Product product in products)
            {
                flowLayoutPanel.Controls.Add(new ProductBox(product));
            }
            ProductBox productBox = new ProductBox(new Product(8973, "Skräddarsydd ProductBox", 0.00m, "Övrigt", 
                "ProductBox kan anpassas till valfri storlek, typsnitt och accentfärg. Om inget av dessa anges sätts de till defaultvärdet: \n\nStorlek: 250x300 \nTypsnitt: Arial \nAccentfärg: Orange"),
                300, 350)
            { AccentColor = Color.DarkGreen, ControlFont = "Calibri" };
            flowLayoutPanel.Controls.Add(productBox);

        }

        // Töm och rita om GUI:t.
        private void Reinitialise()
        {
            Controls.Clear();
            Initialise();
        }
    }
}
