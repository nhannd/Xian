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
    }
}
