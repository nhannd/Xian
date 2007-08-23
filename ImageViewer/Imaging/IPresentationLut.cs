using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IPresentationLut : ILut
	{
		bool Invert { get; set; }
	}

	public abstract class PresentationLutCreationParameters : LutCreationParameters
	{
		protected PresentationLutCreationParameters(string factoryName)
			: base(factoryName)
		{
			this.Invert = false;
		}

		public bool Invert
		{
			get { return (bool)this["Invert"]; }
			set { this["Invert"] = value; }
		}
	}

	public interface IPresentationLutFactory : ILutFactory<IPresentationLut, PresentationLutCreationParameters>
	{
	}

	public class PresentationLutFactoryExtensionPoint : ExtensionPoint<IPresentationLutFactory>
	{
	}
}
