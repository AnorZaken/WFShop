using System;

namespace WFShop
{
    class Shop
    {
        public Shop(string name = "Shop")
        {
            Name = name;
        }

        readonly public string Name;

        public RecieptSaver RecieptSaver { get; private set; }
        public IRecieptFormatter RecieptFormatter => RecieptSaver.Formatter;

        public void InitializeRecieptSaver(string path, IRecieptFormatter formatter)
        {
            if (RecieptSaver != null)
                throw new InvalidOperationException("Already Initialized!");
            RecieptSaver = new RecieptSaver(path, formatter);
        }
    }
}
