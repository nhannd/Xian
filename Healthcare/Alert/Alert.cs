using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Alert
{
    public abstract class AlertNotification : IAlertNotification
    {
        string _rep;
        string _sev;
        string _type;

        protected AlertNotification(string representation, string severity, string type)
        {
            _rep = representation;
            _sev = severity;
            _type = type;
        }

        #region IAlert Members

        public string Representation
        {
            get { return _rep; }
        }

        public string Severity
        {
            get { return _sev; }
        }

        public string Type
        {
            get { return _type; }
        }

        #endregion
    }

    public abstract class Alert : IAlert
    {
        private IDictionary<string, ISpecification> _specs;
        private Type _rootTestObjectType;
        private IAlertNotification _alertNotificationTemplate;

        protected Alert(Type rootTestObjectType, IAlertNotification alertNotificationTemplate)
        {
            _rootTestObjectType = rootTestObjectType;
            _alertNotificationTemplate = alertNotificationTemplate;

            LoadSpecifications();
        }

        #region IAlertTest Members

        public IAlertNotification Test(object o)
        {
            if (_rootTestObjectType.IsInstanceOfType(o))
            {
                return DoTest(o) == false
                    ? _alertNotificationTemplate
                    : null;
            }
            else
            {
                return null;
            }
        }

        #endregion

        private void LoadSpecifications()
        {
            ResourceResolver rr = new ResourceResolver(this.GetType().Assembly);
            string resourceName = string.Format("{0}.cfg.xml", this.GetType().Name);
            try
            {
                using (Stream xmlStream = rr.OpenResource(resourceName))
                {
                    SpecificationFactory specFactory = new SpecificationFactory(xmlStream);
                    _specs = specFactory.GetAllSpecifications();
                }
            }
            catch (Exception)
            {
                // no cfg file for this component
                _specs = new Dictionary<string, ISpecification>();
            }
        }

        protected bool DoTest(Object o)
        {
            foreach (KeyValuePair<string, ISpecification> kvp in _specs)
            {
                TestResult result = kvp.Value.Test(o);
                if (result.Success == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
