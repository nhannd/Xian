using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using ClearCanvas.Common;

namespace SampleShred1
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    public class SampleShredServiceType : ISampleShred1Interface
    {
        public SampleShredServiceType()
        {
            Platform.Log("[" + AppDomain.CurrentDomain.FriendlyName + "]: SampleShredServiceType Constructor");
        }

        #region ISampleShred1Interface Members

        public int GetLastPrimeFound()
        {
            int currentPrime = GlobalStore.CurrentPrime;

            string logMessage = "[" + AppDomain.CurrentDomain.FriendlyName + "] SampleShredServiceType: GetLastPrimeFound() called and returning " + currentPrime.ToString();
            Platform.Log(logMessage);

            return currentPrime;
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
