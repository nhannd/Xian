using System;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public class SpatialMeasurementParams
	{
		double _pixelSpacingX;
		double _pixelSpacingY;
		MeasurementUnits _units;

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
				return _pixelSpacingX;
			}
			set
			{
				_pixelSpacingX = value;
			}
		}

		public double PixelSpacingY
		{
			get
			{
				return _pixelSpacingY;
			}
			set
			{
				_pixelSpacingY = value;
			}
		}

		public MeasurementUnits Units
		{
			get
			{
				return _units;
			}
			set
			{
				_units = value;
			}
		}
	}
}
