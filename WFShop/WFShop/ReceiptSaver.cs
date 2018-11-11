using System.Collections.Generic;
using System.IO;

namespace WFShop
{
    // Klass som användas till att skapa ett kvitto och spara som en fil på datorn.
    class ReceiptSaver
    {
        public ReceiptSaver(string savePath, IReceiptFormatter formatter = null)
        {
            Path = savePath ?? "";
            Formatter = formatter ?? ReceiptFormatter.Default;
        }

        public string Path { get; }
        public IReceiptFormatter Formatter { get; }

        public void Save(IEnumerable<string> receipt)
            => File.WriteAllLines(Path, receipt);

        public IEnumerable<string> Save(ShoppingCart cart)
        {
            var receipt = Formatter.Format(cart);
            Save(receipt);
            return receipt;
        }
    }
}
