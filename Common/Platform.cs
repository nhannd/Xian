using System;
using System.IO;
using System.Reflection;
using System.Resources;
using log4net;
//using log4net.spi;
using log4net.Config;

using ClearCanvas.Common.Auditing;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

// Configure log4net using the .log4net file
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Logging.config", Watch = true)]
// This will cause log4net to look for a configuration file
// called TestApp.exe.log4net in the application base
// directory (i.e. the directory containing TestApp.exe)
// The config file will be watched for changes.

namespace ClearCanvas.Common
{
	public enum LogLevel
	{
		Debug,
		Info,
		Warn,
		Error,
		Fatal
	}


    /// <summary>
    /// 
    /// </summary>
    [ExtensionPoint()]
    public class MessageBoxExtensionPoint : ExtensionPoint<IMessageBox>
    {
    }

    /// <summary>
    /// Defines the Application Root extension point.  When <see cref="Platform.StartApp" /> is called,
    /// the platform creates an application root extension and executes it by calling
    /// <see cref="IApplicationRoot.RunApplication" />.
    /// </summary>
    [ExtensionPoint()]
    public class ApplicationRootExtensionPoint : ExtensionPoint<IApplicationRoot>
    {
    }

    /// <summary>
    /// Defines a general mechanism for establishing a session.  When <see cref="Platform.StartApp"/>
    /// is called, the framework will look for a session manager extension.  If one is found, the
    /// framework will attempt to establish a session by calling <see cref="ISessionManager.InitiateSession"/>.
    /// If no session manager extensions are found, the application will proceed to execute in standalone
    /// (i.e no authentication) mode.
    /// </summary>
    [ExtensionPoint()]
    public class SessionManagerExtensionPoint : ExtensionPoint<ISessionManager>
    {
    }

    [ExtensionPoint()]
    public class TimeProviderExtensionPoint : ExtensionPoint<ITimeProvider>
    {
    }

    /// <summary>
    /// Defines an extension point for service providers.  A service provider is a class that knows how
    /// to provide a specific set of services to the application.  A given service should be provided
    /// exclusively by one provider (i.e. no two providers should provide the same service).  The application obtains
    /// services through the <see cref="Platform.GetService"/> method.
    /// </summary>
    /// <remarks>
    /// A service provider may be accessed by multiple threads.  For reasons of thread-safety, a service provider
    /// should return a new instance of the service class for each call to <see cref="IServiceProvider.GetService"/>,
    /// so that each thread receives its own copy of the service.
    /// If the provider returns the same object (singleton), then the service object itself must be thread-safe.
    /// </remarks>
    [ExtensionPoint]
    public class ServiceProviderExtensionPoint : ExtensionPoint<IServiceProvider>
    {
    }

	/// <summary>
	/// A collection of useful utility functions.
	/// </summary>
	public static class Platform
	{
        private static object _syncRoot = new Object();

        private static string _installDirectory = null;
        private static string _pluginsDirectory = null;
        private static string _logDirectory = null;

		private static string _pluginSubFolder = "plugins";
		private static string _logSubFolder = "logs";
		private static volatile PluginManager _pluginManager;
		private static readonly ILog _log = LogManager.GetLogger(typeof(Platform));
        private static IApplicationRoot _applicationRoot;
		private static IMessageBox _messageBox;
		private static AuditManager _auditManager;
        private static ITimeProvider _timeProvider;
        private static IServiceProvider[] _serviceProviders;

		/// <summary>
		/// Gets the one and only <see cref="PluginManager"/>.
		/// </summary>
		/// <value>The <see cref="PluginManager"/>.</value>
		public static PluginManager PluginManager
		{
			get
			{
				if (_pluginManager == null)
				{
					lock (_syncRoot)
					{
						if (_pluginManager == null)
							_pluginManager = new PluginManager(PluginDirectory);
					}
				}

				return _pluginManager;
			}
		}

        /// <summary>
        /// Gets whether the application is executing on a Win32 operating system
        /// </summary>
        /// <remarks>
        /// This method is thread-safe.
        /// </remarks>
		public static bool IsWin32Platform
		{
			get
			{
				PlatformID id = Environment.OSVersion.Platform;
				return (id == PlatformID.Win32NT || id == PlatformID.Win32Windows || id == PlatformID.Win32S || id == PlatformID.WinCE);
			}
		}

