using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace WFShop
{
    class Receipt : Form
    {
        private DataGridView gridView;
        private readonly ShoppingCart cart;

        public Receipt(ShoppingCart cart)
        {
            this.cart = cart;
            Initialize();
        }

        private void Initialize()
        {
            gridView = new DataGridView
            {
                Font = new Font("Arial", 12),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                RowHeadersVisible = false,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.None,
                Dock = DockStyle.Fill
            };
            Controls.Add(gridView);

            string[] headers =
            {
                "Produktnummer", "Antal", "Produkt", "Pris", "Rabatt", "Totalt"
            };
            gridView.Columns.Add("serialNumber", "Produknummer");
            gridView.Columns.Add("amount", "Antal");
            gridView.Columns.Add("product", "Produkt");
            gridView.Columns.Add("price", "Pris");
            gridView.Columns.Add("discount", "Rabatt");
            gridView.Columns.Add("totalPrice", "Totalt");
            FillGridView();
        }

        private void FillGridView()
        {
            foreach (ProductAmount productEntry in cart)
                gridView.Rows.Add(
                    productEntry.SerialNumber,
                    productEntry.Amount,
                    productEntry.Product.Name,
                    productEntry.Product.Price,
                    cart.TryGetRebate(productEntry.SerialNumber, out DiscountAmount de) ? $"-{de.Amount}" : "",
                    productEntry.Amount * productEntry.Product.Price);
        }
    }
}
