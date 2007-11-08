using System;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    class ProtocolEditorComponentDocument : Document
    {
        private readonly ReportingWorklistItem _item;

        public ProtocolEditorComponentDocument(string accessionNumber, ReportingWorklistItem item, IDesktopWindow desktopWindow)
            : base(accessionNumber, desktopWindow)
        {
            if(string.IsNullOrEmpty(accessionNumber))
            {
                throw new ArgumentException("Cannot be null or empty", "accessionNumber");
            }
            if(item == null)
            {
                throw new ArgumentNullException("item");
            }

            _item = item;
        }

        protected override string GetTitle()
        {
            return string.Format("A# {0} - {1}, {2}", _item.AccessionNumber, _item.PersonNameDetail.FamilyName, _item.PersonNameDetail.GivenName);
        }

        protected override IApplicationComponent GetComponent()
        {
            ProtocolEditorComponent component = new ProtocolEditorComponent(_item);

            component.ProtocolAccepted += ProtocolFinished;
            component.ProtocolCancelled += ProtocolNotFinished;
            component.ProtocolSaved += ProtocolNotFinished;
            component.ProtocolSuspended += ProtocolFinished;
            component.ProtocolRejected += ProtocolFinished;

            return component;
        }

        private void ProtocolNotFinished(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ProtocolFinished(object sender, EventArgs e)
        {
            // Update folder counts

            this.Close();
        }
    }
}
