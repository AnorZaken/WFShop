using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace WFShop
{
    class CartItemBox : Control
    {
        // Control fields
        private Label quantityLabel;
        private Label totalPriceLabel;
        private Control discount = null;

        // Fields
        private Color accentColor;
        private string controlFont;
        private int quantity;

        // Control properties.
        public Button RemoveButton { get; set; }
        public Button QuantitySubtractButton { get; set; }
        public Button QuantityAddButton { get; set; }
        public PictureBox Thumbnail { get; set; }

        // Properties
        public ProductAmount ProductEntry { get; set; }

        private int Quantity
        {
            get => quantity;
            set => quantity = value < 1 ? 1 : value;
        }

        public Color AccentColor
        {
            private get => accentColor;
            set
            {
                accentColor = value;
                ReloadControl();
            }
        }

        public string ControlFont
        {
            private get => controlFont;
            set
            {
                controlFont = value;
                ReloadControl();
            }
        }

        public bool HasDiscountInfo
            => table.RowCount == 2;

        public CartItemBox(ProductAmount productEntry, int width = 450) : base(text: "", left: 0, top: 0, width, height: 50)
        {
            ProductEntry = productEntry;
            quantity = ProductEntry.Amount;
            Initialize();
        }

        private TableLayoutPanel table;

        private void Initialize()
        {
            var panel = new Panel { Dock = DockStyle.Fill };
            Controls.Add(panel);

            table = new TableLayoutPanel { RowCount = 1, Dock = DockStyle.Fill };
            panel.Controls.Add(table);

            // Statisk storlek på thumbnail.
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, width: Height));
            // Produktnamn.
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            // Pris.
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            // QuantityPanel-kontroller.
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            // Totalpris.
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            // Ta bort-knapp.
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));

            Thumbnail = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White,
                Margin = new Padding(0),
                Dock = DockStyle.Fill,
                Cursor = Cursors.Hand
            };
            table.Controls.Add(Thumbnail, 0, 0);

            // Produktnamnet
            var productNameLabel = new Label
            {
                Text = ProductEntry.Product.Name,
                Font = new Font(GetControlFont(), 8, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };
            table.Controls.Add(productNameLabel, 1, 0);

            // Pris-etiketten
            table.Controls.Add(new Label
            {
                Text = $"{ProductEntry.Product.Price} kr",
                Font = new Font(GetControlFont(), 8, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            }, 2, 0);

            // Mini-kontroll som lägger till och tar bort x antal produkter.
            table.Controls.Add(CreateQuantityPanel(), 3, 0);

            totalPriceLabel = new Label
            {
                Text = $"{GetTotalCost()} kr",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(GetControlFont(), 8, FontStyle.Bold),
                Dock = DockStyle.Fill
            };
            table.Controls.Add(totalPriceLabel, 4, 0);

            RemoveButton = new Button
            {
                Text = "🗙",
                Font = new Font(GetControlFont(), 12),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = GetAccentColor(),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                // Tagga för enkel åtkomst.
                Tag = ProductEntry,
                Name = nameof(RemoveButton)
            };
            table.Controls.Add(RemoveButton, 5, 0);
            RemoveButton.Click += (s, e) => Parent.Controls.Remove(this);
            RemoveButton.MouseEnter += (s, e) => RemoveButton.BackColor = Color.Red;
            RemoveButton.MouseLeave += (s, e) => RemoveButton.BackColor = GetAccentColor();
        }

        public void RemoveDiscontInfo()
        {
            if (HasDiscountInfo)
            {
                table.Controls.Remove(discount);
                table.RowCount = 1;
                //table.Height /= 2;
                Height /= 2;
            }
        }

        public void SetDiscountInfo(DiscountAmount discountEntry) // TODO!
        {
            if (discountEntry.Amount == 0)
            {
                RemoveDiscontInfo();
            }
            else if (HasDiscountInfo)
            {
                discount.Text = discountEntry.ToString();
            }
            else
            {
                table.RowCount = 2;
                //table.Height *= 2;
                Height *= 2;
                discount = new Label() { Text = discountEntry.ToString(), Font = new Font(GetControlFont(), 8), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight };
                table.SetColumnSpan(discount, 6);
                table.Controls.Add(discount, 0, 1);
            }
        }

        private Control CreateQuantityPanel()
        {
            // Mini-kontroll.
            var quantityPanel = new Control("", 0, 0, 150, Height - 2) { Margin = new Padding(0) };

            var panel = new Panel { Dock = DockStyle.Fill };
            quantityPanel.Controls.Add(panel);

            var table = new TableLayoutPanel { ColumnCount = 3, Dock = DockStyle.Fill };
            panel.Controls.Add(table);
            // QuantitySubtractButton
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            // quantityLabel
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            // QuantityAddLabel
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            QuantitySubtractButton = new Button
            {
                Text = "-",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(GetControlFont(), 16, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = GetAccentColor(),
                ForeColor = Color.White,
                Margin = new Padding(3, 3, 0, 3),
                Dock = DockStyle.Fill,
                // Tagga för enkel åtkomst.
                Tag = ProductEntry,
                Name = nameof(QuantitySubtractButton)
            };
            table.Controls.Add(QuantitySubtractButton);
            QuantitySubtractButton.Click += (s, e) =>
            {
                // Ta bort sig själv om villkoret uppfylls.
                if (Quantity <= 1)
                    Parent.Controls.Remove(this);
                // Obs! Ändrar inte på antalet produkter utan bara värdet på den lokala variabeln quantity.
                Quantity--;
                quantityLabel.Text = Quantity.ToString();
                totalPriceLabel.Text = $"{GetTotalCost()} kr";
            };

            quantityLabel = new Label
            {
                Text = Quantity.ToString(),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(GetControlFont(), 12, FontStyle.Bold),
                BackColor = Color.White,
                Margin = new Padding(0, 5, 0, 5),
                Dock = DockStyle.Fill
            };
            table.Controls.Add(quantityLabel);

            QuantityAddButton = new Button
            {
                Text = "+",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(GetControlFont(), 16, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = GetAccentColor(),
                ForeColor = Color.White,
                Margin = new Padding(0, 3, 3, 3),
                Dock = DockStyle.Fill,
                // Tagga för enkel åtkomst.
                Tag = ProductEntry,
                Name = nameof(QuantityAddButton)
            };
            table.Controls.Add(QuantityAddButton);
            QuantityAddButton.Click += (s, e) =>
            {
                // Obs! Ändrar inte på antalet produkter utan bara värdet på den lokala variabeln quantity.
                Quantity++;
                quantityLabel.Text = Quantity.ToString();
                totalPriceLabel.Text = $"{GetTotalCost()} kr";
            };

            return quantityPanel;
        }

        private void ReloadControl()
        {
            Controls.Clear();
            Initialize();
        }

        // Om accentColor är tomt sätt det till orange.
        private Color GetAccentColor() => accentColor.IsEmpty ? Color.Orange : accentColor;

        // Om controlFont inte har ett värde, sätt den till Arial.
        private string GetControlFont() => controlFont ?? "Arial";

        public decimal GetTotalCost() => ProductEntry.Product.Price * Quantity;

        public override string ToString() => $"CartItemBox describing: {ProductEntry.Product}";
    }
}
