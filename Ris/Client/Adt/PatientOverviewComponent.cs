using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="PatientComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PatientComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ExtensionPoint]
    public class PatientOverviewToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IPatientOverviewToolContext : IToolContext
    {
        PatientProfile SelectedProfile { get; }
        IDesktopWindow DesktopWindow { get; }
    }


    /// <summary>
    /// PatientComponent class
    /// </summary>
//    [AssociateView(typeof(PatientComponentViewExtensionPoint))]
    public class PatientOverviewComponent : SplitComponentContainer
    {
        class PatientOverviewToolContext : ToolContext, IPatientOverviewToolContext
        {
            private PatientOverviewComponent _component;

            internal PatientOverviewToolContext(PatientOverviewComponent component)
            {
                _component = component;
            }

            public PatientProfile SelectedProfile
            {
                get { return _component.Subject; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }
        }


        private PatientProfilePreviewComponent _preview;
        private PatientProfilePreviewComponent _detail;
 
        private PatientProfile _subject;
        private ToolSet _toolSet;
        private IAdtService _adtService;


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
            set
            {
                _subject = value;
                if (this.IsStarted)
                {
                    _preview.Subject = _subject;
                }
            }
        }

        public override void Start()
        {
            _adtService = ApplicationContext.GetService<IAdtService>();
            _adtService.PatientProfileChanged += PatientProfileChangedEventHandler;


            this.Pane1 = new SplitPane("Preview", _preview = new PatientProfilePreviewComponent(), 1);
            this.Pane2 = new SplitPane("Detail", _detail = new PatientProfilePreviewComponent(), 1);

            _toolSet = new ToolSet(new PatientOverviewToolExtensionPoint(), new PatientOverviewToolContext(this));

            Refresh();

            base.Start();
        }

        private void PatientProfileChangedEventHandler(object sender, EntityChangeEventArgs e)
        {
            // is the patient profile that changed the same one we are looking at?
            if (_subject != null && e.Change.EntityOID == _subject.OID)
            {
                _subject = _adtService.LoadPatientProfile(e.Change.EntityOID, false);
                Refresh();
            }
        }

        public override void Stop()
        {
            _toolSet.Dispose();
            _adtService.PatientProfileChanged -= PatientProfileChangedEventHandler;

            base.Stop();
        }

        public override IActionSet ExportedActions
        {
            get { return _toolSet.Actions; }
        }

        private void Refresh()
        {
            _preview.Subject = _subject;
        }
    }
}
