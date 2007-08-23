using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class GrayscalePresentationLutCreationParameters : PresentationLutCreationParameters
	{
		internal GrayscalePresentationLutCreationParameters()
			: base(GrayscalePresentationLutFactory.FactoryName)
		{
		}

		public override string GetKey()
		{
			return PresentationLut.GetKey<GrayscalePresentationLut>(this.MinInputValue, this.MaxInputValue, this.Invert);
		}
	}

	[ExtensionOf(typeof(PresentationLutFactoryExtensionPoint))]
	public class GrayscalePresentationLutFactory : IPresentationLutFactory
	{
		internal static readonly string FactoryName = "Grayscale";

		public GrayscalePresentationLutFactory()
		{ 
		}

		#region ILutFactory<IPresentationLut,PresentationLutCreationParameters> Members

		public string Name
		{
			get { return FactoryName; }
		}

		public IPresentationLut Create(PresentationLutCreationParameters creationParameters)
		{
			GrayscalePresentationLutCreationParameters parameters = creationParameters as GrayscalePresentationLutCreationParameters;
			Platform.CheckForInvalidCast(parameters, "creationParameters", typeof(GrayscalePresentationLutCreationParameters).Name);

			return new GrayscalePresentationLut(parameters.MinInputValue, parameters.MaxInputValue, parameters.Invert);
		}

		#endregion
	}

	internal class GrayscalePresentationLut : PresentationLut
	{
		internal GrayscalePresentationLut(
			int minInputValue, 
			int maxInputValue,
			bool invert) : base(minInputValue, maxInputValue, invert)
		{
			
		}

		protected override void CreateLut()
		{
			Color color;

			int j = 0;
			uint maxGrayLevel = this.Length - 1;

			for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
			{
				float scale = (float)j / (float)maxGrayLevel;
				j++;

				int value = (int)(byte.MaxValue * scale);
				color = Color.FromArgb(255, value, value, value);
				this[i] = color.ToArgb();
			}
		}

		public override LutCreationParameters GetCreationParametersMemento()
		{
			GrayscalePresentationLutCreationParameters parameters = new GrayscalePresentationLutCreationParameters();
			parameters.Invert = this.Invert;
			return parameters;
		}

		public override bool TrySetCreationParametersMemento(LutCreationParameters creationParameters)
		{
			GrayscalePresentationLutCreationParameters parameters = creationParameters as GrayscalePresentationLutCreationParameters;
			if (parameters == null)
				return false;

			this.Invert = parameters.Invert;
			return true;
		}
	}
}
