using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Healthcare.Alert
{
    // Sample alert using Specification to test address
    //
    //[ExtensionOf(typeof(PatientAlertExtensionPoint))]
    //class AddressAlert : PatientAlert
    //{
    //    private class AddressAlertNotification : AlertNotification
    //    {
    //        public AddressAlertNotification ()
    //            : base("Wrong side of the tracks", "Low", "Address alert")
    //        {
    //        }
    //    }

    //    private IDictionary<string, ISpecification> _specs;

    //    public AddressAlert()
    //        : base(new AddressAlertNotification())
    //    {
    //        LoadSpecifications();
    //    }

    //    private void LoadSpecifications()
    //    {
    //        ResourceResolver rr = new ResourceResolver(this.GetType().Assembly);
    //        string resourceName = string.Format("{0}.cfg.xml", this.GetType().Name);
    //        try
    //        {
    //            using (Stream xmlStream = rr.OpenResource(resourceName))
    //            {
    //                SpecificationFactory specFactory = new SpecificationFactory(xmlStream);
    //                _specs = specFactory.GetAllSpecifications();
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            // no cfg file for this component
    //            _specs = new Dictionary<string, ISpecification>();
    //        }
    //    }

    //    protected override bool DoTest(Patient patient, IPersistenceContext context)
    //    {
    //        foreach (KeyValuePair<string, ISpecification> kvp in _specs)
    //        {
    //            TestResult result = kvp.Value.Test(patient);
    //            if (result.Success == false)
    //            {
    //                _alertNotificationTemplate.Reason = String.Format("{0}\r\n{1}", _alertNotificationTemplate.Reason, result.Reason);
    //                return false;
    //            }
    //        }
    //        return true;
    //    }
    //}
}