        /// <summary>
        /// Gets whether the application is executing on a Unix operating systems
        /// </summary>
        /// <remarks>
        /// This method is thread-safe.
        /// </remarks>
        public static bool IsUnixPlatform
		{
			get
			{
				PlatformID id = Environment.OSVersion.Platform;
				return (id == PlatformID.Unix);
			}
		}

        /// <summary>
        /// Gets the file-system path separator character for the current operating system
        /// </summary>
        /// <remarks>
        /// This method is thread-safe.
        /// </remarks>
        public static char PathSeparator
        {
            get { return IsWin32Platform ? '\\' : '/'; }
        }
		
		/// <summary>
		/// Gets the ClearCanvas installation directory.
		/// </summary>
        /// <remarks>
        /// This method is thread-safe.
        /// </remarks>
        public static string InstallDirectory
		{
			get
			{
                if (_installDirectory == null)
                {
                    lock (_syncRoot)
                    {
						if (_installDirectory == null)
							_installDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    }
                }

				return _installDirectory;
			}
		}

		/// <summary>
		/// Gets the fully qualified plugin directory.
		/// </summary>
        /// <remarks>
        /// This method is thread-safe.
        /// </remarks>
        public static string PluginDirectory
		{
			get
			{
                if (_pluginsDirectory == null)
                {
                    lock (_syncRoot)
                    {
                        if (_pluginsDirectory == null)
                            _pluginsDirectory = string.Format("{0}{1}{2}", Platform.InstallDirectory, Platform.PathSeparator, _pluginSubFolder);
                    }
                }

                return _pluginsDirectory;
			}
		}


		/// <summary>
		/// Gets the fully qualified log directory.
		/// </summary>
        /// <remarks>
        /// This method is thread-safe.
        /// </remarks>
        public static string LogDirectory
		{
			get
			{
                if (_logDirectory == null)
                {
                    lock (_syncRoot)
                    {
                        if (_logDirectory == null)
                            _logDirectory = string.Format("{0}{1}{2}", Platform.InstallDirectory, Platform.PathSeparator, _logSubFolder);
                    }
                }

                return _logDirectory;
			}
		}

		public static AuditManager AuditManager
		{
			get
			{
				if (_auditManager == null)
				{
					lock (_syncRoot)
					{
						if (_auditManager == null)
							_auditManager = new AuditManager();
					}
				}

				return _auditManager;
			}
		}

        /// <summary>
        /// Gets the current time from an extension of <see cref="TimeProviderExtensionPoint"/>, if one exists.
        /// </summary>
        /// <remarks>
        /// The time returned may differ from the current time on the local machine, because the provider may choose
        /// to obtain the time from another source (i.e. a server).
        /// This method is thread-safe.
        /// </remarks>
        public static DateTime Time
        {
            get
            {
                if (_timeProvider == null)
                {
                    lock (_syncRoot)
                    {
                        if (_timeProvider == null)
                        {
                            try
                            {
                                // check for a time provider extension
                                TimeProviderExtensionPoint xp = new TimeProviderExtensionPoint();
                                _timeProvider = (ITimeProvider)xp.CreateExtension();
                            }
                            catch (NotSupportedException)
                            {
                                // can't find time provider, default to local time
                                Log(SR.LogTimeProviderNotFound, LogLevel.Warn);

                                _timeProvider = new LocalTimeProvider();
                            }
                        }
                    }
                }

                // need to lock here, as the time provider itself may not be thread-safe
                // note: lock on _timeProvider rather than _syncRoot, so _syncRoot remains free for other methods
                lock (_timeProvider)
                {
                    return _timeProvider.CurrentTime;
                }
            }
        }

        /// <summary>
        /// Starts the application.
        /// </summary>
        /// <param name="applicationRootFilter">An extension filter that selects the application root extension to execute</param>
        /// <param name="args">The set of arguments passed from the command line</param>
        /// <remarks>
        /// A ClearCanvas based application is started by calling this convenience method from
        /// a bootstrap executable of some kind.  Calling this method results in the loading
        /// of all plugins and creation of an IApplicationRoot extension.  This method is not thread-safe as it should only
        /// ever be invoked once per execution, by a single thread.
        /// </remarks>
        public static void StartApp(ExtensionFilter applicationRootFilter, string[] args)
        {
#if !DEBUG
            try
            {
#endif
            ISessionManager sessionManager = GetSessionManager();
            if (sessionManager != null)
            {
                // allow any exception thrown here to cause the application to terminate
                if (!sessionManager.InitiateSession())
                    return;     // silent exit
            }

            try
            {
                ApplicationRootExtensionPoint xp = new ApplicationRootExtensionPoint();
                _applicationRoot = (applicationRootFilter == null) ?
                    (IApplicationRoot)xp.CreateExtension() :
                    (IApplicationRoot)xp.CreateExtension(applicationRootFilter);
                _applicationRoot.RunApplication(args);
            }
            finally
            {
                if (sessionManager != null)
                    sessionManager.TerminateSession();
            }



#if !DEBUG
            }
            catch (Exception e)
            {
				Platform.Log(e, LogLevel.Fatal);
            }
#endif
        }

