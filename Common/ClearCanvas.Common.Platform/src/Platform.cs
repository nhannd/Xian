///////////////////////////////////////////////////////////
//
//  Platform.cs
//
//  Created on:      08-Feb-2005 10:35:54 PM
//
//  Original author: Norman Young
//
///////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Reflection;
using System.Resources;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace ClearCanvas.Common.Platform
{
	public enum LogCategory
	{
		Trace,
		General,
		Warning,
		Fatal
	}

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

		public static string PluginDir
		{
			get
			{
				return InstallDir + "\\" + m_PluginDir;
			}
		}

		public static string StudyDir
		{
			get
			{
				return InstallDir + "\\" + m_StudyDir;
			}
		}

		public static string LogDir
		{
			get
			{
				return InstallDir + "\\" + m_LogDir;
			}
		}

		// Public methods
		public static void StartApp()
		{
			PluginManager.StartModelPlugin();
			PluginManager.StartMainUIPlugin();
		}

		public static Plugin GetPlugin(string name)
		{
			return PluginManager.GetPlugin(name);
		}

		// Logging
		public static void Log(object message)
		{
			Logger.Write(message);
		}

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

		// Exception handling
		public static bool HandleException(Exception ex, string policy)
		{
			return ExceptionPolicy.HandleException(ex, policy);
		}

		// Argument validation
		public static void CheckForEmptyString(string variable, string variableName)
		{
			ArgumentValidation.CheckForEmptyString(variable, variableName);
		}

		public static void CheckForNullReference(object variable, string variableName)
		{
			ArgumentValidation.CheckForNullReference(variable, variableName);
		}

		public static void CheckForInvalidNullNameReference(string name, string messageName)
		{
			ArgumentValidation.CheckForInvalidNullNameReference(name, messageName);
		}

		public static void CheckExpectedType(object variable, Type type)
		{
			ArgumentValidation.CheckExpectedType(variable, type);
		}

		public static void CheckEnumeration(Type enumType, object variable, string variableName)
		{
			ArgumentValidation.CheckEnumeration(enumType, variable, variableName);
		}

		public static void CheckPositive(int n, string variableName)
		{
			if (n <= 0)
				throw new ArgumentException(SR.ExceptionArgumentNotPositive, variableName);
		}

		public static void CheckPositive(float x, string variableName)
		{
			if (x <= 0.0f)
				throw new ArgumentException(SR.ExceptionArgumentNotPositive, variableName);
		}

		public static void CheckPositive(double x, string variableName)
		{
			if (x <= 0.0d)
				throw new ArgumentException(SR.ExceptionArgumentNotPositive, variableName);
		}

		public static void CheckNonNegative(int n, string variableName)
		{
			if (n < 0)
				throw new ArgumentException(SR.ExceptionArgumentNegative, variableName);
		}
	}
}