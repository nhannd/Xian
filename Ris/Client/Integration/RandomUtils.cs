using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Integration
{
    public static class RandomUtils
    {
        private static Random _randomizer;
        private static Random Randomizer
        {
            get
            {
                if (_randomizer == null)
                    _randomizer = new Random(Platform.Time.Millisecond);

                return _randomizer;
            }
        }

        public static int RandomInteger
        {
            get { return RandomUtils.Randomizer.Next(); }
        }

        public static char RandomAlphabet
        {
            get { return Convert.ToChar(Convert.ToInt32(RandomUtils.Randomizer.Next(0, 25) + 65)); }
        }

        public static string GenerateRandomIntegerString(int length)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                builder.Append(RandomUtils.Randomizer.Next(0,9).ToString());
            }

            return builder.ToString();
        }

        public static string GenerateRandomString(int length)
        {
            StringBuilder builder = new StringBuilder();

            for (int i=0; i<length; i++)
            {
                builder.Append(RandomUtils.RandomAlphabet);
            }

            return builder.ToString();
        }

        public static TItem ChooseRandom<TItem>(IList<TItem> target)
        {
            if (target.Count == 0)
                return default(TItem);

            if (target.Count == 1)
                return target[0];

            int randomIndex = RandomUtils.Randomizer.Next(target.Count - 1);
            return target[randomIndex];
        }

    }
}
