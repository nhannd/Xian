namespace ClearCanvas.Enterprise.Common.Caching
{
	public abstract class CacheOptionsBase
	{
		private string _region = "";

		protected CacheOptionsBase(string region)
		{
			_region = region;
		}

		/// <summary>
		/// Gets or sets the region
		/// </summary>
		public string Region
		{
			get { return _region; }
			set { _region = value; }
		}
	}
}