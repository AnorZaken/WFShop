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
        private Action OnRemoveButtonClicked;

        // Control properties.
        private Button removeButton;
        public ProductEntry ProductEntry { get; }

        // Properties
        public int Quantity
        {
            get => quantity;
            set
            {
                // TODO: Kan fel uppstå?
                quantity = value < 1 ? 1 : value;
                quantityLabel.Text = quantity.ToString();
                totalPriceLabel.Text = $"{GetTotalCost()} kr";
            }
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

        public CartItemBox(ProductEntry productEntry, Action onRemoveButtonClicked, int width = 450) : base(text: "", left: 0, top: 0, width, height: 50)
        {
            // TODO: Gör så att quantity aldrig kan vara mindre än 1.
            ProductEntry = productEntry;
            quantity = productEntry.Amount;
            //this.quantity = quantity;
            OnRemoveButtonClicked = onRemoveButtonClicked;
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

            removeButton = new Button
            {
                Text = "🗙",
                Font = new Font(SetControlFont(), 12),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Red,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill
            };
            table.Controls.Add(removeButton, 5, 0);
            removeButton.Click += OnRemoveButtonClick;
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

            Button quantitySubtractButton = new Button
            {
                Text = "-",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(SetControlFont(), 16, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = SetAccentColor(),
                ForeColor = Color.White,
                Margin = new Padding(3, 3, 0, 3),
                Dock = DockStyle.Fill
            };
            table.Controls.Add(quantitySubtractButton);
            quantitySubtractButton.Click += (s, e) => Quantity--;

            quantityLabel = new Label
            {
                Text = quantity.ToString(),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(SetControlFont(), 12, FontStyle.Bold),
                BackColor = Color.White,
                Margin = new Padding(0, 5, 0, 5),
                Dock = DockStyle.Fill
            };
            table.Controls.Add(quantityLabel);

            Button quantityAddButton = new Button
            {
                Text = "+",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(SetControlFont(), 16, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = SetAccentColor(),
                ForeColor = Color.White,
                Margin = new Padding(0, 3, 3, 3),
                Dock = DockStyle.Fill
            };
            table.Controls.Add(quantityAddButton);
            quantityAddButton.Click += (s, e) => Quantity++;

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

        public decimal GetTotalCost() => ProductEntry.Product.Price * quantity;

        public override string ToString() => $"CartItemBox describing: {ProductEntry.Product}";

        private void OnProductThumbnailClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnRemoveButtonClick(object sender, EventArgs e) => OnRemoveButtonClicked();

        private void OnProductNameLabelClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
