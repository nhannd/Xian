using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using System.Runtime.InteropServices;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Client.Formatting;

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

            public bool Confirm(string message, string type)
            {
                if (string.IsNullOrEmpty(type))
                    type = "okcancel";
                type = type.ToLower();

                if (type == MessageBoxActions.OkCancel.ToString().ToLower())
                {
                    return _component.Host.ShowMessageBox(message, MessageBoxActions.OkCancel) == DialogBoxAction.Ok;
                }
                else if (type == MessageBoxActions.YesNo.ToString().ToLower())
                {
                    return _component.Host.ShowMessageBox(message, MessageBoxActions.YesNo) == DialogBoxAction.Yes;
                }
                else
                {
                    throw new NotSupportedException("Type must be YesNo or OkCancel");
                }
            }

            public void Alert(string message)
            {
                _component.Host.ShowMessageBox(message, MessageBoxActions.Ok);
            }

            public string ResolveStaffName(string search)
            {
                StaffSummary staff = null;
                if (StaffFinder.ResolveNameInteractive(search, _component.Host.DesktopWindow, out staff))
                {
                    return PersonNameFormat.Format(staff.PersonNameDetail);
                }
                return null;
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
