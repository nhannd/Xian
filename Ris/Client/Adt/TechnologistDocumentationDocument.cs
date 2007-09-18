using System;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    public class TechnologistDocumentationDocument : Document
    {
        private readonly ModalityWorklistItem _item;

        public TechnologistDocumentationDocument(string accessionNumber, ModalityWorklistItem item, IDesktopWindow desktopWindow)
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
            return new TechnologistDocumentationComponent(_item);
        }
    }
}