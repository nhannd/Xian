using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Provides a callback when an application component exits
    /// </summary>
    /// <param name="component">The component that exited</param>
    public delegate void ApplicationComponentExitDelegate(IApplicationComponent component);    
    
    /// <summary>
    /// Abstract base class for all application components. 
    /// </summary>
    /// <remarks>
    /// Components should extend this class rather than implement <see cref="IApplicationComponent"/> directly,
    /// as it provides a default implementation suitable for most situations.
    /// </remarks>
    public abstract class ApplicationComponent : IApplicationComponent, INotifyPropertyChanged, IDataErrorInfo
    {
        /// <summary>
        /// Executes the specified application component in a new workspace.  The exit callback will be invoked
        /// when the workspace is closed.
        /// </summary>
        /// <remarks>
        /// If the specified component throws an exception from the <see cref="Start"/> method, that exception
        /// will be propagate to the caller of this method and the component will not be launched.
        /// </remarks>
        /// <param name="desktopWindow">The desktop window in which the workspace will run.</param>
        /// <param name="component">The application component to launch.</param>
        /// <param name="title">The title of the workspace.</param>
        /// <param name="exitCallback">An optional callback to invoke when the workspace is closed.</param>
        /// <returns>The workspace that is hosting the component.</returns>
        public static Workspace LaunchAsWorkspace(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title,
            ApplicationComponentExitDelegate exitCallback)
        {
            return LaunchAsWorkspace(desktopWindow, component, title, null, exitCallback);
        }

        /// <summary>
        /// Executes the specified application component in a new workspace.  The exit callback will be invoked
        /// when the workspace is closed.
        /// </summary>
        /// <param name="desktopWindow">The desktop window in which the workspace will run.</param>
        /// <param name="component">The application component to launch.</param>
        /// <param name="title">The display title of the workspace.</param>
        /// <param name="name">The unique name of the workspace.</param>
        /// <param name="exitCallback">An optional callback to invoke when the workspace is closed.</param>
        /// <returns>The workspace that is hosting the component.</returns>
        public static Workspace LaunchAsWorkspace(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title,
            string name,
            ApplicationComponentExitDelegate exitCallback)
        {
            Platform.CheckForNullReference(desktopWindow, "desktopWindow");
            Platform.CheckForNullReference(component, "component");

            WorkspaceCreationArgs args = new WorkspaceCreationArgs(component, title, name);
            Workspace workspace = desktopWindow.Workspaces.AddNew(args);
            if (exitCallback != null)
            {
                workspace.Closed += delegate(object sender, ClosedEventArgs e)
                {
                    exitCallback(component);
                };
            }
            return workspace;
        }


        /// <summary>
        /// Executes the specified application component in a new shelf.  The exit callback will be invoked
        /// when the shelf is closed.
        /// </summary>
        /// <remarks>
        /// If the specified component throws an exception from its <see cref="Start"/> method, that exception
        /// will be propagate to the caller of this method and the component will not be launched.
        /// </remarks>
        /// <param name="desktopWindow">The desktop window in which the shelf will run.</param>
        /// <param name="component">The application component to launch.</param>
        /// <param name="title">The title of the shelf.</param>
        /// <param name="displayHint">A hint as to how the shelf should initially be displayed.</param>
        /// <param name="exitCallback">The callback to invoke when the shelf is closed.</param>
        /// <returns>The shelf that is hosting the component.</returns>
        public static Shelf LaunchAsShelf(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title,
            ShelfDisplayHint displayHint,
            ApplicationComponentExitDelegate exitCallback)
        {
            return LaunchAsShelf(desktopWindow, component, title, null, displayHint, exitCallback);
        }

        /// <summary>
        /// Executes the specified application component in a new shelf.  The exit callback will be invoked
        /// when the shelf is closed.
        /// </summary>
        /// <remarks>
        /// If the specified component throws an exception from its <see cref="Start"/> method, that exception
        /// will be propagate to the caller of this method and the component will not be launched.
        /// </remarks>
        /// <param name="desktopWindow">The desktop window in which the shelf will run.</param>
        /// <param name="component">The application component to launch.</param>
        /// <param name="title">The title of the shelf.</param>
        /// <param name="name">The unique name shelf.</param>
        /// <param name="displayHint">A hint as to how the shelf should initially be displayed.</param>
        /// <param name="exitCallback">The callback to invoke when the shelf is closed.</param>
        /// <returns>The shelf that is hosting the component.</returns>
        public static Shelf LaunchAsShelf(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title,
            string name,
            ShelfDisplayHint displayHint,
            ApplicationComponentExitDelegate exitCallback)
        {
            Platform.CheckForNullReference(desktopWindow, "desktopWindow");
            Platform.CheckForNullReference(component, "component");

            ShelfCreationArgs args = new ShelfCreationArgs(component, title, name, displayHint);
            Shelf shelf = desktopWindow.Shelves.AddNew(args);
            if (exitCallback != null)
            {
                shelf.Closed += delegate(object sender, ClosedEventArgs e)
                {
                    exitCallback(component);
                };
            }
            return shelf;
        }

        /// <summary>
        /// Executes the specified application component in a modal dialog box.  This call will block until
        /// the dialog box is closed.
        /// </summary>
        /// <remarks>
        /// If the specified component throws an exception from its <see cref="Start"/> method, that exception
        /// will be propagate to the caller of this method and the component will not be launched.
        /// </remarks>
        /// <param name="desktopWindow">The desktop window in which the dialog box is centered.</param>
        /// <param name="component">The application component to launch.</param>
        /// <param name="title">The title of the dialog box.</param>
        /// <returns>The exit code that the component exits with.</returns>
        public static ApplicationComponentExitCode LaunchAsDialog(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title)
        {
            return LaunchAsDialog(desktopWindow, component, title, null);
        }

        private static ApplicationComponentExitCode LaunchAsDialog(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title,
            string name)
        {
            DialogBoxCreationArgs args = new DialogBoxCreationArgs(component, title, name);
            desktopWindow.ShowDialogBox(args);
            return component.ExitCode;
        }

        private IApplicationComponentHost _host;
        private ApplicationComponentExitCode _exitCode;

        private bool _started;
        private bool _modified;
        private event EventHandler _modifiedChanged;

        private event EventHandler _allPropertiesChanged;
        private event PropertyChangedEventHandler _propertyChanged;

        private ValidationRuleSet _validation;
        private bool _showValidationErrors;
        private event EventHandler _validationVisibleChanged;


        /// <summary>
        /// Default constructor
        /// </summary>
        protected ApplicationComponent()
        {
            _exitCode = ApplicationComponentExitCode.Normal;    // default exit code
            
            // default empty validation rule set
            _validation = new ValidationRuleSet();
        }


        /// <summary>
        /// Gets or sets the <see cref="ValidationRuleSet"/> that is associated with this component.
        /// </summary>
        public ValidationRuleSet Validation
        {
            get { return _validation; }
            set { _validation = value; }
        }

        #region Protected members

        /// <summary>
        /// Gets the <see cref="IApplicationComponentHost"/> that is hosting this component.
        /// </summary>
        protected IApplicationComponentHost Host
        {
            get { return _host; }
        }

        /// <summary>
        /// Sets the exit code and asks the host to exit in a single call.
        /// </summary>
        /// <param name="exitCode"></param>
        protected void Exit(ApplicationComponentExitCode exitCode)
        {
            this.ExitCode = exitCode;
            this.Host.Exit();
        }

        /// <summary>
        /// Convenience method to fire the <see cref="ModifiedChanged"/> event.
        /// <remarks>
        /// Note that it is not necessary to explicitly call this method if the 
        /// default implementation of the <see cref="Modified"/> property is used,
        /// since the event is fired automatically when the property is set.
        /// This method is provided for situations where the subclass has chosen
        /// to override the <see cref="Modified"/> property.
        /// </remarks>
        /// </summary>
        protected void NotifyModifiedChanged()
        {
            EventsHelper.Fire(_modifiedChanged, this, EventArgs.Empty);
        }

        /// <summary>
        /// Notifies subscribers of the <see cref="PropertyChanged"/> event that the specified property has changed.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Notifies subscribers of the <see cref="AllPropertiesChanged"/> that all properties may have changed.
        /// A view should respond to this event by refreshing itself completely.
        /// </summary>
        protected void NotifyAllPropertiesChanged()
        {
            EventsHelper.Fire(_allPropertiesChanged, this, EventArgs.Empty);
        }

        #endregion

        #region IApplicationComponent Members

        /// <summary>
        /// Called by the framework to set the host.  For internal use only.
        /// </summary>
        /// <param name="host"></param>
        public void SetHost(IApplicationComponentHost host)
        {
			Platform.CheckForNullReference(host, "host");
            _host = host;
        }

        /// <summary>
        /// Returns the set of actions that the component wishes to export to the desktop.
        /// </summary>
        /// <remarks>
        /// The default implementation of this method returns an empty action set.
        /// </remarks>
        public virtual IActionSet ExportedActions
        {
            get { return new ActionSet(); }
        }

        /// <summary>
        /// Called by the host to initialize the application component.  Override this method to implement
        /// custom initialization logic.
        /// </summary>
        /// <remarks>
        /// Overrides must be sure to call the base implementation.
        /// </remarks>
        public virtual void Start()
        {
            AssertNotStarted();

            _started = true;
        }

        /// <summary>
        /// Called by the host when the application component is being terminated.  Override this method to implement
        /// custom termination logic.
        /// </summary>
        /// <remarks>
        /// Overrides must be sure to call the base implementation.
        /// </remarks>
        public virtual void Stop()
        {
            AssertStarted();

            _started = false;
        }

        /// <summary>
        /// Gets a value indicating whether this component is live or not. 
        /// </summary>
        public bool IsStarted
        {
            get { return _started; }
        }

        /// <summary>
        /// Called by the host to determine if the application component is in a state such that it can be stopped.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation simply check the <see cref="Modified"/> property to see if data has been modified.
        /// If data has been modified, and <see cref="UserInteraction.NotAllowed"/> is specified, it returns false.
        /// If data has been modified, and <see cref="UserInteraction.Allowed"/> is specified, it presents a standard
        /// confirmation dialog asking the user whether or not data should be saved or discarded, or the exit cancelled.
        /// If the user elects to save or discard data, the <see cref="ExitCode"/> property is set accordingly and a value
        /// of true is returned.  If the user elects to cancel, a value of false is returned.
        /// </para>
        /// <para>
        /// Override this method to provide custom logic for responding to this query.
        /// </para>
        /// </remarks>
        public virtual bool CanExit(UserInteraction interactive)
        {
            AssertStarted();

            if (interactive == UserInteraction.NotAllowed)
                return !_modified;

            if (_modified)
            {
				DialogBoxAction result = this.Host.ShowMessageBox(SR.MessageConfirmSaveChangesBeforeClosing, MessageBoxActions.YesNoCancel);
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
                return true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether data has been modified.
        /// </summary>
        public virtual bool Modified
        {
            get { return _modified; }
            protected set
            {
                if (value != _modified)
                {
                    _modified = value;
                    NotifyModifiedChanged();
                }
            }
        }

        /// <summary>
        /// Occurs when all properties may have changed, and the entire view should be updated to reflect
        /// the components data.
        /// </summary>
        public event EventHandler AllPropertiesChanged
        {
            add { _allPropertiesChanged += value; }
            remove { _allPropertiesChanged -= value; }
        }

        /// <summary>
        /// Occurs when the <see cref="Modified"/> property has changed.
        /// </summary>
        public event EventHandler ModifiedChanged
        {
            add { _modifiedChanged += value; }
            remove { _modifiedChanged -= value; }
        }

        /// <summary>
        /// Gets or sets the exit code for the component.
        /// </summary>
        public virtual ApplicationComponentExitCode ExitCode
        {
            get { return _exitCode; }
            protected set { _exitCode = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the component currently has data validation errors.
        /// </summary>
        /// <remarks>
        /// The default implementation checks the <see cref="Validation"/> property to see if the
        /// rule set contains any rules that are not satisfied.  Override this property to implement
        /// custom behaviour.
        /// </remarks>
        public virtual bool HasValidationErrors
        {
            get
            {
                AssertStarted();

                return this.Validation.GetResults(this).FindAll(delegate(ValidationResult r) { return !r.Success; }).Count > 0;
            }
        }

        /// <summary>
        /// Sets the <see cref="ValidationVisible"/> property and raises the <see cref="ValidationVisibleChanged"/> event.
        /// </summary>
        /// <param name="show"></param>
        public virtual void ShowValidation(bool show)
        {
            AssertStarted();

            _showValidationErrors = show;
            EventsHelper.Fire(_validationVisibleChanged, this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets a value indicating whether the view should display validation errors to the user.
        /// </summary>
        public bool ValidationVisible
        {
            get { return _showValidationErrors; }
        }

        /// <summary>
        /// Occurs when the value of the <see cref="ValidationVisible"/> property changes.
        /// </summary>
        public event EventHandler ValidationVisibleChanged
        {
            add { _validationVisibleChanged += value; }
            remove { _validationVisibleChanged -= value; }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }

        #endregion

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                if (_showValidationErrors && _validation != null)
                {
                    ValidationResult result = _validation.GetResults(this, propertyName).Find(
                        delegate(ValidationResult r) { return !r.Success; });

                    return result == null ? null : result.GetMessageString("\n");
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Helper methods

        private void AssertStarted()
        {
            if (!_started)
                throw new InvalidOperationException(SR.ExceptionComponentNeverStarted);
        }

        private void AssertNotStarted()
        {
            if (_started)
                throw new InvalidOperationException(SR.ExceptionComponentAlreadyStarted);
        }

        #endregion
    }
}
