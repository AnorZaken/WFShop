using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WFShop
{
    // Klass som användas till att skapa ett kvitto och spara som en fil på datorn.
    class RecieptSaver
    {
        public RecieptSaver(string savePath, IRecieptFormatter formatter = null)
        {
            Path = savePath ?? "";
            Formatter = formatter ?? RecieptFormatter.Default;
        }

        public string Path { get; }
        public IRecieptFormatter Formatter { get; }

        public void Save(IEnumerable<string> reciept)
            => File.WriteAllLines(Path, reciept);

        public IEnumerable<string> Save(ShoppingCart cart)
        {
            var reciept = Formatter.Format(cart);
            Save(reciept);
            return reciept;
        }
    }
}