		// Public methods

		/// <summary>
		/// Starts the application.
		/// </summary>
		/// <remarks>
		/// A ClearCanvas based application is started by calling this convenience method from
		/// a bootstrap executable of some kind.  Calling this method results in the loading
        /// of all plugins and creation of an IApplicationRoot extension.  This method is not thread-safe as it should only
        /// ever be invoked once per execution, by a single thread.
		/// </remarks>
		public static void StartApp()
		{
            StartApp(null, new string[] { });
		}

        /// <summary>
        /// Obtains an instance of the specified service for use by the application.
        /// </summary>
        /// <remarks>
        /// This method is thread-safe.
        /// </remarks>
        /// <typeparam name="TService">The type of service to obtain</typeparam>
        /// <returns>An instance of the specified service</returns>
        /// <exception cref="UnknownServiceException">The requested service cannot be provided</exception>
        public static TService GetService<TService>()
        {
            return (TService)GetService(typeof(TService));
        }

        public delegate void WithServiceDelegate<T>(T service);

        /// <summary>
        /// Obtains an instance of the specified service for use by the application.  Instead of returning the
        /// service directly, this overload passes the service to the specified delegate for use.  When the delegate returns,
        /// this method automatically takes care of determing whether the service implements <see cref="IDisposable"/>
        /// and calling <see cref="IDisposable.Dispose"/> if it does.  The delegate must not cache the returned service
        /// because it may be disposed as soon as the delegate returns.  For the single-use scenario, this overload is preferred
        /// to the other overloads because it automatically manages the lifecycle of the service object.
        /// </summary>
        /// <remarks>
        /// This method is thread-safe.
        /// </remarks>
        /// <typeparam name="TService">The service to obtain</typeparam>
        /// <param name="proc">A delegate that will receive the service for one-time use</param>
        public static void GetService<TService>(WithServiceDelegate<TService> proc)
        {
            TService service = Platform.GetService<TService>();

            try
            {
                proc(service);
            }
            finally
            {
                if (service is IDisposable)
                {
                    try
                    {
                        (service as IDisposable).Dispose();
                    }
                    catch (Exception e)
                    {
                        // do not allow exceptions thrown from Dispose() because it may have the effect of
                        // hiding an exception that was thrown from the service itself
                        // if the service fails to dispose properly, we don't care, just log it and move on
                        Platform.Log(e);
                    }
                }
            }
        }

        /// <summary>
        /// Obtains an instance of the specified service for use by the application.
        /// </summary>
        /// <remarks>
        /// This method is thread-safe.
        /// </remarks>
        /// <param name="service">The type of service to obtain</param>
        /// <returns>An instance of the specified service</returns>
        /// <exception cref="UnknownServiceException">The requested service cannot be provided</exception>
        public static object GetService(Type service)
        {
            // load all service providers if not yet loaded
            if (_serviceProviders == null)
            {
                lock (_syncRoot)
                {
                    if (_serviceProviders == null)
                    {
                        _serviceProviders = Array.ConvertAll<object, IServiceProvider>(
                                                (new ServiceProviderExtensionPoint()).CreateExtensions(),
                                                    delegate(object sp) { return (IServiceProvider)sp; });
                    }
                }
            }

            // attempt to instantiate the requested service
            foreach (IServiceProvider sp in _serviceProviders)
            {
                // the service provider itself may not be thread-safe, so we need to ensure only one thread will access it
                // at a time
                lock (sp)
                {
                    object impl = sp.GetService(service);
                    if (impl != null)
                        return impl;
                }
            }
            throw new UnknownServiceException(string.Format(SR.ExceptionNoServiceProviderCanProvide, service.FullName));
        }

