namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// A wrapping of the Win32 high resolution performance counter
	/// that can be used to profile code.  Taken from an MSDN article.
    /// 
    /// Do not use this class directly - use <see cref="CodeClock" /> instead.
	/// </summary>
	internal class Win32PerformanceCounter : IPerformanceCounter
	{
		[System.Runtime.InteropServices.DllImport("KERNEL32")]
		private static extern bool QueryPerformanceCounter(  ref long lpPerformanceCount);
		
		[System.Runtime.InteropServices.DllImport("KERNEL32")]
		private static extern bool QueryPerformanceFrequency( ref long lpFrequency);

		public long Count
		{
			get {
				long count = 0;
				QueryPerformanceCounter(ref count);
				return count;
			}
		}
		
		public long Frequency
		{
			get {
				long freq = 0;
				QueryPerformanceFrequency(ref freq);
				return freq;
			}
		}
	}
}
