using System;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	/// <summary>
	/// Summary description for MeasurementParams.
	/// </summary>
	public class SpatialMeasurementParams
	{
		double _PixelSpacingX;
		double _PixelSpacingY;
		MeasurementUnits _Units;

		public enum MeasurementUnits
		{
			Centimeters,
			Millimeters
		}

		public SpatialMeasurementParams()
		{
		}

		public double PixelSpacingX
		{
			get
			{
				return _PixelSpacingX;
			}
			set
			{
				_PixelSpacingX = value;
			}
		}

		public double PixelSpacingY
		{
			get
			{
				return _PixelSpacingY;
			}
			set
			{
				_PixelSpacingY = value;
			}
		}

		public MeasurementUnits Units
		{
			get
			{
				return _Units;
			}
			set
			{
				_Units = value;
			}
		}
	}
}
