using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using System.Threading;
using System.Security.Principal;
using ClearCanvas.Common.Utilities;
using System.Reflection;

namespace ClearCanvas.Desktop
{
    #region Extension Points

    /// <summary>
    /// Defines an extension point for an implementation of <see cref="IGuiToolkit"/>.
    /// The application requires one extension of this point.
    /// </summary>
    [ExtensionPoint]
    public class GuiToolkitExtensionPoint : ExtensionPoint<IGuiToolkit>
    {
    }

    /// <summary>
    /// Defines an extension point for a view onto the application.  One extension
    /// is required.
    /// </summary>
    [ExtensionPoint]
    public class ApplicationViewExtensionPoint : ExtensionPoint<IApplicationView>
    {
    }

    public interface IApplicationToolContext : IToolContext
    {
    }

    /// <summary>
    /// Defines an extension point of application tools.  Application tools are global
    /// to the application.  An application tool is instantiated exactly once.  Application
    /// tools cannot have actions.
    /// </summary>
    [ExtensionPoint]
    public class ApplicationToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    #endregion

    /// <summary>
    /// Singleton class that represents the desktop application.
    /// </summary>
    /// <remarks>
    /// This class may be subclassed.
    /// </remarks>
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    [AssociateView(typeof(ApplicationViewExtensionPoint))]
    public class Application : IApplicationRoot
    {
        #region Public Static Members

        private static Application _instance;

        /// <summary>
        /// Gets the singleton instance of the <see cref="Application"/> object
        /// </summary>
        public static Application Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets the toolkit ID of the currently loaded GUI <see cref="IGuiToolkit"/>,
        /// or null if the toolkit has not been loaded yet.
        /// </summary>
        public static string GuiToolkitID
        {
            get { return _instance.GuiToolkit != null ? _instance.GuiToolkit.ToolkitID : null; }
        }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        public static string Name
        {
            get { return _instance.ApplicationName; }
        }

        /// <summary>
        /// Gets the version of the application.
        /// </summary>
        public static Version Version
        {
            get { return _instance.ApplicationVersion; }
        }

        /// <summary>
        /// Gets the collection of application windows.
        /// </summary>
        public static DesktopWindowCollection DesktopWindows
        {
            get { return _instance.Windows; }
        }

        /// <summary>
        /// Gets the currently active window.
        /// </summary>
        public static DesktopWindow ActiveDesktopWindow
        {
            get { return DesktopWindows.ActiveWindow; }
        }

        public static DialogBoxAction ShowMessageBox(string message, MessageBoxActions actions)
        {
            return _instance.View.ShowMessageBox(message, actions);
        }

        /// <summary>
        /// Attempts to close all open windows and terminate the application.
        /// </summary>
        /// <returns>True if the application successfully quits, or false if it does not.</returns>
        public static bool Quit()
        {
            return _instance.DoQuit();
        }

        /// <summary>
        /// Occurs before the application is about to quit.
        /// </summary>
        public static event EventHandler<QuittingEventArgs> Quitting
        {
            add { _instance._quitting += value; }
            remove { _instance._quitting -= value; }
        }

        /// <summary>
        /// Occurs before the application is about to quit.
        /// </summary>
        public static event EventHandler Quitted
        {
            add { _instance._quitted += value; }
            remove { _instance._quitted -= value; }
        }


        #endregion

        #region ApplicationToolContext

        class ApplicationToolContext : ToolContext, IApplicationToolContext
        {
            internal ApplicationToolContext(Application application)
            {

            }
        }

        #endregion

        private string _appName;
        private Version _appVersion;
        private IGuiToolkit _guiToolkit;
        private IApplicationView _view;
        private DesktopWindowCollection _windows;
        private ToolSet _toolSet;

        private event EventHandler<QuittingEventArgs> _quitting;
        private event EventHandler _quitted;


        /// <summary>
        /// Default constructor
        /// </summary>
        public Application()
        {
            _instance = this;
        }

