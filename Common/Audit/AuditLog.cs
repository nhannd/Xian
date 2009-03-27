using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace ClearCanvas.Common.Audit
{
	/// <summary>
	/// Defines an extension point for audit sinks.
	/// </summary>
	[ExtensionPoint]
	public class AuditSinkExtensionPoint : ExtensionPoint<IAuditSink>
	{
	}

	/// <summary>
	/// Represents an audit log.
	/// </summary>
	/// <remarks>
	/// 
	/// </remarks>
	public class AuditLog
	{
		private readonly string _category;
		private readonly IAuditSink[] _sinks;

		#region Constructors
		
		/// <summary>
		/// Constructs an audit log with the specified category.
		/// </summary>
		/// <param name="category"></param>
		public AuditLog(string category)
			:this(category, new IAuditSink[]{ CreateSink() })
		{
		}

		/// <summary>
		/// Constructs an audit log with the specified category and sinks.
		/// </summary>
		/// <param name="category"></param>
		/// <param name="sinks"></param>
		private AuditLog(string category, IAuditSink[] sinks)
		{
			_category = category;
			_sinks = sinks;			
		}
 
		#endregion	
		
		#region Public API

		/// <summary>
		/// Writes an entry to the audit log containing the specified information.
		/// </summary>
		/// <param name="operation"></param>
		/// <param name="details"></param>
		public void WriteEntry(string operation, string details)
		{
			AuditEntryInfo entry = new AuditEntryInfo(
				_category,
				Platform.Time,
				Dns.GetHostName(),
				null,	// not currently in use
				GetUserName(),
				operation,
				details);

			foreach (IAuditSink sink in _sinks)
			{
				try
				{
					sink.WriteEntry(entry);
				}
				catch(Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Gets the identity of the current thread or null if not established.
		/// </summary>
		/// <returns></returns>
		private static string GetUserName()
		{
			IPrincipal p = Thread.CurrentPrincipal;
			return (p != null && p.Identity != null) ? p.Identity.Name : null;
		}

		/// <summary>
		/// Creates the a single audit sink via the <see cref="AuditSinkExtensionPoint"/>.
		/// </summary>
		/// <returns></returns>
		private static IAuditSink CreateSink()
		{
			try
			{
				return (IAuditSink)(new AuditSinkExtensionPoint()).CreateExtension();
			}
			catch(NotSupportedException)
			{
				//TODO: should there be some kind of default audit sink that just writes to a local log file or something
				throw;
			}
		}

		#endregion
	}
}
