using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace WFShop
{
    class ProductBox : Control
    {
        // Control fields
        public Button AddToCartButton { get; set; }
        public PictureBox Thumbnail { get; set; }

        // Fields
        private Color accentColor;
        private string controlFont;

        // Control properties.

        // Properties
        public Product Product { get; }

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

        // Konstruktorn sätter defaultvärden eller tar emot valfria värden som sätter storleken på kontrollen.
        public ProductBox(Product product, int width = 250, int height = 300) : base(text: "", left: 0, top: 0, width, height)
        {
            Product = product;            
            Initialize();
        }
        
        // Rita upp ProductBox-instansen.
        private void Initialize()
        {
            Panel panel = new Panel { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle };
            Controls.Add(panel);

            TableLayoutPanel table = new TableLayoutPanel
            {
                ColumnCount = 2,
                Dock = DockStyle.Fill,
            };
            panel.Controls.Add(table);
            // Hälften av kontrollen höjd tas upp av produktbilden.
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, Height / 2));
            // Titel.
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            // Beskrivning.
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            // Statisk höjd på priset och knappens cell.
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            // Priset.
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            // Knappen.
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            Thumbnail = new PictureBox
            {
                BackColor = SetAccentColor(),
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                Margin = new Padding(0),
                Cursor = Cursors.Hand
            };
            table.Controls.Add(Thumbnail);
            table.SetColumnSpan(Thumbnail, 2);

            Label productNameLabel = new Label
            {
                Text = Product.Name,
                Font = new Font(SetControlFont(), 8, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };
            table.Controls.Add(productNameLabel);
            table.SetColumnSpan(productNameLabel, 2);

            RichTextBox productDescriptionTextBox = new RichTextBox
            {
                Text = Product.Description,
                Font = new Font(SetControlFont(), 8),
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                ReadOnly = true,
                BackColor = panel.BackColor,
                Dock = DockStyle.Fill
            };
            table.Controls.Add(productDescriptionTextBox);
            table.SetColumnSpan(productDescriptionTextBox, 2);
            table.SetRowSpan(productDescriptionTextBox, 2);

            // Pris-etiketten
            table.Controls.Add(new Label
            {
                Text = $"{Product.Price} kr",
                Font = new Font(SetControlFont(), 8, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            });

            AddToCartButton = new Button
            {
                Text = "Lägg till",
                Font = new Font(SetControlFont(), 8, FontStyle.Bold),
                BackColor = SetAccentColor(),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Tag = Product,
                Name = nameof(AddToCartButton)
            };
            table.Controls.Add(AddToCartButton);
        }

        private void ReInitialise()
        {
            Controls.Clear();
            Initialize();
        }

        public override string ToString() => $"ProductBox describing: {Product}";

        private Color SetAccentColor() => accentColor.IsEmpty ? Color.Orange : accentColor;

        private string SetControlFont() => controlFont ?? "Arial";
    }
}
