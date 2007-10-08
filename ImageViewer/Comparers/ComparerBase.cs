namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Base class for comparers that are used for sorting of collections.
	/// </summary>
	public abstract class ComparerBase
	{
		private int _returnValue;

		/// <summary>
		/// Initializes a new instance of <see cref="ComparerBase"/>.
		/// </summary>
		protected ComparerBase()
		{
			Reverse = false;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ComparerBase"/>.
		/// </summary>
		/// <param name="reverse"></param>
		protected ComparerBase(bool reverse)
		{
			this.Reverse = reverse;
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not the collection will be
		/// sorted in ascending or descending order.
		/// </summary>
		public bool Reverse
		{
			get
			{ 
				return ( this.ReturnValue == 1); 
			}
			set
			{
				if (!value)
					this.ReturnValue = -1;
				else
					this.ReturnValue = 1;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not the collection will be
		/// sorted in ascending or descending order.
		/// </summary>
		/// <value>1 if <see cref="Reverse"/> is <b>true</b></value>
		/// <value>-1 if <see cref="Reverse"/> is <b>false</b></value>
		protected int ReturnValue
		{
			get { return _returnValue; }
			set { _returnValue = value; }
		}
	}
}
