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
        // Controls field
        private Button addToCartButton;
        private RichTextBox productDescriptionTextBox;

        // Istället för Tag
        private Product product;

        // Konstruktorn sätter defaultvärden och tar emot valfria värden som sätter storleken på kontrollen.
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
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };
            panel.Controls.Add(table);
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 200));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            PictureBox productThumbnailBox = new PictureBox
            {
                BackColor = Color.Black,
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
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            table.Controls.Add(productNameLabel);
            table.SetColumnSpan(productNameLabel, 2);

            productDescriptionTextBox = new RichTextBox
            {
                Text = product.Description,
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                BackColor = panel.BackColor,
                Dock = DockStyle.Fill
            };
            table.Controls.Add(productDescriptionTextBox);
            table.SetRowSpan(productDescriptionTextBox, 2);

            table.Controls.Add(new Label
            {
                Text = $"{product.Price} kr",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            });

            addToCartButton = new Button
            {
                Text = "Lägg till",
                Dock = DockStyle.Fill
            };
            table.Controls.Add(addToCartButton);
            addToCartButton.Click += OnAddToCartButtonClick;
        }

        // Behövs nog antagligen inte.
        private void Reinitialise()
        {
            Controls.Clear();
            Initialise();
        }

        private void OnAddToCartButtonClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnProductThumbnailBoxClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
