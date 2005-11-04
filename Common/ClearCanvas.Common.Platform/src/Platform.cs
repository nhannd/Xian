using System;
using System.IO;
using System.Reflection;
using System.Resources;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace ClearCanvas.Common
{
	public enum LogCategory
	{
		Trace,
		General,
		Warning,
		Fatal
	}

	/// <summary>
	/// A collection of useful utility functions.  
	/// </summary>
	public class Platform 
	{
		// Private attributes
		private static string m_InstallDir = null;
		private static string m_PluginDir = "plugins";
		private static string m_StudyDir = "studies";
		private static string m_LogDir = "logs";
		private static volatile PluginManager m_PluginManager;
		private static object syncRoot = new Object();

		// Properties

		/// <summary>
		/// Gets a reference to the one and only PluginManager.
		/// </summary>
		public static PluginManager PluginManager 
		{
			get
			{
				if (m_PluginManager == null)
				{
					lock (syncRoot)
					{
						if (m_PluginManager == null)
							m_PluginManager = new PluginManager(PluginDir);
					}
				}

				return m_PluginManager;
			}
		}

		/// <summary>
		/// Gets or sets the installation directory.
		/// </summary>
		public static string InstallDir
		{
			get
			{
				if (m_InstallDir == null)
					m_InstallDir = Directory.GetCurrentDirectory();

				return m_InstallDir;
			}
			set
			{
				m_InstallDir = value;
			}
		}

		/// <summary>
		/// Gets or sets the plugin directory.
		/// </summary>
		public static string PluginDir
		{
			get
			{
				return InstallDir + "\\" + m_PluginDir;
			}
		}

		/// <summary>
		/// Gets or sets the study directory.
		/// </summary>
		public static string StudyDir
		{
			get
			{
				return InstallDir + "\\" + m_StudyDir;
			}
		}

		/// <summary>
		/// Gets or sets the log directory.
		/// </summary>
		public static string LogDir
		{
			get
			{
				return InstallDir + "\\" + m_LogDir;
			}
		}

		// Public methods

		/// <summary>
		/// Starts the application.
		/// </summary>
		/// <remarks> 
		/// A ClearCanvas based application is started by calling this convenience method from
		/// a bootstrap executable of some kind.  Calling this method results in the loading
		/// of all plugins and starting of the model and view plugins (in that order).  
		/// </remarks>
		public static void StartApp()
		{
			PluginManager.StartModelPlugin();
			PluginManager.StartViewPlugin();
		}

		/// <summary>
		/// Writes a message to the default log.
		/// </summary>
		/// <param name="message"></param>
		public static void Log(object message)
		{
			Logger.Write(message);
		}

		/// <summary>
		/// Writes a message to
		/// </summary>
		/// <param name="message"></param>
		/// <param name="category"></param>
		public static void Log(object message, string category)
		{
			Logger.Write(message, category);
		}

		public static void Log(object message, LogCategory category)
		{
			string str;

			switch (category)
			{
				case LogCategory.Trace:
					str = "Trace";
					break;
				case LogCategory.General:
					str = "General";
					break;
				case LogCategory.Warning:
					str = "Warning";
					break;
				default:
					str = "Fatal";
					break;
			}

			Logger.Write(message, str);
		}

		/// <summary>
		/// Handles a caught exception.
		/// </summary>
		/// <param name="ex">The caught exception.</param>
		/// <param name="policy">The name of the policy to use when handling the exception.</param>
		/// <returns><b>true</b> if the exception is to be rethrown, <b>false</b> if not</returns>
		/// <remarks>Used in a catch block, this utility method allows an exception
		/// to be handled in a way defined by an XML defined exception policy.  This way, changes
		/// in exception policies don't have to implemented by recompiling new code,
		/// but rather by changing an XML file at runtime.  Policies are typically defined
		/// in the <c>exceptionhandlingconfiguration.config</c> file of the application.  This method
		/// is just a wrapper around the Enterprise Library's HandleException method.
		/// </remarks>
		public static bool HandleException(Exception ex, string policy)
		{
			return ExceptionPolicy.HandleException(ex, policy);
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
			ArgumentValidation.CheckForEmptyString(variable, variableName);
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
			ArgumentValidation.CheckForNullReference(variable, variableName);
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
			ArgumentValidation.CheckExpectedType(variable, type);
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
		/// layer = new OverlayLayer();
		/// OverlayLayer overlay = layer as OverlayLayer;
		/// // No exception thrown
		/// Platform.CheckForInvalidCast(overlay, "layer", "OverlayLayer");
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
				throw new InvalidCastException(SR.ExceptionInvalidCast(castInputName, castTypeName));
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
				throw new ArgumentOutOfRangeException(SR.ExceptionArgumentOutOfRange(argumentValue, min, max, variableName));
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
				throw new IndexOutOfRangeException(SR.ExceptionIndexOutOfRange(index, min, max, obj.GetType().Name));
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
				throw new InvalidOperationException(SR.ExceptionMemberNotSet(variableName));
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
				throw new InvalidOperationException(SR.ExceptionMemberNotSetVerbose(variableName, detailedMessage));
		}
	}
}