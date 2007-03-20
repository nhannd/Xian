using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Common;

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
        EntityRef<PatientProfile> PatientProfile { get; }
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

            public EntityRef<PatientProfile> PatientProfile
            {
                get { return _component._patientProfileRef; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }
        }


        private PatientProfilePreviewComponent _preview;
        private PatientProfilePreviewComponent _detail;

        private EntityRef<PatientProfile> _patientProfileRef;
        private PatientProfile _patientProfile;

        private ToolSet _toolSet;
        private IAdtService _adtService;


        /// <summary>
        /// Constructor
        /// </summary>
        public PatientOverviewComponent(EntityRef<PatientProfile> profileRef)
            :base(SplitOrientation.Vertical)
        {
            _patientProfileRef = profileRef;
        }

        public override void Start()
        {
            _adtService = ApplicationContext.GetService<IAdtService>();
            _adtService.PatientProfileChanged += PatientProfileChangedEventHandler;


            this.Pane1 = new SplitPane(SR.TitlePreview, _preview = new PatientProfilePreviewComponent(), 1);
            this.Pane2 = new SplitPane(SR.TitleDetail, _detail = new PatientProfilePreviewComponent(), 1);

            _toolSet = new ToolSet(new PatientOverviewToolExtensionPoint(), new PatientOverviewToolContext(this));

            Refresh();

            base.Start();
        }

        private void PatientProfileChangedEventHandler(object sender, EntityChangeEventArgs e)
        {
            // is the patient profile that changed the same one we are looking at?
            if (_patientProfileRef != null && e.EntityRef.Equals(_patientProfileRef))
            {
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
            _patientProfile = _adtService.LoadPatientProfile(_patientProfileRef, false);
            this.Host.SetTitle(string.Format(SR.TitlePatientComponent,
                Format.Custom(_patientProfile.Name),
                Format.Custom(_patientProfile.Mrn)));

            _preview.PatientProfileRef = _patientProfileRef;
        }
    }
}
