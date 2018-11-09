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
        private ComboBox filterComboBox;
        private RadioButton sortByNameRadioButton;
        private RadioButton sortByPriceRadioButton;

        // Fields
        private IReadOnlyCollection<Product> products;

        // Control properties

        // Properties
        private ShoppingCart cart;

        public MyForm()
        {
            Text = "<App Name>"; // TODO: läs från config
            Size = new Size(1570, 890);
            MinimumSize = new Size(1570, 890);
            products = Product.AllProducts;
            cart = LoadOrCreateCart();

            Initialize();
        }

        private static ShoppingCart LoadOrCreateCart()
        {
            if (!FileHandler.TryLoadShoppingCart(out ShoppingCart cart, out int errorCount))
                return new ShoppingCart();
            else if (errorCount != 0)
                MessageBox.Show("Fel uppstod då den sparade varukorgen laddades."
                    + "\nVar god kontrollera varukorgens innehåll."
                    + "\n(Mer info finns i programmets error-ström.)");
            return cart;
        }

        // Rita upp GUI:t.
        private void Initialize()
        {
            SizeChanged += (s, e) =>
            {
                foreach (var cib in cartItemBoxView.Controls)
                    (cib as CartItemBox).Width = (cib as CartItemBox).Parent.Width;
            };

            splitContainer = new SplitContainer
            {
                //BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill
            };
            Controls.Add(splitContainer);
            // En tredjedel av fönstret.
            splitContainer.SplitterDistance = (int)(Width / 1.5);
            splitContainer.SplitterMoved += (s, e) =>
            {
                if (splitContainer.SplitterDistance > (int)(Width / 1.5))
                    splitContainer.SplitterDistance = (int)(Width / 1.5);
            };
            splitContainer.SplitterMoving += (s, e) =>
            {
                if (splitContainer.SplitterDistance > (int)(Width / 1.5))
                    splitContainer.SplitterDistance = (int)(Width / 1.5);
            };

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
            filterComboBox.SelectedItem = filterComboBox.Items[0];
            // TODO: Läs in kategorier från textfil eller från List.            
        }


        private void CreatePanel2Controls()
        {
            TableLayoutPanel mainTable = new TableLayoutPanel
            {
                RowCount = 2,
                Dock = DockStyle.Fill
            };
            splitContainer.Panel2.Controls.Add(mainTable);
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            TableLayoutPanel cartViewTable = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 3,
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
                Dock = DockStyle.Fill,
                BackColor = Color.White,
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
            saveCartButton.Click += (s, e) =>
            {
                FileHandler.SaveShoppingCart(cart);
                MessageBox.Show("Din varukorg sparades.");
            };

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
                if (cart.ArticleCount > 0)
                {
                    FileHandler.SaveReceipt(cart);
                    MessageBox.Show("Kvittot har sparats till:\n" + $"\"{FileHandler.PathToReceipt}\"");
                    ClearCart();
                }
                else
                    MessageBox.Show("Din varukorg är tom.");
            };

            // Ladda om cartItemBoxView så att eventuella produkter i textfilen visas.
            RefreshCartItemBoxView();
        }

        private void ClearCart()
        {
            cart.Clear();
            RefreshCartItemBoxView();
            RefreshTotalCost();
        }

        private void CreateProductBoxes(IReadOnlyCollection<Product> products, string filter = "Alla", string sortBy = null)
        {
            flowProductBoxView.Controls.Clear();

            //Skapa ProductBox för varje produkt som finns i textfilen.

            // IEnumerable som innehåller filtrerad version av product-listan.
            IEnumerable<Product> filteredProducts = 
                filter == "Alla" || filter is null ?
                products.Select(p => p) :
                products.Where(p => p.Category.Name == filter);

            // IEnumerable som innehåller en sorterad version av product-listan.
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
            CreateProductBoxes(products, filterComboBox.SelectedItem as string, sortByNameRadioButton.Checked ? "Name" : sortByPriceRadioButton.Checked ? "Price" : "");
        }

        // this
        private void OnCouponCodeTextBoxChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            // We only allow a single coupon (for now anyway)
            cart.ClearCoupons();

            if (cart.AddCoupon(textBox.Text))
                textBox.BackColor = Color.Green;
            else if (textBox.Text.Length > 0)
                textBox.BackColor = Color.Red;
            else
                textBox.BackColor = Color.White;

            RefreshTotalCost();
        }

        // ProductBox
        private void OnAnyButtonClick_ProductBox(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Product product = (Product)button.Tag;

            if (button.Name == "AddToCartButton")
            {
                cart.Add(product);
                RefreshTotalCost();
                RefreshCartItemBoxView();
            }
        }

        private void RefreshTotalCost()
            => totalCostLabel.Text = $"{cart.FinalPrice} kr";

        private void OnThumbnailClick_ProductBox(object sender, EventArgs e)
        {
            Form enlargePhoto = new Form() { Size = new Size(750, 750) };
            enlargePhoto.Controls.Add(new PictureBox
            {
                Image = (sender as PictureBox).Image,
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Fill
            });
            enlargePhoto.Show();
        }

        // CartItemBox
        private void OnAnyButtonClick_CartItemBox(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            ProductEntry pe = (ProductEntry)button.Tag;

            if (button.Name == "QuantityAddButton")
            {
                cart.Add(pe.Product);
                RefreshTotalCost();
                var c = GetCartItemBox();
                if (cart.TryGetRebate(pe.SerialNumber, out DiscountEntry de))
                    c.SetDiscountInfo(de);
            }
            else if (button.Name == "QuantitySubtractButton")
            {
                cart.Remove(pe.Product);
                RefreshTotalCost();
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
                RefreshTotalCost();
            }

            // lokal metod för att undvika kod-duplicering:
            CartItemBox GetCartItemBox()
                => (CartItemBox)(button.Parent.Parent.Parent.Parent.Parent.Parent);
        }

        private void OnThumbnailClick_CartItemBox(object sender, EventArgs e)
        {
            Form enlargePhoto = new Form() { Size = new Size(750, 750) };
            enlargePhoto.Controls.Add(new PictureBox
            {
                Image = (sender as PictureBox).Image,
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Fill
            });
            enlargePhoto.Show();
        }
    }
}
