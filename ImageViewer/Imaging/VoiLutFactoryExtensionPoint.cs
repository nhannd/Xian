using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IVoiLutFactory
	{
		string Name { get; }
		string Description { get; }

		bool AppliesTo(IPresentationImage presentationImage);
		ILut Create(IPresentationImage presentationImage);
	}

	public sealed class VoiLutFactoryExtensionPoint : ExtensionPoint<IVoiLutFactory>
	{
	}
}
