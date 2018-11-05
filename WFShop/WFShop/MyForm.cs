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
        // Control fields
        private SplitContainer splitContainer;
        private FlowLayoutPanel flowProductBoxView;
        private FlowLayoutPanel cartItemBoxView;
        private TextBox couponCodeTextBox;
        private Label totalCostLabel;

        // Fields
        private List<Product> products;

        // Control properties

        // Properties
        private ShoppingCart cart;

        public MyForm()
        {
            FileHandler.PathToProducts = "productSortiment.csv";
            FileHandler.PathToCart = "cart.csv"; // TODO: Fixa så att man slipper mata in hela sökvägen.
            try
            {
                products = FileHandler.GetProducts();
                cart = FileHandler.GetShoppingCart();
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message);
                Environment.Exit(0);
            }

            Text = "<App Name>";
            Size = new Size(1000, 500);

            CreateGUI();
        }

        // Rita upp GUI:t.
        private void CreateGUI()
        {
            splitContainer = new SplitContainer
            {
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill
            };
            Controls.Add(splitContainer);
            // En tredjedel av fönstret.
            splitContainer.SplitterDistance = (int)(Width / 1.5);

            CreatePanel1Controls();
            CreatePanel2Controls();

            CreateProductBoxes(products);
        }

        // Töm och rita om GUI:t.
        private void Reinitialise()
        {
            Controls.Clear();
            CreateGUI();
        }

        private void CreatePanel1Controls()
        {
            TableLayoutPanel mainTable = new TableLayoutPanel
            {
                RowCount = 2,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Dock = DockStyle.Fill
            };
            splitContainer.Panel1.Controls.Add(mainTable);
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            Panel sortPanel = new Panel
            {
                //BackColor = Color.FromArgb(128, 128, 72),
                Margin = new Padding(0),
                Dock = DockStyle.Fill
            };
            mainTable.Controls.Add(sortPanel);

            TableLayoutPanel sortTable = new TableLayoutPanel { ColumnCount = 4, Dock = DockStyle.Fill };
            sortTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            sortTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            sortTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            sortTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            sortPanel.Controls.Add(sortTable);
            sortTable.Controls.Add(new Label
            {
                Text = "Sortera efter: ",
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            });
            RadioButton sortByNameRadioButton = new RadioButton { Text = "Namn", Font = new Font("Arial", 12), CheckAlign = ContentAlignment.MiddleLeft };
            sortTable.Controls.Add(sortByNameRadioButton);
            RadioButton sortByCost = new RadioButton { Text = "Pris", Font = new Font("Arial", 12), CheckAlign = ContentAlignment.MiddleLeft };
            sortTable.Controls.Add(sortByCost);
            RadioButton sortByRelevance = new RadioButton { Text = "Relevans", Font = new Font("Arial", 12), CheckAlign = ContentAlignment.MiddleLeft };
            sortTable.Controls.Add(sortByRelevance);

            flowProductBoxView = new FlowLayoutPanel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill
            };
            mainTable.Controls.Add(flowProductBoxView);
        }

        private void CreatePanel2Controls()
        {
            TableLayoutPanel mainTable = new TableLayoutPanel
            {
                RowCount = 2,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Dock = DockStyle.Fill
            };
            splitContainer.Panel2.Controls.Add(mainTable);
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            TableLayoutPanel cartViewTable = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 3,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Dock = DockStyle.Fill
            };
            mainTable.Controls.Add(cartViewTable);
            cartViewTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            cartViewTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            cartViewTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            cartItemBoxView = new FlowLayoutPanel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill
            };
            cartViewTable.Controls.Add(cartItemBoxView);
            cartViewTable.SetColumnSpan(cartItemBoxView, 2);

            cartViewTable.Controls.Add(new Label
            {
                Text = "Rabattkod: ",
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            });

            couponCodeTextBox = new TextBox
            {
                Font = new Font("Arial", 12),
                Dock = DockStyle.Fill
            };
            cartViewTable.Controls.Add(couponCodeTextBox);

            cartViewTable.Controls.Add(new Label
            {
                Text = $"Totalt: ",
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            });

            totalCostLabel = new Label
            {
                Text = $"{cart.FinalPrice} kr",
                Font = new Font("Arial", 12),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };
            cartViewTable.Controls.Add(totalCostLabel);

            TableLayoutPanel buttonTable = new TableLayoutPanel
            {
                ColumnCount = 2,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Dock = DockStyle.Fill
            };
            mainTable.Controls.Add(buttonTable);
            buttonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            buttonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));

            Button saveCartButton = new Button
            {
                Text = "Spara",
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.Orange,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0),
                Dock = DockStyle.Fill
            };
            buttonTable.Controls.Add(saveCartButton);
            saveCartButton.Click += (s, e) => FileHandler.SaveShoppingCart(cart);

            Button checkOutButton = new Button
            {
                Text = "Slutför",
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0),
                Dock = DockStyle.Fill
            };
            buttonTable.Controls.Add(checkOutButton);

            // Ladda om cartItemBoxView så att eventuella produkter i textfilen visas.
            RefreshCartItemBoxView();
        }

        private void CreateProductBoxes(List<Product> products)
        {
            //Skapa ProductBox för varje produkt som finns i textfilen.
            foreach (Product product in products)
            {
                ProductBox p = new ProductBox(product);
                flowProductBoxView.Controls.Add(p);
                p.AddToCartButton.Click += OnAddToCartButtonClick;
            }
        }


        private void RefreshCartItemBoxView()
        {
            cartItemBoxView.Controls.Clear();
            foreach (ProductEntry productEntry in cart)
            {
                CartItemBox c = new CartItemBox(productEntry);
                cartItemBoxView.Controls.Add(c);
                c.QuantityAddButton.Click += OnAnyButtonClickInCartItemBox;
                c.QuantitySubtractButton.Click += OnAnyButtonClickInCartItemBox;
                c.RemoveButton.Click += OnAnyButtonClickInCartItemBox;
            }

        }

        // ProductBox
        private void OnAddToCartButtonClick(object sender, EventArgs e)
        {
            Product product = (Product)(sender as Button).Tag;
            cart.Add(product);
            totalCostLabel.Text = $"{cart.FinalPrice} kr";
            RefreshCartItemBoxView();
        }

        // CartItemBox
        private void OnAnyButtonClickInCartItemBox(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            ProductEntry productEntry = (ProductEntry)button.Tag;

            if (button.Name == "QuantityAddButton")
            {
                cart.Add(productEntry.Product);
                totalCostLabel.Text = $"{cart.FinalPrice} kr";
            }
            else if (button.Name == "QuantitySubtractButton")
            {
                cart.Remove(productEntry.Product, 1);
                totalCostLabel.Text = $"{cart.FinalPrice} kr";
            }
            else if (button.Name == "RemoveButton")
            {
                cart.RemoveAll(productEntry.Product);
                totalCostLabel.Text = $"{cart.FinalPrice} kr";
                RefreshCartItemBoxView();
            }
        }
    }
}
