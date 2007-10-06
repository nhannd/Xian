namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Abstract class providing the base functionality for Luts where the <see cref="WindowWidth"/>
	/// and <see cref="WindowCenter"/> are calculated and/or retrieved from an external source.
	/// </summary>
	public abstract class CalculatedVoiLutLinear : VoiLutLinearBase, IVoiLutLinear
	{
		/// <summary>
		/// Gets the Window Width.
		/// </summary>
		protected sealed override double GetWindowWidth()
		{
			return this.WindowWidth;
		}

		/// <summary>
		/// Gets the Window Center.
		/// </summary>
		protected sealed override double GetWindowCenter()
		{
			return this.WindowCenter;
		}
		
		#region IVoiLutLinear Members

		/// <summary>
		/// Gets the Window Width.
		/// </summary>
		public abstract double WindowWidth { get; }

		/// <summary>
		/// Gets the Window Center.
		/// </summary>
		public abstract double WindowCenter { get; }

		#endregion
	}
}
