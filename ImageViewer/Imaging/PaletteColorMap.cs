#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// N.B. To be used only for colorspace conversions of a palette color image - do not install as a regular color map!
	/// </summary>
	internal class PaletteColorMap : ColorMap
	{
		private readonly PaletteColorLut _lut;

		public PaletteColorMap(PaletteColorLut lut)
		{
			this.MinInputValue = lut.FirstMappedPixelValue;
			this.MaxInputValue = lut.FirstMappedPixelValue + lut.CountEntries - 1;
			_lut = lut;
		}

		protected override void Create()
		{
			for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
			{
				this[i] = _lut[i].ToArgb();
			}
		}

		public override string GetDescription()
		{
			return SR.DescriptionPaletteColorMap;
		}

		public static PaletteColorMap Create(IDicomAttributeProvider dataSource)
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			PaletteColorLut paletteColorLut = PaletteColorLut.Create(dataSource);

			clock.Stop();
			PerformanceReportBroker.PublishReport("PaletteColorMap", "Create(IDicomAttributeProvider)", clock.Seconds);

			return new PaletteColorMap(paletteColorLut);
		}
	}
}