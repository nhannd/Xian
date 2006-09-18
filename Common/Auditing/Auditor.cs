
using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Auditing
{
	/// <summary>
	/// The Auditor extension point (extensions implement <see cref="IAuditor"/>).
	/// </summary>
	/// <remarks>
	/// Although there would normally only be a single auditor present in a running application,
	/// it is possible for there to be more than one.  For example, you might have a local auditor
	/// that logs to a text file, and a remote auditor that logs to a .NET remoting service.
	/// </remarks>
	[ExtensionPoint()]
    public class AuditorExtensionPoint : ExtensionPoint<IAuditor>
    {
    }

	/// <summary>
	/// The AuditManager is responsible for loading all Auditors (<see cref="AuditorExtensionPoint"/>) 
	/// as well as passing each <see cref="IAuditMessage"/> that has been generated to the Auditors.
	/// </summary>
	public class AuditManager : BasicExtensionPointManager<IAuditor>
	{
		public AuditManager()
		{
		}

		/// <summary>
		/// Audits an <see cref="IAuditMessage"/> to all existing auditors (<see cref="AuditorExtensionPoint"/>).
		/// </summary>
		/// <param name="auditMessage">Interface to an <see cref="IAuditMessage"/>.</param>
		/// <seealso cref="BasicExtensionPointManager.LoadExtensions"/>
		public void Audit(IAuditMessage auditMessage)
		{
			LoadExtensions();

			foreach (IAuditor auditor in this.Extensions)
			{
				try
				{
					auditor.Audit(auditMessage);
				}
				catch (Exception e)
				{
					Platform.Log(e, LogLevel.Error);
				}
			}
		}

		protected override IExtensionPoint GetExtensionPoint()
		{
			return new AuditorExtensionPoint();
		}
	}
}
