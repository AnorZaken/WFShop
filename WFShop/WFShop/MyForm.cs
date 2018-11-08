﻿using System;
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
        private ComboBox filterComboBox;
        private RadioButton sortByNameRadioButton;
        private RadioButton sortByPriceRadioButton;

        // Fields
        private List<Product> products;

        // Control properties

        // Properties
        private ShoppingCart cart;

        public MyForm()
        {
            FileHandler.PathToProducts = "productSortiment.csv";
            FileHandler.PathToCart = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\Temp\cart.csv";
            FileHandler.PathToReceipt = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\receipt.txt";
            ImageHandler.PathToFolder = Environment.CurrentDirectory + @"\Images";

            if (!File.Exists(FileHandler.PathToCart))
            {
                using (StreamWriter streamWriter = new StreamWriter(FileHandler.PathToCart))
                {
                    streamWriter.Write(string.Empty);
                    streamWriter.Close(); // <- Lyckas inte stänga med File-klassen.
                }
                MessageBox.Show($"Skapade filen {FileHandler.PathToCart}.");
                
            }
            cart = new ShoppingCart();
            try
            {
                products = FileHandler.GetProducts();
                // TODO: Logiskt fel uppstår på raden nedan. Programmet hittar och läser filer, men produkterna som returneras är null.
                cart = FileHandler.GetShoppingCart();
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message);
                Environment.Exit(0);
            }

            Text = "<App Name>";
            Size = new Size(1000, 500);

            Initialize();
        }

        // Rita upp GUI:t.
        private void Initialize()
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

        // Förstör och återskapa inre kontroller.
        private void ReInitialize()
        {
            Controls.Clear();
            Initialize();
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

            Panel filterPanel = new Panel
            {
                Margin = new Padding(0),
                Dock = DockStyle.Fill
            };
            mainTable.Controls.Add(filterPanel);

            TableLayoutPanel filterTable = new TableLayoutPanel { ColumnCount = 4, Dock = DockStyle.Fill };
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));

            filterPanel.Controls.Add(filterTable);
            filterTable.Controls.Add(new Label
            {
                Text = "Sortera efter: ",
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            });

            sortByNameRadioButton = new RadioButton { Text = "Namn", Font = new Font("Arial", 12), Name = nameof(sortByNameRadioButton) };
            filterTable.Controls.Add(sortByNameRadioButton);
            sortByNameRadioButton.CheckedChanged += (s, e) => RefreshProductBoxView();

            sortByPriceRadioButton = new RadioButton { Text = "Pris", Font = new Font("Arial", 12), Name = nameof(sortByPriceRadioButton) };
            filterTable.Controls.Add(sortByPriceRadioButton);
            sortByPriceRadioButton.CheckedChanged += (s, e) => RefreshProductBoxView();

            filterComboBox = new ComboBox { Font = new Font("Arial", 12), Sorted = true, Dock = DockStyle.Fill };
            filterTable.Controls.Add(filterComboBox);

            flowProductBoxView = new FlowLayoutPanel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill
            };
            mainTable.Controls.Add(flowProductBoxView);
            
            // Returnera en IEnumerable med unika kategorier.
            IEnumerable<string> categories = products.Select(p => p.Category.Name).Distinct();
            filterComboBox.Items.Add("Alla");
            filterComboBox.Items.AddRange(categories.ToArray());
            filterComboBox.SelectedIndexChanged += (s, e) => RefreshProductBoxView();
            // TODO: Läs in kategorier från textfil eller från List.            
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
            couponCodeTextBox.TextChanged += OnCouponCodeTextBoxChanged;

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

            Button checkoutButton = new Button
            {
                Text = "Slutför beställning",
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0),
                Dock = DockStyle.Fill
            };
            buttonTable.Controls.Add(checkoutButton);
            checkoutButton.Click += (s, e) =>
            {
                FileHandler.CreateReceipt(cart);
            };

            // Ladda om cartItemBoxView så att eventuella produkter i textfilen visas.
            RefreshCartItemBoxView();
        }

        private void CreateProductBoxes(List<Product> products, string filter = "Alla", string sortBy = null)
        {
            //Skapa ProductBox för varje produkt som finns i textfilen.
            // Rensa flowProductBoxView när metoden anropas.
            //flowProductBoxView.Controls.Clear();
            // IEnumerable som innehåller filtrerad version av product-listan.
            IEnumerable<Product> filteredProducts = 
                filter == "Alla" || filter is null 
                ? products.Select(p => p) 
                : products.Where(p => p.Category.Name == filter);

            IEnumerable<Product> orderedProducts = 
                sortBy is "Name" ? filteredProducts.OrderBy(p => p.Name) : 
                sortBy is "Price" ? filteredProducts.OrderBy(p => p.Price) : 
                filteredProducts;
           

            foreach (Product product in orderedProducts)
            {
                ProductBox p = new ProductBox(product, 200, 250);
                flowProductBoxView.Controls.Add(p);
                p.Thumbnail.Image = ImageHandler.LoadImage(product.SerialNumber) ?? ImageHandler.Default;
                p.AddToCartButton.Click += OnAnyButtonClick_ProductBox;
                p.Thumbnail.Click += OnThumbnailClick_ProductBox;
            }
        }

        private void RefreshCartItemBoxView()
        {
            cartItemBoxView.Controls.Clear();
            foreach (ProductEntry pe in cart)
            {
                CartItemBox c = new CartItemBox(pe, cartItemBoxView.Width - 6);
                cartItemBoxView.Controls.Add(c);
                c.Thumbnail.Image = ImageHandler.LoadImage(pe.SerialNumber) ?? ImageHandler.Default;
                c.QuantityAddButton.Click += OnAnyButtonClick_CartItemBox;
                c.QuantitySubtractButton.Click += OnAnyButtonClick_CartItemBox;
                c.RemoveButton.Click += OnAnyButtonClick_CartItemBox;
                c.Thumbnail.Click += OnThumbnailClick_CartItemBox;

                if (cart.TryGetRebate(pe.SerialNumber, out DiscountEntry de))
                    c.SetDiscountInfo(de);

                splitContainer.SplitterMoved += (s, e) => c.Width = cartItemBoxView.Width - 6;
            }
        }

        private void RefreshProductBoxView()
        {
            flowProductBoxView.Controls.Clear();
            CreateProductBoxes(products, filterComboBox.SelectedItem as string, sortByNameRadioButton.Checked ? "Name" : sortByPriceRadioButton.Checked ? "Price" : "");
        }

        // this
        private void OnCouponCodeTextBoxChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            // Test.
            if (textBox.Text == "abc123")
                textBox.BackColor = Color.Green;
            else
                textBox.BackColor = Color.Red;
        }

        // ProductBox
        private void OnAnyButtonClick_ProductBox(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Product product = (Product)button.Tag;

            if (button.Name == "AddToCartButton")
            {
                cart.Add(product);
                totalCostLabel.Text = $"{cart.FinalPrice} kr";
                RefreshCartItemBoxView();
            }
        }

        private void OnThumbnailClick_ProductBox(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        // CartItemBox
        private void OnAnyButtonClick_CartItemBox(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            ProductEntry pe = (ProductEntry)button.Tag;

            if (button.Name == "QuantityAddButton")
            {
                cart.Add(pe.Product);
                totalCostLabel.Text = $"{cart.FinalPrice} kr";
                var c = GetCartItemBox();
                if (cart.TryGetRebate(pe.SerialNumber, out DiscountEntry de))
                    c.SetDiscountInfo(de);
            }
            else if (button.Name == "QuantitySubtractButton")
            {
                cart.Remove(pe.Product, 1);
                totalCostLabel.Text = $"{cart.FinalPrice} kr";
                var c = GetCartItemBox();
                if (c.HasDiscountInfo)
                {
                    cart.TryGetRebate(pe.SerialNumber, out DiscountEntry de);
                    c.SetDiscountInfo(de);
                }
            }
            else if (button.Name == "RemoveButton")
            {
                cart.RemoveAll(pe.Product);
                totalCostLabel.Text = $"{cart.FinalPrice} kr";
                RefreshCartItemBoxView();
            }

            // lokal metod för att undvika kod-duplicering:
            CartItemBox GetCartItemBox()
                => (CartItemBox)(button.Parent.Parent.Parent.Parent.Parent.Parent);
        }

        private void OnThumbnailClick_CartItemBox(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        enum SortBy
        {
            Name, Price, Unspecified
        }
    }
}
