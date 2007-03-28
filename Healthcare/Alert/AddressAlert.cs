using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Healthcare.Alert
{
    [ExtensionOf(typeof(AlertExtensionPoint))]
    class AddressAlert : Alert
    {
        private class AddressAlertNotification : AlertNotification
        {
            public AddressAlertNotification ()
                : base("Wrong side of the tracks", "Low", "Address alert")
        	{
            }
        }

        public AddressAlert()
            : base(typeof(Patient), new AddressAlertNotification())
        {
        }
    }
}
