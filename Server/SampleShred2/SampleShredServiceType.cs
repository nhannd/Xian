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
using System.ServiceModel;
using ClearCanvas.Common;

namespace SampleShred2
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    public class SampleShredServiceType : ISampleShred2Interface
    {
        public SampleShredServiceType()
        {
            Platform.Log("[" + AppDomain.CurrentDomain.FriendlyName + "]: SampleShredServiceType Constructor");
        }

        #region ISampleShred1Interface Members

        public string GetLastPiFound()
        {
            double currentPi = GlobalStore.CurrentPi;
            int significantDigits = (int) Math.Log10(GlobalStore.Darts) + 1;
            StringBuilder digitFormatter = new StringBuilder();
            digitFormatter.AppendFormat("{{0:f{0}}}", significantDigits);

            StringBuilder piStringBuilder = new StringBuilder();
            piStringBuilder.AppendFormat(digitFormatter.ToString(), currentPi);

            string logMessage = "[" + AppDomain.CurrentDomain.FriendlyName + "] SampleShredServiceType: GetLastPiFound() called and returning " + piStringBuilder.ToString();
            Platform.Log(logMessage);

            return piStringBuilder.ToString();
        }

        #endregion

        private void FindNextPrime()
        {
            if (2 == _currentPrime)
            {
                _currentPrime = 3;
                return;
            }

            int numberToCheck = _currentPrime + 2;
            double squareRoot = Math.Sqrt(numberToCheck);

            int factor = 3; // we can start at 3, 'coz numberToCheck will never be even
            while (factor <= squareRoot)
            {
                if (0 == numberToCheck % factor)
                {
                    numberToCheck = numberToCheck + 2;
                    factor = 3;
                }
                else
                {
                    factor += 2;
                }

            }

            _currentPrime = numberToCheck;
            return;
        }

        private int _currentPrime = 2;
    }
}
