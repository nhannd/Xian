using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare.Alert
{
    public abstract class AlertNotification : IAlertNotification
    {
        private string _rep;
        private string _sev;
        private string _type;
        private List<string> _details;

        protected AlertNotification(string representation, string severity, string type)
        {
            _rep = representation;
            _sev = severity;
            _type = type;
            _details = new List<string>();
        }

        #region IAlertNotification Members

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

        public List<string> Reasons
        {
            get { return _details; }
            set { _details = value; }
        }

        #endregion
    }
}
