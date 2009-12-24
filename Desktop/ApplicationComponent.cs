#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Desktop.Tools;
using System.Collections.Generic;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines an extension point for tools that operate on application components.
    /// </summary>
    [ExtensionPoint]
    public class ApplicationComponentMetaToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    /// <summary>
    /// Defines the interface for a tool context for tools that extend <see cref="ApplicationComponentMetaToolExtensionPoint"/>.
    /// </summary>
    public interface IApplicationComponentMetaToolContext : IToolContext
    {
        /// <summary>
        /// Gets the desktop window in which the application component is running.
        /// </summary>
        DesktopWindow DesktopWindow { get; }

        /// <summary>
        /// Gets the running application component.
        /// </summary>
        ApplicationComponent Component { get; }
    }


    /// <summary>
    /// Provides a callback when an application component exits.
    /// </summary>
    /// <param name="component">The component that exited.</param>
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

        #region LaunchAsWorkspace overloads

        /// <summary>
        /// Executes the specified application component in a new workspace; the exit callback will be invoked
        /// when the workspace is closed.
        /// </summary>
        /// <remarks>
        /// If the specified component throws an exception from the <see cref="Start"/> method, that exception
        /// will be propagated to the caller of this method and the component will not be launched.
        /// </remarks>
        /// <param name="desktopWindow">The desktop window in which the workspace will run.</param>
        /// <param name="component">The application component to launch.</param>
        /// <param name="title">The title of the workspace.</param>
        /// <param name="exitCallback">An optional callback to invoke when the workspace is closed.</param>
        /// <returns>The workspace that is hosting the component.</returns>
        [Obsolete("This overload has been deprecated.  Use the Workspace.Closed event instead of the exitCallback parameter.")]
        public static Workspace LaunchAsWorkspace(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title,
            ApplicationComponentExitDelegate exitCallback)
        {
            WorkspaceCreationArgs args = new WorkspaceCreationArgs(component, title, null);
            return LaunchAsWorkspace(desktopWindow, args, exitCallback);
        }

        /// <summary>
        /// Executes the specified application component in a new workspace; the exit callback will be invoked
        /// when the workspace is closed.
        /// </summary>
        /// <param name="desktopWindow">The desktop window in which the workspace will run.</param>
        /// <param name="component">The application component to launch.</param>
        /// <param name="title">The display title of the workspace.</param>
        /// <param name="name">The unique name of the workspace.</param>
        /// <param name="exitCallback">An optional callback to invoke when the workspace is closed.</param>
        /// <returns>The workspace that is hosting the component.</returns>
        [Obsolete("This overload has been deprecated.  Use the Workspace.Closed event instead of the exitCallback parameter.")]
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

        	return LaunchAsWorkspace(desktopWindow, args, exitCallback);
        }

		/// <summary>
        /// Executes the specified application component in a new workspace.
		/// </summary>
        /// <remarks>
        /// If the specified component throws an exception from the <see cref="Start"/> method, that exception
        /// will be propagate to the caller of this method and the component will not be launched.
        /// </remarks>
		/// <param name="desktopWindow">The desktop window in which the workspace will run.</param>
        /// <param name="component">The application component to launch.</param>
        /// <param name="title">The title of the workspace.</param>
        /// <param name="name">A name that will be assigned to the workspace.</param>
		/// <returns>The workspace that is hosting the component.</returns>
		public static Workspace LaunchAsWorkspace(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title,
            string name)
        {
            Platform.CheckForNullReference(desktopWindow, "desktopWindow");
            Platform.CheckForNullReference(component, "component");

            WorkspaceCreationArgs args = new WorkspaceCreationArgs(component, title, name);

            return LaunchAsWorkspace(desktopWindow, args, null);
        }

        /// <summary>
        /// Executes the specified application component in a new workspace.
        /// </summary>
        /// <remarks>
        /// If the specified component throws an exception from the <see cref="Start"/> method, that exception
        /// will be propagated to the caller of this method and the component will not be launched.
        /// </remarks>
        /// <param name="desktopWindow">The desktop window in which the workspace will run.</param>
        /// <param name="component">The application component to launch.</param>
        /// <param name="title">The title of the workspace.</param>
        /// <returns>The workspace that is hosting the component.</returns>
        public static Workspace LaunchAsWorkspace(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title)
        {
            Platform.CheckForNullReference(desktopWindow, "desktopWindow");
            Platform.CheckForNullReference(component, "component");

            WorkspaceCreationArgs args = new WorkspaceCreationArgs(component, title, null);

            return LaunchAsWorkspace(desktopWindow, args, null);
        }


        /// <summary>
        /// Executes the specified application component in a new workspace.
        /// </summary>
        /// <remarks>
        /// If the specified component throws an exception from the <see cref="Start"/> method, that exception
        /// will be propagate to the caller of this method and the component will not be launched.
        /// </remarks>
        /// <param name="desktopWindow">The desktop window in which the workspace will run.</param>
        /// <param name="creationArgs">A <see cref="WorkspaceCreationArgs"/> object.</param>
        /// <returns>The workspace that is hosting the component.</returns>
        public static Workspace LaunchAsWorkspace(
            IDesktopWindow desktopWindow,
            WorkspaceCreationArgs creationArgs)
        {
            Platform.CheckForNullReference(desktopWindow, "desktopWindow");
            Platform.CheckForNullReference(creationArgs, "creationArgs");

            return LaunchAsWorkspace(desktopWindow, creationArgs, null);
        }
        
        /// <summary>
        /// Private helper method to support LaunchAsWorkspace.
		/// </summary>
		private static Workspace LaunchAsWorkspace(
    		IDesktopWindow desktopWindow, 
			WorkspaceCreationArgs creationArgs,
    		ApplicationComponentExitDelegate exitCallback)
		{
			Platform.CheckForNullReference(desktopWindow, "desktopWindow");
			Platform.CheckForNullReference(creationArgs, "creationArgs");

			Workspace workspace = desktopWindow.Workspaces.AddNew(creationArgs);
			if (exitCallback != null)
			{
				workspace.Closed += delegate(object sender, ClosedEventArgs e)
				{
					exitCallback(creationArgs.Component);
				};
			}
			return workspace;
		}

        #endregion

        #region LaunchAsShelf overloads

        /// <summary>
        /// Executes the specified application component in a new shelf; the exit callback will be invoked
        /// when the shelf is closed.
        /// </summary>
        /// <remarks>
        /// If the specified component throws an exception from its <see cref="Start"/> method, that exception
        /// will be propagated to the caller of this method and the component will not be launched.
        /// </remarks>
        /// <param name="desktopWindow">The desktop window in which the shelf will run.</param>
        /// <param name="component">The application component to launch.</param>
        /// <param name="title">The title of the shelf.</param>
        /// <param name="displayHint">A hint as to how the shelf should initially be displayed.</param>
        /// <param name="exitCallback">The callback to invoke when the shelf is closed.</param>
        /// <returns>The shelf that is hosting the component.</returns>
        [Obsolete("This overload has been deprecated.  Use the Shelf.Closed event instead of the exitCallback parameter.")]
        public static Shelf LaunchAsShelf(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title,
            ShelfDisplayHint displayHint,
            ApplicationComponentExitDelegate exitCallback)
        {
            ShelfCreationArgs args = new ShelfCreationArgs(component, title, null, displayHint);
            return LaunchAsShelf(desktopWindow, args, exitCallback);
        }

        /// <summary>
        /// Executes the specified application component in a new shelf; the exit callback will be invoked
        /// when the shelf is closed.
        /// </summary>
        /// <remarks>
        /// If the specified component throws an exception from its <see cref="Start"/> method, that exception
        /// will be propagated to the caller of this method and the component will not be launched.
        /// </remarks>
        /// <param name="desktopWindow">The desktop window in which the shelf will run.</param>
        /// <param name="component">The application component to launch.</param>
        /// <param name="title">The title of the shelf.</param>
        /// <param name="name">The unique name shelf.</param>
        /// <param name="displayHint">A hint as to how the shelf should initially be displayed.</param>
        /// <param name="exitCallback">The callback to invoke when the shelf is closed.</param>
        /// <returns>The shelf that is hosting the component.</returns>
        [Obsolete("This overload has been deprecated.  Use the Shelf.Closed event instead of the exitCallback parameter.")]
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
            return LaunchAsShelf(desktopWindow, args, exitCallback);
        }

        /// <summary>
        /// Executes the specified application component in a new shelf.
        /// </summary>
        /// <remarks>
        /// If the specified component throws an exception from its <see cref="Start"/> method, that exception
        /// will be propagate to the caller of this method and the component will not be launched.
        /// </remarks>
        /// <param name="desktopWindow">The desktop window in which the shelf will run.</param>
        /// <param name="component">The application component to launch.</param>
        /// <param name="title">The title of the shelf.</param>
        /// <param name="name">The unique name of the shelf.</param>
        /// <param name="displayHint">A hint as to how the shelf should initially be displayed.</param>
        /// <returns>The shelf that is hosting the component.</returns>
        public static Shelf LaunchAsShelf(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title,
            string name,
            ShelfDisplayHint displayHint)
        {
            Platform.CheckForNullReference(desktopWindow, "desktopWindow");
            Platform.CheckForNullReference(component, "component");

            ShelfCreationArgs args = new ShelfCreationArgs(component, title, name, displayHint);
            return LaunchAsShelf(desktopWindow, args, null);
        }

        /// <summary>
        /// Executes the specified application component in a new shelf.
        /// </summary>
        /// <remarks>
        /// If the specified component throws an exception from its <see cref="Start"/> method, that exception
        /// will be propagate to the caller of this method and the component will not be launched.
        /// </remarks>
        /// <param name="desktopWindow">The desktop window in which the shelf will run.</param>
        /// <param name="component">The application component to launch.</param>
        /// <param name="title">The title of the shelf.</param>
        /// <param name="displayHint">A hint as to how the shelf should initially be displayed.</param>
        /// <returns>The shelf that is hosting the component.</returns>
        public static Shelf LaunchAsShelf(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title,
            ShelfDisplayHint displayHint)
        {
            ShelfCreationArgs args = new ShelfCreationArgs(component, title, null, displayHint);
            return LaunchAsShelf(desktopWindow, args, null);
        }

        /// <summary>
        /// Executes the specified application component in a new shelf.
        /// </summary>
        /// <remarks>
        /// If the specified component throws an exception from its <see cref="Start"/> method, that exception
        /// will be propagate to the caller of this method and the component will not be launched.
        /// </remarks>
        /// <param name="desktopWindow">The desktop window in which the shelf will run.</param>
        /// <param name="creationArgs">A <see cref="ShelfCreationArgs"/> object.</param>
        /// <returns>The shelf that is hosting the component.</returns>
        public static Shelf LaunchAsShelf(
            IDesktopWindow desktopWindow,
            ShelfCreationArgs creationArgs)
        {
            Platform.CheckForNullReference(desktopWindow, "desktopWindow");
            Platform.CheckForNullReference(creationArgs, "creationArgs");

            return LaunchAsShelf(desktopWindow, creationArgs, null);
        }

        /// <summary>
        /// Private helper method to support LaunchAsShelf
        /// </summary>
        private static Shelf LaunchAsShelf(
            IDesktopWindow desktopWindow,
            ShelfCreationArgs args,
            ApplicationComponentExitDelegate exitCallback)
        {
            Shelf shelf = desktopWindow.Shelves.AddNew(args);
            if (exitCallback != null)
            {
                shelf.Closed += delegate
                {
                    exitCallback(args.Component);
                };
            }
            return shelf;
        }

        #endregion

        #region LaunchAsDialog overloads

        /// <summary>
        /// Executes the specified application component in a modal dialog box; this call will block until
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
            DialogBoxCreationArgs args = new DialogBoxCreationArgs(component, title, null);
            return LaunchAsDialog(desktopWindow, args);
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
        /// <param name="creationArgs">A <see cref="DialogBoxCreationArgs"/> object.</param>
        /// <returns>The exit code that the component exits with.</returns>
        public static ApplicationComponentExitCode LaunchAsDialog(
            IDesktopWindow desktopWindow,
            DialogBoxCreationArgs creationArgs)
        {
            desktopWindow.ShowDialogBox(creationArgs);
            return creationArgs.Component.ExitCode;
        }

        #endregion

        #region ApplicationComponentMetaToolContext

        class ApplicationComponentMetaToolContext : IApplicationComponentMetaToolContext
        {
            private readonly ApplicationComponent _component;
            private readonly DesktopWindow _window;

            internal ApplicationComponentMetaToolContext(ApplicationComponent component, DesktopWindow window)
            {
                _component = component;
                _window = window;
            }

            public DesktopWindow DesktopWindow
            {
                get { return _window; }
            }

            public ApplicationComponent Component
            {
                get { return _component; }
            }
        }

        #endregion


        private IApplicationComponentHost _host;
        private ApplicationComponentExitCode _exitCode;

        private bool _isStarted;
    	private event EventHandler _started;
		private event EventHandler _stopped;

        private bool _modified;
        private event EventHandler _modifiedChanged;

        private event EventHandler _allPropertiesChanged;
        private event PropertyChangedEventHandler _propertyChanged;

        private ValidationRuleSet _validation;
        private bool _showValidationErrors;
        private event EventHandler _validationVisibleChanged;

        private IResourceResolver _resourceResolver;

        private ToolSet _toolSet;


        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ApplicationComponent()
        {
            _exitCode = ApplicationComponentExitCode.None;    // default exit code

            // default resource resolver
            _resourceResolver = new ResourceResolver(this.GetType().Assembly);

			// create default validation rule set containing rules for this type
			_validation = new ValidationRuleSet(ValidationCache.GetRules(this.GetType()));
		}


        /// <summary>
        /// Gets or sets the <see cref="ValidationRuleSet"/> that is associated with this component.
        /// </summary>
        public ValidationRuleSet Validation
        {
            get { return _validation; }
            set { _validation = value; }
        }

        /// <summary>
        /// Gets the meta context-menu model. The menu is displayed when clicking on the background of an application component.
        /// </summary>
        public virtual ActionModelNode MetaContextMenuModel
        {
            get
            {
                return ActionModelRoot.CreateModel(typeof(ApplicationComponent).FullName, "applicationcomponent-metacontextmenu", _toolSet.Actions);
            }
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
		/// </summary>
		/// <remarks>
        /// Note that it is not necessary to explicitly call this method if the 
        /// default implementation of the <see cref="Modified"/> property is used,
        /// since the event is fired automatically when the property is set.
        /// This method is provided for situations where the subclass has chosen
        /// to override the <see cref="Modified"/> property.
        /// </remarks>
        protected void NotifyModifiedChanged()
        {
            EventsHelper.Fire(_modifiedChanged, this, EventArgs.Empty);
        }

        /// <summary>
        /// Notifies subscribers of the <see cref="PropertyChanged"/> event that the specified property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Notifies subscribers of the <see cref="AllPropertiesChanged"/> that all properties may have changed.
        /// </summary>
        /// <remarks>
		/// A view should respond to this event by refreshing itself completely.
		/// </remarks>
        protected void NotifyAllPropertiesChanged()
        {
            EventsHelper.Fire(_allPropertiesChanged, this, EventArgs.Empty);
        }

        #endregion

        #region IApplicationComponent Members

        /// <summary>
        /// Called by the framework to set the host.
        /// </summary>
        /// <remarks>
		/// For internal framework use only.
		/// </remarks>
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
            get
            {
                return new ActionSet();
            }
        }

        /// <summary>
        /// Called by the host to initialize the application component.
        /// </summary>
        ///  <remarks>
		/// Override this method to implement custom initialization logic.  Overrides must be sure to call the base implementation.
        /// </remarks>
        public virtual void Start()
        {
            AssertNotStarted();

			// create meta-tools
			_toolSet = new ToolSet(new ApplicationComponentMetaToolExtensionPoint(),
                new ApplicationComponentMetaToolContext(this, _host.DesktopWindow));

            _isStarted = true;

			EventsHelper.Fire(_started, this, EventArgs.Empty);
        }

    	/// <summary>
    	/// Occurs after the component has started.
    	/// </summary>
    	public event EventHandler Started
		{
			add { _started += value; }
			remove { _started -= value; }
		}

    	/// <summary>
        /// Called by the host when the application component is being terminated.
        /// </summary>
        /// <remarks>
		/// Override this method to implement custom termination logic.  Overrides must be sure to call the base implementation.
        /// </remarks>
        public virtual void Stop()
        {
            AssertStarted();

            if (_toolSet != null)
            {
                _toolSet.Dispose();
                _toolSet = null;
            }

            _isStarted = false;

			EventsHelper.Fire(_stopped, this, EventArgs.Empty);
		}

    	/// <summary>
    	/// Occurs after the component has stopped.
    	/// </summary>
    	public event EventHandler Stopped
		{
			add { _stopped += value; }
			remove { _stopped -= value; }
		}

		/// <summary>
        /// Gets a value indicating whether this component is live or not. 
        /// </summary>
        public bool IsStarted
        {
            get { return _isStarted; }
        }

        /// <summary>
        /// Called by the framework to determine if this component is in a state
        /// such that it can be stopped without user interaction.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The behaviour of the default implementation depends upon the type of host.  If the component is running
        /// in an <see cref="IShelfHost"/> or an <see cref="IDialogBoxHost"/>, this method always returns true.
        /// Otherwise, it checks the <see cref="Modified"/> property and returns
        /// false if data has been modified.
        /// </para>
        /// <para>
		/// Override this method to provide custom logic for responding to this query.
		/// </para>
        /// </remarks>
        public virtual bool CanExit()
        {
            AssertStarted();

            if(this.Host is IShelfHost || this.Host is IDialogBoxHost)
                return true;

            return !this.Modified;
        }

        /// <summary>
        /// Called by the framework in the case where the host has initiated the exit, rather than the component,
        /// to give the component a chance to prepare prior to being stopped.
        /// </summary>
        /// <remarks>
        /// The behaviour of the default implementation depends upon the type of host.  If the component is running
        /// in a <see cref="IShelfHost"/> or an <see cref="IDialogBoxHost"/>, this method always returns true.
        /// Otherwise, it checks the <see cref="Modified"/> property to see if data has been modified.
        /// If data has been modified, a standard confirmation dialog is presented, asking the user whether the changes
        /// should be discarded, or the exit cancelled.
        /// </remarks>
        /// <returns>
        /// True if there are no modifications or the user elects to discard modifications, otherwise false.
        /// </returns>
        public virtual bool PrepareExit()
        {
            AssertStarted();

            if (this.Host is IShelfHost || this.Host is IDialogBoxHost)
            {
                // nothing to do
                return true;
            }

            // if modified, check if the user intended to discard the changes
            if (this.Modified)
            {
                if(this.Host.ShowMessageBox(SR.MessageConfirmDiscardChangesBeforeClosing, MessageBoxActions.OkCancel) == DialogBoxAction.Cancel)
                {
                    // user has cancelled the "close" operation, therefore we can't exit
                    return false;
                }
            }

            // data was not modified, or the user has chosen to discard the changes
            return true;
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
        /// the component's data.
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
            set { _exitCode = value; }
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
        public virtual void ShowValidation(bool show)
        {
            _showValidationErrors = show;
            if (_isStarted)
            {
                EventsHelper.Fire(_validationVisibleChanged, this, EventArgs.Empty);
            }
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

		/// <summary>
		/// Notifies subscribers that one of the component's properties has changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }

        #endregion

        #region IDataErrorInfo Members

		/// <summary>
		/// Not implemented.
		/// </summary>
        string IDataErrorInfo.Error
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

		/// <summary>
		/// Gets the error message, taken from the results of <see cref="Validation"/>, that arose due to the
		/// value of the input <paramref name="propertyName"/>.
		/// </summary>
		/// <param name="propertyName">The name of the property to check for errors.</param>
		/// <returns>A string representation of the error, or null if there isn't one.</returns>
        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                if (_showValidationErrors && _validation != null)
                {
                    ValidationResult result = ValidationResult.Combine(_validation.GetResults(this, propertyName));
                    return result.Success ? null : result.GetMessageString("\n");
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
            if (!_isStarted)
                throw new InvalidOperationException(SR.ExceptionComponentNeverStarted);
        }

        private void AssertNotStarted()
        {
            if (_isStarted)
                throw new InvalidOperationException(SR.ExceptionComponentAlreadyStarted);
        }

        #endregion
    }
}