		/// <summary>
        /// Logs the specified message at <see cref="LogLevel.Info"/>.
        /// </summary>
		/// <param name="message"></param>
		public static void Log(object message)
		{
			Log(message, LogLevel.Info);
		}

        /// <summary>
        /// Logs the specified exception at <see cref="LogLevel.Error"/>.
        /// </summary>
        /// <remarks>This method is thread-safe.</remarks>
        /// <param name="ex"></param>
        public static void Log(Exception ex)
        {
            Log(ex, LogLevel.Error);
        }

        /// <summary>
        /// Logs the specified message at the specified <see cref="LogLevel"/>.
        /// </summary>
        /// <remarks>This method is thread-safe.</remarks>
        /// <param name="message"></param>
        /// <param name="category"></param>
		public static void Log(object message, LogLevel category)
		{
			switch (category)
			{
				case LogLevel.Debug:
					_log.Debug(message);
					break;
				case LogLevel.Info:
					_log.Info(message);
					break;
				case LogLevel.Warn:
					_log.Warn(message);
					break;
				case LogLevel.Error:
					_log.Error(message);
					break;
				case LogLevel.Fatal:
					_log.Fatal(message);
					break;
			}
		}

        /// <summary>
        /// Logs the specified exception at the specified <see cref="LogLevel"/>.
        /// </summary>
        /// <remarks>This method is thread-safe.</remarks>
        /// <param name="ex"></param>
        /// <param name="category"></param>
		public static void Log(Exception ex, LogLevel category)
		{
			switch (category)
			{
				case LogLevel.Debug:
					_log.Debug(SR.ExceptionThrown, ex);
					break;
				case LogLevel.Info:
					_log.Info(SR.ExceptionThrown, ex);
					break;
				case LogLevel.Warn:
					_log.Warn(SR.ExceptionThrown, ex);
					break;
				case LogLevel.Error:
					_log.Error(SR.ExceptionThrown, ex);
					break;
				case LogLevel.Fatal:
					_log.Fatal(SR.ExceptionThrown, ex);
					break;
			}
		}

        /// <summary>
        /// Displays a message box with the specified message.
        /// </summary>
        /// <remarks>
        /// This method is thread-safe, however displaying message boxes from a thread other than the UI
        /// thread is not a recommended practice.
        /// </remarks>
        [Obsolete("Use DesktopWindow.ShowMessageBox instead", false)]
        public static void ShowMessageBox(string message)
		{
            ShowMessageBox(message, MessageBoxActions.Ok);
		}

        /// <summary>
        /// Displays a message box with the specified message and buttons, and returns a value indicating the action
        /// taken by the user.
        /// </summary>
        /// <remarks>
        /// This method is thread-safe, however displaying message boxes from a thread other than the UI
        /// thread is not a recommended practice.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        [Obsolete("Use DesktopWindow.ShowMessageBox instead", false)]
        public static DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons)
        {
            // create message box if does not exist
            if (_messageBox == null)
            {
                lock (_syncRoot)
                {
                    if (_messageBox == null)
                    {
                        MessageBoxExtensionPoint xp = new MessageBoxExtensionPoint();
                        _messageBox = (IMessageBox)xp.CreateExtension();
                    }
                }
            }

            // must lock here, because we have no guarantee that the underlying _messageBox object is thread-safe
            // lock on the _messageBox itself, rather than _syncRoot, so that _syncRoot is free for other threads to lock on
            // (i.e the message box may block this thread for a long time, and all other threads would halt if we locked on _syncRoot)
            lock (_messageBox)
            {
                return _messageBox.Show(message, buttons);
            }
        }


		/// <summary>
		/// Checks if a string is empty.
		/// </summary>
		/// <param name="variable">The string to check.</param>
		/// <param name="variableName">The variable name of the string to checked.</param>
		/// <exception cref="ArgumentNullException"><paramref name="variable"/> or or <paramref name="variableName"/>
		/// is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="variable"/> is zero length.</exception>
		public static void CheckForEmptyString(string variable, string variableName)
		{
			CheckForNullReference(variable, variableName);
			CheckForNullReference(variableName, "variableName");

			if (variable.Length == 0)
				throw new ArgumentException(String.Format(SR.ExceptionEmptyString, variableName));
		}

