#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
