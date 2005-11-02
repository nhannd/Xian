using System;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Summary description for EventsHelper.
	/// </summary>
	public class EventsHelper
	{
		public static void Fire(Delegate del, params object[] args)
		{
			if (del == null)
				return;

			Delegate[] delegates = del.GetInvocationList();

			foreach(Delegate sink in delegates)
			{
				try
				{
					sink.DynamicInvoke(args);
				}
				catch (Exception e)
				{
					bool rethrow = Platform.HandleException(e, "LogExceptionPolicy");

					if (rethrow)
						throw;
				}
			}
		}
	}
}
