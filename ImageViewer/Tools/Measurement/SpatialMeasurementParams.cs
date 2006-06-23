using System;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	/// <summary>
	/// Summary description for MeasurementParams.
	/// </summary>
	public class SpatialMeasurementParams
	{
		double m_PixelSpacingX;
		double m_PixelSpacingY;
		MeasurementUnits m_Units;

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
				return m_PixelSpacingX;
			}
			set
			{
				m_PixelSpacingX = value;
			}
		}

		public double PixelSpacingY
		{
			get
			{
				return m_PixelSpacingY;
			}
			set
			{
				m_PixelSpacingY = value;
			}
		}

		public MeasurementUnits Units
		{
			get
			{
				return m_Units;
			}
			set
			{
				m_Units = value;
			}
		}
	}
}
