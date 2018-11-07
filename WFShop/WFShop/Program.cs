using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace WFShop
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            //Discount Test!
            Discounts.BuyXPayForY.Parser.ParseOrNull(
                new Dictionary<string, string>
                {
                    ["Type"] = "BXP4Y",
                    ["Name"] = "3 för 2 Apelsiner!",
                    ["Desc"] = "...",
                    ["ProductSN"] = "273984130",
                    ["BuyX"] = "3",
                    ["PayY"] = "2",
                }
                );

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MyForm());
        }
    }
}
