using System;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Default clock that works on all platforms.  Accuracy not guaranteed.
	/// </summary>
	/// <remarks>
	/// This class should implement a high-resolution clock.  However, the current implementation
	/// just uses the generic System.DateTime class because it is portable.
	/// </remarks>
	public class DefaultClock : IPerformanceClock
	{
		
		public long Count
		{
			get {
				return System.DateTime.Now.Ticks;
			}
		}
		
		public long Frequency
		{
			get {
				return 10000000;	// 10 million
			}
		}
		
		
	}
}