		/// <summary>
		/// Checks if an object reference is null.
		/// </summary>
		/// <param name="variable">The object reference to check.</param>
		/// <param name="variableName">The variable name of the object reference to check.</param>
		/// <remarks>Use for checking if an input argument is <b>null</b>.  To check if a member variable
		/// is <b>null</b> (i.e., to see if an object is in a valid state), use <see cref="CheckMemberIsSet"/> instead.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="variable"/> or <paramref name="variableName"/>
		/// is <b>null</b>.</exception>
		public static void CheckForNullReference(object variable, string variableName)
		{
			if (variableName == null)
				throw new ArgumentNullException("variableName");

			if (null == variable)
				throw new ArgumentNullException(variableName);
		}

		/// <summary>
		/// Checks if an object is of the expected type.
		/// </summary>
		/// <param name="variable">The object to check.</param>
		/// <param name="type">The variable name of the object to check.</param>
		/// <exception cref="ArgumentNullException"><paramref name="variable"/> or <paramref name="type"/>
		/// is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="type"/> is not the expected type.</exception>
		public static void CheckExpectedType(object variable, Type type)
		{
			CheckForNullReference(variable, "variable");
			CheckForNullReference(type, "type");

			if (!type.IsAssignableFrom(variable.GetType()))
				throw new ArgumentException(String.Format(SR.ExceptionExpectedType, type.FullName));
		}

		/// <summary>
		/// Checks if a cast is valid.
		/// </summary>
		/// <param name="castOutput">The object resulting from the cast.</param>
		/// <param name="castInputName">The variable name of the object that was cast.</param>
		/// <param name="castTypeName">The name of the type the object was cast to.</param>
		/// <remarks>To use this method, casts have to be done using the <b>as</b> operator.  The
		/// method depends on failed casts resulting in <b>null</b>.</remarks>
		/// <example>
		/// <code>
		/// [C#]
		/// layer = new GraphicLayer();
		/// GraphicLayer graphicLayer = layer as GraphicLayer;
		/// // No exception thrown
		/// Platform.CheckForInvalidCast(graphicLayer, "layer", "GraphicLayer");
		///
		/// ImageLayer image = layer as ImageLayer;
		/// // InvalidCastException thrown
		/// Platform.CheckForInvalidCast(image, "layer", "ImageLayer");
		/// </code>
		/// </example>
		/// <exception cref="ArgumentNullException"><paramref name="castOutput"/>,
		/// <paramref name="castInputName"/>, <paramref name="castTypeName"/> is <b>null</b></exception>
		/// <exception cref="InvalidCastException">Cast is invalid.</exception>
		public static void CheckForInvalidCast(object castOutput, string castInputName, string castTypeName)
		{
			Platform.CheckForNullReference(castOutput, "castOutput");
			Platform.CheckForNullReference(castInputName, "castInputName");
			Platform.CheckForNullReference(castTypeName, "castTypeName");

			if (castOutput == null)
				throw new InvalidCastException(String.Format(SR.ExceptionInvalidCast, castInputName, castTypeName));
		}

		/// <summary>
		/// Checks if a value is positive.
		/// </summary>
		/// <param name="n">The value to check.</param>
		/// <param name="variableName">The variable name of the value to check.</param>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="n"/> &lt;= 0.</exception>
		public static void CheckPositive(int n, string variableName)
		{
			Platform.CheckForNullReference(variableName, "variableName");

			if (n <= 0)
				throw new ArgumentException(SR.ExceptionArgumentNotPositive, variableName);
		}

		/// <summary>
		/// Checks if a value is positive.
		/// </summary>
		/// <param name="x">The value to check.</param>
		/// <param name="variableName">The variable name of the value to check.</param>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="x"/> &lt;= 0.</exception>
		public static void CheckPositive(float x, string variableName)
		{
			Platform.CheckForNullReference(variableName, "variableName");

			if (x <= 0.0f)
				throw new ArgumentException(SR.ExceptionArgumentNotPositive, variableName);
		}

		/// <summary>
		/// Checks if a value is positive.
		/// </summary>
		/// <param name="x">The value to check.</param>
		/// <param name="variableName">The variable name of the value to check.</param>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="x"/> &lt;= 0.</exception>
		public static void CheckPositive(double x, string variableName)
		{
			Platform.CheckForNullReference(variableName, "variableName");

			if (x <= 0.0d)
				throw new ArgumentException(SR.ExceptionArgumentNotPositive, variableName);
		}

