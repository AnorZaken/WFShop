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
        private FlowLayoutPanel list;

        public MyForm()
        {
            Text = "<App Name>";
            Size = new Size(1000, 500);

            Initialise();
        }

        // Rita upp GUI:t.
        private void Initialise()
        {
            ShoppingCart entries = new ShoppingCart();
            List<Product> products = new List<Product>();
            try
            {
                products = FileHandler.ReadProductsFromFile("productSortiment.csv");
            }
            catch (FileNotFoundException e)
            {
                throw e;
            }

            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(splitContainer);

            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill
            };
            splitContainer.Panel1.Controls.Add(flowLayoutPanel);

            list = new FlowLayoutPanel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill
            };
            splitContainer.Panel2.Controls.Add(list);
            ProductBox productBox = new ProductBox(new Product(8973, "Skräddarsydd ProductBox", 12, "Övrigt",
                "ProductBox kan anpassas till valfri storlek, typsnitt och accentfärg. Om inget av dessa anges sätts de till defaultvärdet: \n\nStorlek: 250x300 \nTypsnitt: Arial \nAccentfärg: Orange"),
                300, 350)
            { AccentColor = Color.DarkGreen, ControlFont = "Calibri" };
            flowLayoutPanel.Controls.Add(productBox);
            productBox.AddToCartButton.Click += OnAddToCartButtonClick;

        }

        // Töm och rita om GUI:t.
        private void Reinitialise()
        {
            Controls.Clear();
            Initialise();
        }

        private void OnAddToCartButtonClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Product p = (Product)button.Tag;
            Console.WriteLine($"Added {p.Name} to cart.");
            CartItemBox cartItemBox = new CartItemBox(p);
            list.Controls.Add(cartItemBox);
        }
    }
}
