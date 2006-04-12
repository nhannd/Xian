using System;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Helps clients raise events safely.
	/// </summary>
	public class EventsHelper
	{
		/// <summary>
		/// Raises events safely.
		/// </summary>
		/// <param name="del">Delegate.</param>
		/// <param name="args">Parameters to be passed to the delegate.</param>
		/// <remarks>Use this method to safely invoke user code via delegates.
		/// Because there is no guarantee that user code will not throw
		/// exceptions, it has to be executed safely.  That is, any exceptions thrown
		/// must be handled.  This method will log any exceptions thrown in user code.
		/// Whether the exception is rethrown depends on how the exception policy
		/// has been configured.  The typical usage is shown below.</remarks>
		/// <example>
		/// <code>
		/// [C#]
		/// public class PresentationImage
		/// {
		///    private event EventHandler _imageDrawingEvent;
		///    
		///    public void Draw()
		///    {
		///       EventsHelper.Fire(_imageDrawingEvent, this, new DrawImageEventArgs());
		///    }
		/// }
		/// </code>
		/// </example>
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
					bool rethrow = Platform.HandleException(e);

					if (rethrow)
						throw e;
				}
			}
		}
	}
}
