namespace ClearCanvas.Common
{
	public interface IPerformanceClock
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
