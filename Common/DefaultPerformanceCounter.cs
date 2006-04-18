using System;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Default performance counter that works on all platforms.  Accuracy not guaranteed.
    /// 
    /// Do not use this class directly - use <see cref="CodeClock" /> instead.
    /// </summary>
	internal class DefaultPerformanceCounter : IPerformanceCounter
	{
		
		public long Count
		{
			get {
				return System.DateTime.UtcNow.Ticks;
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
