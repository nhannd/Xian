using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;

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
        EntityRef PatientProfile { get; }
        IDesktopWindow DesktopWindow { get; }
    }


    /// <summary>
    /// PatientComponent class
    /// </summary>
//    [AssociateView(typeof(PatientComponentViewExtensionPoint))]
    public class PatientOverviewComponent : ApplicationComponent
    {
        class PatientOverviewToolContext : ToolContext, IPatientOverviewToolContext
        {
            private PatientOverviewComponent _component;

            internal PatientOverviewToolContext(PatientOverviewComponent component)
            {
                _component = component;
            }

            public EntityRef PatientProfile
            {
                get { return _component._patientProfileRef; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }
        }


        private EntityRef _patientProfileRef;

        private ToolSet _toolSet;


        /// <summary>
        /// Constructor
        /// </summary>
        public PatientOverviewComponent(EntityRef profileRef)
        {
            _patientProfileRef = profileRef;
        }

        public override void Start()
        {
            _toolSet = new ToolSet(new PatientOverviewToolExtensionPoint(), new PatientOverviewToolContext(this));

            Refresh();

            base.Start();
        }
/*
        private void PatientProfileChangedEventHandler(object sender, EntityChangeEventArgs e)
        {
            // is the patient profile that changed the same one we are looking at?
            if (_patientProfileRef != null && e.EntityRef.Equals(_patientProfileRef))
            {
                Refresh();
            }
        }
*/
        public override void Stop()
        {
            _toolSet.Dispose();
 //           _adtService.PatientProfileChanged -= PatientProfileChangedEventHandler;

            base.Stop();
        }

        public override IActionSet ExportedActions
        {
            get { return _toolSet.Actions; }
        }

        private void Refresh()
        {
            this.Host.SetTitle(string.Format(SR.TitlePatientComponent,
                "Name",
                "MRN"));
        }
    }
}
