using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
    public delegate void ApplicationComponentExitDelegate(IApplicationComponent component);    
    
    /// <summary>
    /// Abstract base class for all application components.  Components should extend this class
    /// rather than implement <see cref="IApplicationComponent"/> directly, as it provides default
    /// implementations that behave well in most situations.
    /// </summary>
    public abstract class ApplicationComponent : IApplicationComponent
    {
        /// <summary>
        /// Executes the specified application component in a new workspace.  The exit callback will be invoked
        /// when the workspace is closed.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
        /// <param name="exitCallback"></param>
        /// <returns></returns>
        public static IWorkspace LaunchAsWorkspace(
            IApplicationComponent component,
            string title,
            ApplicationComponentExitDelegate exitCallback)
        {
            IExtensionPoint viewExtensionPoint = GetViewExtensionPoint(component.GetType());
            IWorkspace workspace = new ApplicationComponentHostWorkspace(title, component, viewExtensionPoint, exitCallback);
            DesktopApplication.WorkspaceManager.Workspaces.Add(workspace);
            return workspace;
        }

        /// <summary>
        /// Executes the specified application component in a modal dialog box.  This call will block until
        /// the dialog box is closed.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static ApplicationComponentExitCode LaunchAsDialog(
            IApplicationComponent component,
            string title)
        {
            ApplicationComponentHostDialog hostDialog = new ApplicationComponentHostDialog(title, component);
            return hostDialog.RunModal();
        }

        /// <summary>
        /// Returns the view extension point associated with the specified application component type by
        /// looking for an attribute of type <see cref="ApplicationComponentViewAttribute"/>.
        /// </summary>
        /// <param name="applicationComponentType"></param>
        /// <returns></returns>
        public static IExtensionPoint GetViewExtensionPoint(Type applicationComponentType)
        {
            object[] attrs = applicationComponentType.GetCustomAttributes(typeof(ApplicationComponentViewAttribute), false);
            if (attrs.Length == 0)
                throw new Exception("View attribute not specified");    //TODO elaborate

            ApplicationComponentViewAttribute viewAttribute = (ApplicationComponentViewAttribute)attrs[0];
            return (IExtensionPoint)Activator.CreateInstance(viewAttribute.ViewExtensionPointType);
        }



        /// <summary>
        /// This class is intentionally not exposed as an extension point because
        /// it is not intended that any extensions ever be implemented - it is only
        /// used by the StubToolContext
        /// </summary>
        public class StubToolExtensionPoint : ExtensionPoint<ITool>
        {
        }

        private IApplicationComponentHost _host;
        private ToolSet _stubToolSet;
        private ApplicationComponentExitCode _exitCode;
        private bool _modified;
        private event EventHandler _modifiedChanged;

        public ApplicationComponent()
        {
            _exitCode = ApplicationComponentExitCode.Normal;    // default exit code
        }

        /// <summary>
        /// Provides subclasses with access to the host
        /// </summary>
        protected IApplicationComponentHost Host
        {
            get { return _host; }
        }

        /// <summary>
        /// Convenience method to fire the <see cref="ModifiedChanged"/> event.
        /// Note that it is not necessary to explicitly call this method if the 
        /// default implementation of the <see cref="Modified"/> property is used,
        /// since the event is fired automatically.
        /// 
        /// This method is provided for situations where the subclass has chosen
        /// to override the <see cref="Modified"/> property.
        /// </summary>
        protected void FireModifiedChanged()
        {
            EventsHelper.Fire(_modifiedChanged, this, new EventArgs());
        }

        #region IApplicationComponent Members

        public void SetHost(IApplicationComponentHost host)
        {
            _host = host;
        }

        /// <summary>
        /// Returns an empty <see cref="IToolSet"/>.  Subclasses may override this property if they
        /// wish to expose a toolset to the framework.
        /// </summary>
        public virtual IToolSet ToolSet
        {
            get
            {
                if (_stubToolSet == null)
                {
                    _stubToolSet = new ToolSet(new StubToolExtensionPoint(), new ToolContext());
                }
                return _stubToolSet;
            }
        }

        /// <summary>
        /// Default implementation of <see cref="IApplicationComponent.Start"/>
        /// </summary>
        public virtual void Start()
        {
        }

        /// <summary>
        /// Default implementation of <see cref="IApplicationComponent.Stop"/>
        /// </summary>
        public virtual void Stop()
        {
        }

        /// <summary>
        /// Default implementation of <see cref="IApplicationComponent.CanExit"/>.
        /// Checks the <see cref="Modified"/> property, and if true, presents a standard
        /// confirmation dialog to the user asking whether or not changes should be
        /// retained.
        /// </summary>
        public virtual bool CanExit()
        {
            if (this.Modified)
            {
                DialogBoxAction result = this.Host.ShowMessageBox("Save changes before closing?", MessageBoxActions.YesNoCancel);
                switch (result)
                {
                    case DialogBoxAction.Yes:
                        this.ExitCode = ApplicationComponentExitCode.Normal;
                        return true;
                    case DialogBoxAction.No:
                        this.ExitCode = ApplicationComponentExitCode.Cancelled;
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                // this is equivalent to cancelling
                this.ExitCode = ApplicationComponentExitCode.Cancelled;
            }
            return true;
        }

        /// <summary>
        /// Default implementation of <see cref="IApplicationComponent.Modified"/>
        /// Set this property from within the subclass.
        /// </summary>
        public virtual bool Modified
        {
            get { return _modified; }
            protected set
            {
                if (value != _modified)
                {
                    _modified = value;
                    FireModifiedChanged();
                }
            }
        }

        /// <summary>
        /// Default implementation of <see cref="IApplicationComponent.ModifiedChanged"/>
        /// </summary>
        public event EventHandler ModifiedChanged
        {
            add { _modifiedChanged += value; }
            remove { _modifiedChanged -= value; }
        }

        /// <summary>
        /// Default implementation of <see cref="IApplicationComponent.ExitCode"/>
        /// Set this property from within the subclass.
        /// </summary>
        public virtual ApplicationComponentExitCode ExitCode
        {
            get { return _exitCode; }
            protected set { _exitCode = value; }
        }

        #endregion
    }
}
