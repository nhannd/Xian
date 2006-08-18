
namespace ClearCanvas.Common.Auditing
{
	/// <summary>
	/// The IAuditor interface that all auditor extensions must implement.
	/// </summary>
	public interface IAuditor
	{
		/// <summary>
		/// Audits an <see cref="IAuditMessage"/>.
		/// </summary>
		/// <param name="auditMessage">The message to be audited.</param>
		void Audit(IAuditMessage auditMessage);
	}
}
