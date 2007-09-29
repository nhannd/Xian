using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal sealed class InitialVoiLutProvider : IInitialVoiLutProvider
	{
		private static InitialVoiLutProvider _instance;

		private readonly IInitialVoiLutProvider _extensionProvider;

		private InitialVoiLutProvider()
		{
			try
			{
				_extensionProvider = new InitialVoiLutProviderExtensionPoint().CreateExtension() as IInitialVoiLutProvider;
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}

		public static InitialVoiLutProvider Instance
		{
			get
			{
				if (_instance == null)
					_instance = new InitialVoiLutProvider();

				return _instance;
			}
		}

		#region IInitialVoiLutProvider Members

		public IComposableLut GetLut(IPresentationImage presentationImage)
		{
			IComposableLut lut = null;
			if (_extensionProvider != null)
				lut = _extensionProvider.GetLut(presentationImage);

			return lut;
		}

		#endregion
	}
}
