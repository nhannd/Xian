using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "global-menus/MenuTools/Performed Procedure")]
    [ClickHandler("apply", "Apply")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class PPTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
    {
        private PerformedProcedureComponent _component;

        public PPTool()
        {
        }

        public void Apply()
        {
            if (_component == null)
            {
                _component = new PerformedProcedureComponent();
                ApplicationComponent.LaunchAsWorkspace(this.Context.DesktopWindow,
                    _component,
                    "Performed Procedure",
                    delegate(IApplicationComponent c) { _component = null; });
            }
        }
    }

    /// <summary>
    /// Extension point for views onto <see cref="PerformedProcedureComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PerformedProcedureComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PerformedProcedureComponent class
    /// </summary>
    [AssociateView(typeof(PerformedProcedureComponentViewExtensionPoint))]
    public class PerformedProcedureComponent : ApplicationComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PerformedProcedureComponent()
        {
        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public string ReportPageUrl
        {
            get { return PerformedProcedureComponentSettings.Default.ReportPageUrl; }
        }

        public string ReportData
        {
            get { return PerformedProcedureComponentSettings.Default.ReportData; }
            set
            {
                PerformedProcedureComponentSettings.Default.ReportData = value;
                PerformedProcedureComponentSettings.Default.Save();
            }
        }

        public void Close()
        {
            this.Host.Exit();
        }

        #endregion

    }
}
