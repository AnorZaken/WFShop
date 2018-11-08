using System;

namespace WFShop
{
    public static class Range
    {
        public enum Option
        {
            Inclusive_Length,
            Inclusive_Inclusive,
            Inclusive_Exclusive,
            Exclusive_Inclusive,
            Exclusive_Exclusive,
        }

        public static void Normalize(int rangeValue1, int rangeValue2, Option rangeOption, out int startIndex, out int length)
        {
            Normalize(ref rangeValue1, ref rangeValue2, rangeOption);
            startIndex = rangeValue1;
            length = rangeValue2;
        }

        // When the method returns range1 will be startIndex and range2 will be length.
        public static void Normalize(ref int rangeValue1, ref int rangeValue2, Option rangeOption)
        {
            switch (rangeOption)
            {
                case Option.Inclusive_Length:
                    break;
                case Option.Inclusive_Inclusive:
                    rangeValue2 -= rangeValue1 - 1;
                    break;
                case Option.Inclusive_Exclusive:
                    rangeValue2 -= rangeValue1;
                    break;
                case Option.Exclusive_Inclusive:
                    rangeValue2 -= rangeValue1;
                    ++rangeValue1;
                    break;
                case Option.Exclusive_Exclusive:
                    ++rangeValue1;
                    rangeValue2 -= rangeValue1;
                    break;
                default:
                    throw new ArgumentException($"Invalid {nameof(Range)}.{nameof(Option)} value.");
            }
            if (rangeValue1 < 0)
                throw new ArgumentOutOfRangeException(message: "StartIndex not allowed to be less than 0.", null);
            if (rangeValue2 < 0)
                throw new ArgumentOutOfRangeException(message: "Length not allowed to be less than 0.", null);
        }
    }
}
