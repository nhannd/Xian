namespace ClearCanvas.Common
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
