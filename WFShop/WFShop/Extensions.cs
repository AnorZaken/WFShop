using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFShop
{
    static class Extensions
    {
        public static bool RangeIsWhiteSpace(this string s, int startIndex, int length)
            => RangeIsWhiteSpace(s, startIndex, length, Range.Option.Inclusive_Length);

        public static bool RangeIsWhiteSpace(this string s, int rangeValue1, int rangeValue2, Range.Option rangeOption)
        {
            Range.Normalize(ref rangeValue1, ref rangeValue2, rangeOption);
            if (rangeValue1 > s.Length)
                throw new ArgumentOutOfRangeException(message: "StartIndex not allowed to be greater than the length of the string.", null);
            for (int i = 0; i < rangeValue2; ++i)
                if (!char.IsWhiteSpace(s[rangeValue1 + i]))
                    return false;
            return true;
        }
    }
}
