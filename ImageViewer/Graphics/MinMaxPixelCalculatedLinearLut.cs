using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class MinMaxPixelCalculatedLinearLutCreationParameters : VoiLutCreationParameters
	{
		private IndexedPixelData _pixelData;
		private IModalityLut _modalityLut;
		private string _uniqueIdentifier;

		private double _windowWidth;
		private double _windowCenter;

		public MinMaxPixelCalculatedLinearLutCreationParameters(IndexedPixelData pixelData, IModalityLut modalityLut)
			: base(MinMaxPixelCalculatedLinearLutFactory.FactoryName)
		{
			Platform.CheckForNullReference(pixelData, "pixelData");
			Platform.CheckForNullReference(modalityLut, "modalityLut");

			_pixelData = pixelData;
			_modalityLut = modalityLut;

			_windowWidth = double.NaN;
			_windowCenter = double.NaN;
		}

		internal IndexedPixelData PixelData
		{
			get { return _pixelData; }
		}

		internal IModalityLut ModalityLut
		{
			get { return _modalityLut; }
		}

		internal double WindowWidth
		{
			get { return _windowWidth; }
			set { _windowWidth = value; }
		}

		internal double WindowCenter
		{
			get { return _windowCenter; }
			set { _windowCenter = value; }
		}

		public override string GetKey()
		{
			if (double.IsNaN(_windowWidth))
				return MinMaxPixelCalculatedLinearLutFactory.GetKey(_pixelData, _modalityLut, this.MinInputValue, this.MaxInputValue, out _windowWidth, out _windowCenter);
			else
				return MinMaxPixelCalculatedLinearLutFactory.GetKey(_pixelData, _modalityLut, this.MinInputValue, this.MaxInputValue, _windowWidth, _windowCenter);
		}
	}

	[ExtensionOf(typeof(VoiLutFactoryExtensionPoint))]
	public class MinMaxPixelCalculatedLinearLutFactory : IVoiLutFactory
	{
		internal static readonly string FactoryName = "MinMaxPixelCalculatedLinear";

		#region ILutFactory<IVoiLut,VoiLutCreationParameters> Members

		public string Name
		{
			get { return FactoryName; }
		}

		public IVoiLut Create(VoiLutCreationParameters creationParameters)
		{
			MinMaxPixelCalculatedLinearLutCreationParameters parameters = creationParameters as MinMaxPixelCalculatedLinearLutCreationParameters;
			Platform.CheckForInvalidCast(parameters, "creationParameters", typeof(MinMaxPixelCalculatedLinearLutCreationParameters).Name);
			return new MinMaxPixelCalculatedLinearLut(parameters);
		}

		#endregion

		internal static string GetKey(IndexedPixelData pixelData, IModalityLut modalityLut, int minInputValue, int maxInputValue, double windowWidth, double windowCenter)
		{
			return String.Format("Auto Min/Max Linear: MinIn={0}, MaxIn={1}, WW={2}, WC={3}", minInputValue, maxInputValue, windowWidth, windowCenter);
		}

		internal static string GetKey(IndexedPixelData pixelData, IModalityLut modalityLut, int minInputValue, int maxInputValue, out double windowWidth, out double windowCenter)
		{
			CalculateWindowWidthAndCenter(pixelData, modalityLut, out windowWidth, out windowCenter);
			return String.Format("Auto Min/Max Linear: MinIn={0}, MaxIn={1}, WW={2}, WC={3}", minInputValue, maxInputValue, windowWidth, windowCenter);
		}

		internal static void CalculateWindowWidthAndCenter(IndexedPixelData pixelData, IModalityLut modalityLut, out double windowWidth, out double windowCenter)
		{
			int minPixelValue, maxPixelValue;
			pixelData.CalculateMinMaxPixelValue(out minPixelValue, out maxPixelValue);
			minPixelValue = modalityLut[minPixelValue];
			maxPixelValue = modalityLut[maxPixelValue];
			windowWidth = maxPixelValue - minPixelValue + 1;
			windowCenter = windowWidth / 2;
		}
	}

	internal class MinMaxPixelCalculatedLinearLut : CalculatedVoiLutLinear
	{
		private IndexedPixelData _pixelData;
		private IModalityLut _modalityLut;
		private string _uniqueIdentifier;

		private double _windowWidth;
		private double _windowCenter;

		public MinMaxPixelCalculatedLinearLut(MinMaxPixelCalculatedLinearLutCreationParameters creationParameters)
			: base(creationParameters.MinInputValue, creationParameters.MaxInputValue)
		{
			_pixelData = creationParameters.PixelData;
			_modalityLut = creationParameters.ModalityLut;
			//on an undo operation, these may not be NaN, avoiding having to recalculate the min/max values unnecessarily.
			_windowWidth = creationParameters.WindowWidth;
			_windowCenter = creationParameters.WindowCenter;
		}

		public override double WindowWidth
		{
			get
			{
				Calculate();
				return _windowWidth;
			}
		}

		public override double WindowCenter
		{
			get
			{
				Calculate();
				return _windowCenter;
			}
		}

		private void Calculate()
		{
			if (double.IsNaN(_windowWidth))
			{
				MinMaxPixelCalculatedLinearLutFactory.CalculateWindowWidthAndCenter(_pixelData, _modalityLut, out _windowWidth, out _windowCenter);
				base.OnLutChanged();
			}
		}

		public override LutCreationParameters GetCreationParametersMemento()
		{
			MinMaxPixelCalculatedLinearLutCreationParameters parameters = new MinMaxPixelCalculatedLinearLutCreationParameters(_pixelData, _modalityLut);
			parameters.WindowWidth = _windowWidth;
			parameters.WindowCenter = _windowCenter;
			return parameters;
		}

		public override bool TrySetCreationParametersMemento(LutCreationParameters creationParameters)
		{
			MinMaxPixelCalculatedLinearLutCreationParameters parameters = creationParameters as MinMaxPixelCalculatedLinearLutCreationParameters;
			if (parameters == null)
				return false;

			return true;
		}

		public override string GetKey()
		{
			return MinMaxPixelCalculatedLinearLutFactory.GetKey(_pixelData, _modalityLut, this.MinInputValue, this.MaxInputValue, this.WindowWidth, this.WindowCenter);
		}
	}
}
