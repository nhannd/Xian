using System;
using System.IO;
using System.Reflection;
using System.Resources;
using log4net;
using log4net.spi;
using log4net.Config;

// Configure log4net using the .log4net file
[assembly: log4net.Config.DOMConfigurator(ConfigFile = "Logging.config", Watch = true)]
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
	/// A collection of useful utility functions.
	/// </summary>
	public static class Platform
	{
		private static string _installDir = null;
		private static string _pluginDir = "plugins";
		private static string _studyDir = "studies";
		private static string _logDir = "logs";
		private static volatile PluginManager _pluginManager;
		private static object _syncRoot = new Object();
		private static readonly ILog _log = LogManager.GetLogger(typeof(Platform));
        private static IApplicationRoot _applicationRoot;

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
							_pluginManager = new PluginManager(PluginDir);
					}
				}

				return _pluginManager;
			}
		}

		public static bool IsWin32Platform
		{
			get
			{
				PlatformID id = Environment.OSVersion.Platform;
				return (id == PlatformID.Win32NT || id == PlatformID.Win32Windows || id == PlatformID.Win32S || id == PlatformID.WinCE);
			}
		}
		
		public static bool IsUnixPlatform
		{
			get
			{
				PlatformID id = Environment.OSVersion.Platform;
				return (id == PlatformID.Unix);
			}
		}

        public static char PathSeparator
        {
            get { return IsWin32Platform ? '\\' : '/'; }
        }
		
		/// <summary>
		/// Gets or sets ClearCanvas' installation directory.
		/// </summary>
		/// <value>ClearCanvas' fully qualified installation directory.</value>
		public static string InstallDir
		{
			get
			{
				if (_installDir == null)
					_installDir = Directory.GetCurrentDirectory();

				return _installDir;
			}
			set
			{
				_installDir = value;
			}
		}

		/// <summary>
		/// Gets or sets the plugin directory.
		/// </summary>
		/// <value>The fully qualified plugin directory.</value>
		public static string PluginDir
		{
			get
			{
                return string.Format("{0}{1}{2}", InstallDir, PathSeparator, _pluginDir);
			}
		}

		/// <summary>
		/// Gets or sets the study directory.
		/// </summary>
		/// <value>The fully qualified study directory.</value>
		public static string StudyDir
		{
			get
			{
                return string.Format("{0}{1}{2}", InstallDir, PathSeparator, _studyDir);
			}
		}

		/// <summary>
		/// Gets or sets the log directory.
		/// </summary>
		/// <value>The fully qualified log directory.</value>
		public static string LogDir
		{
			get
			{
                return string.Format("{0}{1}{2}", InstallDir, PathSeparator, _logDir);
			}
		}

		// Public methods

		/// <summary>
		/// Starts the application.
		/// </summary>
		/// <remarks>
		/// A ClearCanvas based application is started by calling this convenience method from
		/// a bootstrap executable of some kind.  Calling this method results in the loading
		/// of all plugins and creation of an IApplicationRoot extension.
		/// </remarks>
		public static void StartApp()
		{
            try
            {
                _applicationRoot = (IApplicationRoot)Platform.CreateExtension(typeof(IApplicationRoot));
                _applicationRoot.RunApplication();
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }

		/// <summary>
		/// Writes a message to the default log.
		/// </summary>
		/// <param name="message"></param>
		public static void Log(object message)
		{
			Log(message, LogLevel.Info);
		}

		public static void Log(Exception ex)
		{
			Log(SR.ExceptionThrown, LogLevel.Error);
		}

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
		/// Handles a caught exception.
		/// </summary>
		/// <param name="ex">The caught exception.</param>
		/// 
		/// <returns><b>true</b> if the exception is to be rethrown, <b>false</b> if not</returns>
		/// <remarks>Used in a catch block, this utility method allows an exception
		/// to be handled in a way defined by an XML defined exception policy.  This way, changes
		/// in exception policies don't have to implemented by recompiling new code,
		/// but rather by changing an XML file at runtime.  Policies are typically defined
		/// in the <c>exceptionhandlingconfiguration.config</c> file of the application.  This method
		/// is just a wrapper around the Enterprise Library's HandleException method.
		/// </remarks>
		public static bool HandleException(Exception ex)
		{
			Log(ex);
			return true;
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
        /// Creates an instance of the extension of the specified type, searching across all plugins
        /// for extensions.  If more than one extension is found, only the first one is created.  If no
        /// extensions are found, a <see cref="NotSupportedException"/> is thrown.
        /// </summary>
        /// <param name="extensionOfType">The type of the extension point interface</param>
        /// <returns>An instance of the requested extension.</returns>
        public static object CreateExtension(Type extensionOfType)
        {
            CheckForNullReference(extensionOfType, "extensionOfType");
            return ExtensionLoader.CreateExtension(PluginManager.Extensions, extensionOfType, null);
        }

        /// <summary>
        /// Creates an instance of the extension of the specified type, searching across all plugins
        /// for extensions.  Only extensions matching the specified filter are considered.
        /// If more than one extension is found, only the first one is created.  If no
        /// extensions are found, a <see cref="NotSupportedException"/> is thrown.
        /// </summary>
        /// <param name="extensionOfType">The type of the extension point interface</param>
        /// <param name="filter">The filter provides additional criteria to select extensions</param>
        /// <returns>An instance of the requested extension.</returns>
        public static object CreateExtension(Type extensionOfType, ExtensionFilter filter)
        {
            CheckForNullReference(extensionOfType, "extensionOfType");
            CheckForNullReference(filter, "filter");

            return ExtensionLoader.CreateExtension(PluginManager.Extensions, extensionOfType, filter);
        }

        /// <summary>
        /// Creates an instance of each extension of the specified type, searching across all plugins
        /// for extensions.  If no extensions are found, the returned array is empty.
        /// </summary>
        /// <param name="extensionOfType">The type of the extension point interface</param>
        /// <returns>An array containing one instance of each extension that was created.</returns>
        public static object[] CreateExtensions(Type extensionOfType)
        {
            CheckForNullReference(extensionOfType, "extensionOfType");

            return ExtensionLoader.CreateExtensions(PluginManager.Extensions, extensionOfType, null);
        }

        /// <summary>
        /// Creates an instance of each extension of the specified type, searching across all plugins
        /// for extensions.  If no extensions are found, the returned array is empty.
        /// </summary>
        /// <param name="filter">The filter provides additional criteria to select extensions</param>
        /// <param name="extensionOfType">The type of the extension point interface</param>
        /// <returns>An array containing one instance of each extension that was created.</returns>
        public static object[] CreateExtensions(Type extensionOfType, ExtensionFilter filter)
        {
            CheckForNullReference(extensionOfType, "extensionOfType");
            CheckForNullReference(filter, "filter");

            return ExtensionLoader.CreateExtensions(PluginManager.Extensions, extensionOfType, filter);
        }
	}
}