        #region IApplicationRoot members

        /// <summary>
        /// Runs the application.
        /// </summary>
        void IApplicationRoot.RunApplication(string[] args)
        {
            try
            {
                Run(args);
            }
            finally
            {
                CleanUp();
            }
        }

        #endregion

        #region Protected overridables

        protected virtual void Run(string[] args)
        {
            _guiToolkit = (IGuiToolkit)(new GuiToolkitExtensionPoint()).CreateExtension();

            // init toolkit before creating any windows
            _guiToolkit.Initialize();

            _view = (IApplicationView)ViewFactory.CreateAssociatedView(this.GetType());

            _windows = CreateDesktopWindowCollection();
            _windows.ItemClosed += delegate(object sender, ClosedItemEventArgs<DesktopWindow> e)
            {
                // terminate the app when the window count goes to 0 if the app isn't already quitting
                if (_windows.Count == 0 && e.Reason != CloseReason.ApplicationQuit)
                {
                    DoQuit();
                }
            };

            // load tools
            _toolSet = new ToolSet(new ApplicationToolExtensionPoint(), new ApplicationToolContext(this));

            // create a root window
            CreateRootWindow();

            // start message pump - this will block until _guiToolkit.QuitMessagePump() is called
            _guiToolkit.RunMessagePump();
        }

        protected virtual DesktopWindow CreateRootWindow()
        {
            return _windows.AddNew("Root");
        }

        protected virtual void CleanUp()
        {
            if (_view != null && _view is IDisposable)
            {
                (_view as IDisposable).Dispose();
                _view = null;
            }

            if (_toolSet != null)
            {
                _toolSet.Dispose();
                _toolSet = null;
            }

            if (_windows != null)
            {
                (_windows as IDisposable).Dispose();
            }

            if (_guiToolkit != null && _guiToolkit is IDisposable)
            {
                (_guiToolkit as IDisposable).Dispose();
            }
        }

        protected virtual void OnQuitting(QuittingEventArgs args)
        {
            EventsHelper.Fire(_quitting, this, args);
        }

        protected virtual void OnQuitted(EventArgs args)
        {
            EventsHelper.Fire(_quitted, this, args);
        }

        protected virtual string GetName()
        {
            return DesktopSettings.Default.ApplicationName;
        }

        protected virtual Version GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
        
        #endregion

        #region Helpers

        internal bool DoQuit()
        {
            QuittingEventArgs args = new QuittingEventArgs(UserInteraction.Allowed, false);
            OnQuitting(args);
            if(args.Cancel)
                return false;

            if (!CloseAllWindows())
                return false;

            OnQuitted(EventArgs.Empty);

            _guiToolkit.QuitMessagePump();

            return true;
        }

        protected bool CloseAllWindows()
        {
            // make a copy of the windows collection for iteration
            List<DesktopWindow> windows = new List<DesktopWindow>(_windows);

            foreach (DesktopWindow window in windows)
            {
                // try to close the window
                bool closed = window.Close(UserInteraction.Allowed, CloseReason.ApplicationQuit);

                // if one fails, abort
                if (!closed)
                    return false;
            }
            return true;
        }

        protected string ApplicationName
        {
            get
            {
                if (_appName == null)
                    _appName = GetName();
                return _appName;
            }
        }

        protected Version ApplicationVersion
        {
            get
            {
                if (_appVersion == null)
                    _appVersion = GetVersion();
                return _appVersion;
            }
        }


        protected DesktopWindowCollection Windows
        {
            get { return _windows; }
        }

        internal IDesktopWindowView OpenWindowView(DesktopWindow window)
        {
            return _view.OpenWindow(window);
        }

        protected IGuiToolkit GuiToolkit
        {
            get { return _guiToolkit; }
        }

        protected IApplicationView View
        {
            get { return _view; }
        }

        private DesktopWindowCollection CreateDesktopWindowCollection()
        {
            return new DesktopWindowCollection(this);
        }

        #endregion
    }
}
