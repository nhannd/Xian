namespace ClearCanvas.Common.Utilities
{
	public interface IPerformanceCounter
	{
		long Count
		{
			get;
		}
		
		long Frequency
		{
			get;
		}
	}
}
