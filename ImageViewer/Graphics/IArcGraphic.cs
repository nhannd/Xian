#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
		/// <remarks>
		/// It is good practice to set the <see cref="StartAngle"/> before the <see cref="SweepAngle"/>
		/// because in the case where a graphic is scaled differently in x than in y, the conversion
		/// of the <see cref="SweepAngle"/> from <see cref="CoordinateSystem.Source"/> to
		/// <see cref="CoordinateSystem.Destination"/> coordinates is dependent upon the <see cref="StartAngle"/>.
		/// However, under normal circumstances, where the scale in x and y are the same, the <see cref="StartAngle"/>
		/// and <see cref="SweepAngle"/> can be set independently.
		/// </remarks>
		float StartAngle { get; set; }

		/// <summary>
		/// Gets or sets the angle that the arc sweeps out.
		/// </summary>
		/// <remarks>
		/// See <see cref="StartAngle"/> for information on setting the <see cref="SweepAngle"/>.
		/// </remarks>
		float SweepAngle { get; set; }
	}
}