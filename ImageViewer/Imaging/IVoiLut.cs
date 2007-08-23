using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IVoiLut : ILut
	{ 
	}

	public abstract class VoiLutCreationParameters : LutCreationParameters
	{
		protected VoiLutCreationParameters(string factoryName)
			: base(factoryName)
		{
		}
	}

	public interface IVoiLutFactory : ILutFactory<IVoiLut, VoiLutCreationParameters>
	{
	}

	public class VoiLutFactoryExtensionPoint : ExtensionPoint<IVoiLutFactory>
	{
	}
}
