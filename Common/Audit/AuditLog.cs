using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace ClearCanvas.Common.Audit
{
	[ExtensionPoint]
	public class AuditSinkExtensionPoint : ExtensionPoint<IAuditSink>
	{
	}


	public class AuditLog
	{
		private readonly string _category;
		private readonly IAuditSink[] _sinks;

		#region Constructors
		
		public AuditLog(string category)
			:this(category, new IAuditSink[]{ CreateSink() })
		{
		}

		public AuditLog(string category, IAuditSink[] sinks)
		{
			_category = category;
			_sinks = sinks;			
		}
 
		#endregion	
		
		#region Public API

		public void WriteEntry(string operation, string details)
		{
			AuditLogEntryDetail entry = new AuditLogEntryDetail(
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

		private static string GetUserName()
		{
			IPrincipal p = Thread.CurrentPrincipal;
			return (p != null && p.Identity != null) ? p.Identity.Name : null;
		}

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
