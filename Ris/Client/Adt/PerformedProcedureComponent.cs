using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using System.Runtime.InteropServices;

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
        /// The script callback is an object that is made available to the web browser so that
        /// the javascript code can invoke methods on the host.  It must be COM-visible.
        /// </summary>
        [ComVisible(true)]
        public class ScriptCallback
        {
            private PerformedProcedureComponent _component;

            public ScriptCallback(PerformedProcedureComponent component)
            {
                _component = component;
            }

            public void ShowMessageBox(string message)
            {
                _component.Host.ShowMessageBox(message, MessageBoxActions.Ok);
            }

            public string LookupStaff(string search)
            {
                return "";
            }
        }

        private ScriptCallback _scriptCallback;


        /// <summary>
        /// Constructor
        /// </summary>
        public PerformedProcedureComponent()
        {
            _scriptCallback = new ScriptCallback(this);
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

        public ScriptCallback ScriptObject
        {
            get { return _scriptCallback; }
        }

        public void Close()
        {
            this.Host.Exit();
        }

        #endregion

    }
}
