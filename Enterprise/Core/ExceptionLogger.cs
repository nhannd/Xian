using System;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Utility class to add <see cref="ExceptionLogEntry"/>
	/// </summary>
	public class ExceptionLogger
	{
		/// <summary>
		/// Adds an <see cref="ExceptionLogEntry"/>.  If the <see cref="ExceptionLogEntry"/> cannot be created, the error is logged to the log file instead.
		/// </summary>
		/// <param name="operationName"></param>
		/// <param name="e"></param>
		public static void Log(string operationName, Exception e)
		{
			try
			{
				// log the error to the database
				using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update, PersistenceScopeOption.RequiresNew))
				{
					// disable change-set auditing for this context
					((IUpdateContext)PersistenceScope.CurrentContext).ChangeSetRecorder = null;

					DefaultExceptionRecorder recorder = new DefaultExceptionRecorder();
					ExceptionLogEntry logEntry = recorder.CreateLogEntry(operationName, e);

					PersistenceScope.CurrentContext.Lock(logEntry, DirtyState.New);

					scope.Complete();
				}
			}
			catch (Exception x)
			{
				// if we fail to properly log the exception, there is nothing we can do about it
				// just log a message to the log file
				Platform.Log(LogLevel.Error, x);

				// also log the original exception to the log file, since it did not get logged to the DB
				Platform.Log(LogLevel.Error, e);
			}
		}
	}
}