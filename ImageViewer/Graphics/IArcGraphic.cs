namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines an arc graphic.
	/// </summary>
	public interface IArcGraphic : IBoundableGraphic
	{
		/// <summary>
		/// Gets or sets the angle at which the arc begins.
		/// </summary>
		float StartAngle { get; set; }

		/// <summary>
		/// Gets or sets the angle that the arc sweeps out.
		/// </summary>
		float SweepAngle { get; set; }
	}
}