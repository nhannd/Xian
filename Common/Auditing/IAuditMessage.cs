
namespace ClearCanvas.Common.Auditing
{
	/// <summary>
	/// IAuditMessage is a base interface for any message that is sent to an auditor.
	/// </summary>
	/// 
	/// <remarks>
	/// The following assumptions are made about audit messages:
	/// 
	///	1. Audit messages are text-based.
	/// 2. The GetMessage method is used to retrieve the message text.  The message text
	///    is formatted/created by the implementation of IAuditMessage.
	/// 3. It is up to the implementor, but it is recommended that the implementation of
	///    IAuditMessage be responsible (and therefore have access to all necessary information)
	///    for constructing the message text.  The Auditor can then simply use the GetMessage method
	///    to log the message to the appropriate Audit Repository.  In this scenario, the
	///    auditor is only responsible for ensuring that the audit messages are reliably stored
	///    to the Audit Repository (whatever that may be: text file, logging service, etc).  The implementation
	///    of IAuditMessage is responsible for gathering the required information to format the
	///    message appropriately.
	/// </remarks>

	public interface IAuditMessage
	{
		/// <summary>
		/// Get the text of the Audit Message.
		/// </summary>
		/// <returns>The message text.</returns>
		string GetMessage();
	}
}
