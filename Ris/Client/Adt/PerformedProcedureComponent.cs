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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "global-menus/MenuTools/Perform Procedure")]
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
            _component = new PerformedProcedureComponent();
            ApplicationComponent.LaunchAsWorkspace(this.Context.DesktopWindow,
                _component,
                "Perform Procedure",
                delegate(IApplicationComponent c) {  });
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
                    return PersonNameFormat.Format(staff.Name);
                }
                return null;
            }

            public string DateFormat
            {
                get { return Format.DateFormat; }
            }

            public string TimeFormat
            {
                get { return Format.TimeFormat; }
            }

            public string DateTimeFormat
            {
                get { return Format.DateTimeFormat; }
            }

            public string GetData(string tag)
            {
                // TODO retrieve data associated with tag
                return PerformedProcedureComponentSettings.Default.DetailsData;
            }

            public void SetData(string tag, string data)
            {
                // TODO set data associated with tag
                PerformedProcedureComponentSettings.Default.DetailsData = data;
            }

        }

        private ScriptCallback _scriptCallback;
        private event EventHandler _beforeAccept;


        /// <summary>
        /// Constructor
        /// </summary>
        public PerformedProcedureComponent()
        {
            _scriptCallback = new ScriptCallback(this);
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public event EventHandler BeforeAccept
        {
            add { _beforeAccept += value; }
            remove { _beforeAccept -= value; }
        }
        public string DetailsPageUrl
        {
            get { return PerformedProcedureComponentSettings.Default.DetailsPageUrl; }
        }

        public ScriptCallback ScriptObject
        {
            get { return _scriptCallback; }
        }

        public void Validate()
        {
            this.ShowValidation(!this.ValidationVisible);
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
            }
            else
            {
                EventsHelper.Fire(_beforeAccept, this, EventArgs.Empty);

                this.ExitCode = ApplicationComponentExitCode.Normal;
                this.Host.Exit();
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            this.Host.Exit();
        }

        #endregion

    }
}
