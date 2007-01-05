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
