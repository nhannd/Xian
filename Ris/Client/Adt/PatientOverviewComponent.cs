using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="PatientComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PatientComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PatientComponent class
    /// </summary>
//    [AssociateView(typeof(PatientComponentViewExtensionPoint))]
    public class PatientOverviewComponent : SplitComponentContainer
    {
        private PatientProfilePreviewComponent _preview;
        private PatientProfilePreviewComponent _detail;
 
        private PatientProfile _subject;


        /// <summary>
        /// Constructor
        /// </summary>
        public PatientOverviewComponent(PatientProfile subject)
            :base(SplitOrientation.Vertical)
        {
            _subject = subject;
        }

        public PatientProfile Subject
        {
            get { return _subject; }
        }

        public override void Start()
        {
            this.Pane1 = new SplitPane("Preview", _preview = new PatientProfilePreviewComponent(), 1);
            this.Pane2 = new SplitPane("Detail", _detail = new PatientProfilePreviewComponent(), 1);

            _preview.Subject = _subject;

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }
    }
}
