using System;

namespace WFShop
{
    partial class Product
    {
        public class DuplicateException : Exception
        {
            internal DuplicateException(Product old, Product @new)
                : base($"Cannot add a new poduct with serialnumber {old.SerialNumber} because a different product with the same serialnumber already exists!")
            {
                Old = old;
                New = @new;
            }

            // Useful for debugging:
            internal readonly Product Old, New;
        }
    }
}
