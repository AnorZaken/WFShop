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
        private SplitContainer splitContainer;
        private FlowLayoutPanel flowLayoutPanel;
        private FlowLayoutPanel listFlow;

        private SplitterPanel panel1;
        private SplitterPanel panel2;

        private ShoppingCart cart;

        public MyForm()
        {
            Text = "<App Name>";
            Size = new Size(1000, 500);

            Initialise();
        }

        // Rita upp GUI:t.
        private void Initialise()
        {
            List<Product> products = FileHandler.ReadProductsFromFile("productSortiment.csv");
            cart = new ShoppingCart();

            splitContainer = new SplitContainer { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle };
            Controls.Add(splitContainer);

            panel1 = splitContainer.Panel1;
            panel2 = splitContainer.Panel2;

            flowLayoutPanel = new FlowLayoutPanel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill
            };
            panel1.Controls.Add(flowLayoutPanel);

            listFlow = new FlowLayoutPanel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill
            };
            panel2.Controls.Add(listFlow);

            ProductBox customProductBox = new ProductBox(new Product(1234, "Skräddarsydd ProductBox", 0, "Övrigt", "Exempel på en skräddarsydd ProductBox."), () => { })
            {
                AccentColor = Color.DarkGreen,
                ControlFont = "Calibri"
            };
            flowLayoutPanel.Controls.Add(customProductBox);

            foreach (var product in products)
            {
                // Metod att skicka till ProductBox.
                Action onAddToCartButtonClicked = () =>
                {
                    cart.Add(product);
                    CreateCartList();
                };

                ProductBox productBox = new ProductBox(product, onAddToCartButtonClicked);
                flowLayoutPanel.Controls.Add(productBox);
            }
        }

        // Töm och rita om GUI:t.
        private void Reinitialise()
        {
            Controls.Clear();
            Initialise();
        }

        // Uppdaterar även listFlow.
        private void CreateCartList()
        {
            // Ta bort eventuella kontroller som finns kvar i listFlow.
            listFlow.Controls.Clear();
            foreach (var entry in cart)
            {
                // Metod att skicka till CartItemBox.
                Action onRemoveButtonClicked = () =>
                {
                    cart.Remove(entry.Product, 1);
                    CreateCartList();
                };

                CartItemBox cartItemBox = new CartItemBox(entry, onRemoveButtonClicked);
                listFlow.Controls.Add(cartItemBox);

                Console.WriteLine(cartItemBox);
                
            }
        }
    }
}
