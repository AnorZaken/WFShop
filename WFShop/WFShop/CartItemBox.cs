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

        // Fields
        private Color accentColor;
        private string controlFont;
        private int quantity;

        // Control properties.
        public Button RemoveButton { get; set; }
        public Button QuantitySubtractButton { get; set; }
        public Button QuantityAddButton { get; set; }

        // Properties
        public ProductEntry ProductEntry { get; set; }
        private int Quantity
        {
            get => quantity;
            set => quantity = value < 0 ? 0 : value;
        }

        public Color AccentColor
        {
            private get => accentColor;
            set
            {
                accentColor = value;
                ReInitialise();
            }
        }

        public string ControlFont
        {
            private get => controlFont;
            set
            {
                controlFont = value;
                ReInitialise();
            }
        }

        public CartItemBox(ProductEntry productEntry, int width = 450) : base(text: "", left: 0, top: 0, width, height: 50)
        {
            ProductEntry = productEntry;
            quantity = ProductEntry.Amount;
            Initialise();
        }

        private void Initialise()
        {
            // TODO: Gör att kontrollen anpassar sig till förälderns storlek.

            Panel panel = new Panel { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle };
            Controls.Add(panel);

            TableLayoutPanel table = new TableLayoutPanel
            {
                RowCount = 1,
                Dock = DockStyle.Fill
            };
            panel.Controls.Add(table);

            // Statisk storlek på thumbnail.
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, width: Height));
            // Produktnamn.
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            // Pris.
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            // QuantityPanel-kontroller.
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            // Totalpris.
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            // Ta bort-knapp.
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            // Allt = 100%

            PictureBox productThumbnail = new PictureBox
            {
                BackColor = SetAccentColor(),
                Margin = new Padding(0),
                Dock = DockStyle.Fill
            };
            table.Controls.Add(productThumbnail, 0, 0);
            productThumbnail.Click += OnProductThumbnailClick;

            Label productNameLabel = new Label
            {
                Text = ProductEntry.Product.Name,
                Font = new Font(SetControlFont(), 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };
            table.Controls.Add(productNameLabel, 1, 0);
            productNameLabel.Click += OnProductNameLabelClick;

            // Pris-etiketten
            table.Controls.Add(new Label
            {
                Text = $"{ProductEntry.Product.Price} kr",
                Font = new Font(SetControlFont(), 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            }, 2, 0);

            // Mini-kontroll som lägger till och tar bort x antal produkter.
            table.Controls.Add(CreateQuantityPanel(), 3, 0);

            totalPriceLabel = new Label
            {
                Text = $"{GetTotalCost()} kr",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(SetControlFont(), 12, FontStyle.Bold),
                Dock = DockStyle.Fill
            };
            table.Controls.Add(totalPriceLabel, 4, 0);

            RemoveButton = new Button
            {
                Text = "🗙",
                Font = new Font(SetControlFont(), 12),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Red,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                // Tagga för enkel åtkomst.
                Tag = ProductEntry,
                Name = "RemoveButton"
            };
            table.Controls.Add(RemoveButton, 5, 0);
        }

        private Control CreateQuantityPanel()
        {
            // Mini-kontroll.
            Control quantityPanel = new Control("", 0, 0, 150, Height - 2) { Margin = new Padding(0) };

            Panel panel = new Panel { Dock = DockStyle.Fill };
            quantityPanel.Controls.Add(panel);

            TableLayoutPanel table = new TableLayoutPanel
            {
                ColumnCount = 3,
                Dock = DockStyle.Fill
            };
            panel.Controls.Add(table);
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));


            QuantitySubtractButton = new Button
            {
                Text = "-",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(SetControlFont(), 16, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = SetAccentColor(),
                ForeColor = Color.White,
                Margin = new Padding(3, 3, 0, 3),
                Dock = DockStyle.Fill,
                // Tagga för enkel åtkomst.
                Tag = ProductEntry,
                Name = "QuantitySubtractButton"
            };
            table.Controls.Add(QuantitySubtractButton);
            QuantitySubtractButton.Click += (s, e) =>
            {
                // Ändrar inte antalet produkter.
                Quantity--;
                quantityLabel.Text = Quantity.ToString();
                totalPriceLabel.Text = $"{GetTotalCost()} kr";
            };

            quantityLabel = new Label
            {
                Text = Quantity.ToString(),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(SetControlFont(), 12, FontStyle.Bold),
                BackColor = Color.White,
                Margin = new Padding(0, 5, 0, 5),
                Dock = DockStyle.Fill
            };
            table.Controls.Add(quantityLabel);

            QuantityAddButton = new Button
            {
                Text = "+",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(SetControlFont(), 16, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = SetAccentColor(),
                ForeColor = Color.White,
                Margin = new Padding(0, 3, 3, 3),
                Dock = DockStyle.Fill,
                // Tagga för enkel åtkomst.
                Tag = ProductEntry,
                Name = "QuantityAddButton"
            };
            table.Controls.Add(QuantityAddButton);
            QuantityAddButton.Click += (s, e) =>
            {
                // Ändrar inte antalet produkter.
                Quantity++;
                quantityLabel.Text = Quantity.ToString();
                totalPriceLabel.Text = $"{GetTotalCost()} kr";
            };

            return quantityPanel;
        }

        private void ReInitialise()
        {
            Controls.Clear();
            Initialise();
        }

        // Om accentColor är tomt sätt det till orange.
        private Color SetAccentColor() => accentColor.IsEmpty ? Color.Orange : accentColor;

        // Om controlFont inte har ett värde, sätt den till Arial.
        private string SetControlFont() => controlFont ?? "Arial";

        public decimal GetTotalCost() => ProductEntry.Product.Price * Quantity;

        public override string ToString() => $"CartItemBox describing: {ProductEntry.Product}";

        private void OnProductThumbnailClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnProductNameLabelClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
