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
        public Button AddToCartButton;

        // Fields
        private Color accentColor;
        private string controlFont;
        private Product product;

        // Properties
        public Color AccentColor
        {
            private get => accentColor;
            set
            {
;               accentColor = value;
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
            this.product = product;
            Initialise();
        }
        
        // Rita upp ProductBox-instansen.
        private void Initialise()
        {
            Panel panel = new Panel { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle };
            Controls.Add(panel);

            TableLayoutPanel table = new TableLayoutPanel
            {
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                //CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
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

            PictureBox productThumbnailBox = new PictureBox
            {
                BackColor = SetAccentColor(),
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                Cursor = Cursors.Hand
            };
            table.Controls.Add(productThumbnailBox);
            table.SetColumnSpan(productThumbnailBox, 2);
            productThumbnailBox.Click += OnProductThumbnailBoxClick;

            Label productNameLabel = new Label
            {
                Text = product.Name,
                Font = new Font(SetControlFont(), 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };
            table.Controls.Add(productNameLabel);
            table.SetColumnSpan(productNameLabel, 2);

            RichTextBox productDescriptionTextBox = new RichTextBox
            {
                Text = product.Description,
                Font = new Font(SetControlFont(), 12),
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
                Text = $"{product.Price} kr",
                Font = new Font(SetControlFont(), 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            });

            AddToCartButton = new Button
            {
                Text = "Lägg till",
                Font = new Font(SetControlFont(), 12, FontStyle.Bold),
                BackColor = SetAccentColor(),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Tag = product
            };
            table.Controls.Add(AddToCartButton);
            //addToCartButton.Click += OnAddToCartButtonClick;
        }

        private void ReInitialise()
        {
            Controls.Clear();
            Initialise();
        }

        private Color SetAccentColor() => accentColor.IsEmpty ? Color.Orange : accentColor;

        private string SetControlFont() => controlFont ?? "Arial";

        //private void OnAddToCartButtonClick(object sender, EventArgs e)
        //{
        //    FlowLayoutPanel flow = (FlowLayoutPanel)Tag;
        //    flow.Controls.Add(new CartItemBox(product));
        //}

        private void OnProductThumbnailBoxClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
