using System;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	internal sealed class VoiLutLinearPresetApplicator : IVoiLutPresetApplicator
	{
		private int _windowWidth;
		private int _windowCenter;

		public VoiLutLinearPresetApplicator(int windowWidth, int windowCenter)
		{
			_windowWidth = windowWidth;
			_windowCenter = windowCenter;
		}

		public int WindowWidth
		{
			get { return _windowWidth; }
		}

		public int WindowCenter
		{
			get { return _windowCenter; }
		}

		#region IVoiLutPresetApplicator Members

		public bool AppliesTo(IPresentationImage image)
		{
			return (image is IVOILUTLinearProvider);
		}

		public void Apply(IPresentationImage image)
		{
			Platform.CheckForNullReference(image, "image");

			IVOILUTLinearProvider provider = image as IVOILUTLinearProvider;
			Platform.CheckForInvalidCast(provider, "image", "IVOILUTLinearProvider");
			
			WindowLevelApplicator applicator = new WindowLevelApplicator(image);
			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandWindowLevel;
			command.BeginState = applicator.CreateMemento();

			provider.VoiLutLinear.WindowWidth = this.WindowWidth;
			provider.VoiLutLinear.WindowCenter = this.WindowCenter;

			applicator.ApplyToLinkedImages();
			command.EndState = applicator.CreateMemento();

			if (!command.EndState.Equals(command.BeginState))
				image.ImageViewer.CommandHistory.AddCommand(command);
		}

		#endregion
	}
}