		/// <summary>
		/// Checks if a value is non-negative.
		/// </summary>
		/// <param name="n">The value to check.</param>
		/// <param name="variableName">The variable name of the value to check.</param>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="ArgumentException"><paramref name="n"/> &lt; 0.</exception>
		public static void CheckNonNegative(int n, string variableName)
		{
			Platform.CheckForNullReference(variableName, "variableName");

			if (n < 0)
				throw new ArgumentException(SR.ExceptionArgumentNegative, variableName);
		}

		/// <summary>
		/// Checks if a value is within a specified range.
		/// </summary>
		/// <param name="argumentValue">Value to be checked.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <param name="variableName">Variable name of value to be checked.</param>
		/// <remarks>Checks if <paramref name="min"/> &lt;= <paramref name="argumentValue"/> &lt;= <paramref name="max"/></remarks>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="argumentValue"/> is not within the
		/// specified range.</exception>
		public static void CheckArgumentRange(int argumentValue, int min, int max, string variableName)
		{
			Platform.CheckForNullReference(variableName, "variableName");

			if (argumentValue < min || argumentValue > max)
				throw new ArgumentOutOfRangeException(String.Format(SR.ExceptionArgumentOutOfRange, argumentValue, min, max, variableName));
		}

		/// <summary>
		/// Checks if an index is within a specified range.
		/// </summary>
		/// <param name="index">Index to be checked</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <param name="obj">Object being indexed.</param>
		/// <remarks>Checks if <paramref name="min"/> &lt;= <paramref name="index"/> &lt;= <paramref name="max"/></remarks>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is not within the
		/// specified range.</exception>
		public static void CheckIndexRange(int index, int min, int max, object obj)
		{
			Platform.CheckForNullReference(obj, "obj");

			if (index < min || index > max)
				throw new IndexOutOfRangeException(String.Format(SR.ExceptionIndexOutOfRange, index, min, max, obj.GetType().Name));
		}

		/// <summary>
		/// Checks if a field or property is null.
		/// </summary>
		/// <param name="variable">Field or property to be checked.</param>
		/// <param name="variableName">Name of field or property to be checked.</param>
		/// <remarks>Use this method in your classes to verify that the object
		/// is not in an invalid state by checking that various fields and/or properties
		/// have been set, i.e., are not null.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="InvalidOperatonException"><paramref name="variable"/> is <b>null</b>.</exception>
		public static void CheckMemberIsSet(object variable, string variableName)
		{
			Platform.CheckForNullReference(variableName, "variableName");

			if (variable == null)
				throw new InvalidOperationException(String.Format(SR.ExceptionMemberNotSet, variableName));
		}

		/// <summary>
		/// Checks if a field or property is null.
		/// </summary>
		/// <param name="variable">Field or property to be checked.</param>
		/// <param name="variableName">Name of field or property to be checked.</param>
		/// <param name="detailedMessage">A more detailed and informative message describing
		/// why the object is in an invalid state.</param>
		/// <remarks>Use this method in your classes to verify that the object
		/// is not in an invalid state by checking that various fields and/or properties
		/// have been set, i.e., are not null.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="variableName"/> is <b>null</b>.</exception>
		/// <exception cref="InvalidOperatonException"><paramref name="variable"/> is <b>null</b>.</exception>
		public static void CheckMemberIsSet(object variable, string variableName, string detailedMessage)
		{
			Platform.CheckForNullReference(variableName, "variableName");
			Platform.CheckForNullReference(detailedMessage, "detailedMessage");

			if (variable == null)
				throw new InvalidOperationException(String.Format(SR.ExceptionMemberNotSetVerbose, variableName, detailedMessage));
		}

        /// <summary>
        /// Launches a file browser opened to the parent folder of a path to a single file or folder and highlights the file/folder 
        /// </summary>
        public static void OpenFileBrowser(string path)
        {
            if (IsWin32Platform)
            {                
                System.Diagnostics.Process.Start("explorer.exe", "/n,/select," + path);
            }
        }

        /// <summary>
        /// Private method to get a session manager
        /// </summary>
        /// <returns></returns>
        private static ISessionManager GetSessionManager()
        {
            try
            {
                SessionManagerExtensionPoint xp = new SessionManagerExtensionPoint();
                return (ISessionManager)xp.CreateExtension();
            }
            catch (NotSupportedException)
            {
                // no session manager implementation
                return null;
            }
        }
    }
}
