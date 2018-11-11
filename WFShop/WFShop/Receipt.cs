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
        private RichTextBox textBox;
        private readonly IEnumerable<string> formattedReceipt;

        public Receipt(IEnumerable<string> formattedReceipt)
        {
            this.formattedReceipt = formattedReceipt;
            Text = "Kvitto";
            Size = new Size(500, 890);
            Initialize();
        }

        private void Initialize()
        {
            textBox = new RichTextBox
            {
                Text = "",
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                Font = new Font("Arial", 12),
                Dock = DockStyle.Fill
            };
            Controls.Add(textBox);
            PrintReceipt();
        }

        private void PrintReceipt()
        {
            foreach (string line in formattedReceipt)
                textBox.Text += $"{line}\n";
        }
    }
}
