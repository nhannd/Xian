using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common;
using System.Runtime.Serialization;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	[Serializable]
	public sealed class VoiLutPreset : LutPreset
	{
		private string _label;
		private float _windowWidth;
		private float _windowCenter;

		public override string Label
		{
			get { return _label; }
			set { _label = value; }
		}

		public float WindowWidth
		{
			get { return _windowWidth; }
			set { _windowWidth = value; }
		}

		public float WindowCenter
		{
			get { return _windowCenter; }
			set { _windowCenter = value; }
		}

		public override bool Apply(IPresentationImage image)
		{
			Platform.CheckForNullReference(image, "image");

			IVOILUTLinearProvider provider = image as IVOILUTLinearProvider;

			if (provider == null)
				return false;

			WindowLevelApplicator applicator = new WindowLevelApplicator(image);

			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandWindowLevel;
			command.BeginState = applicator.CreateMemento();

			provider.VoiLutLinear.WindowWidth = this.WindowWidth;
			provider.VoiLutLinear.WindowCenter = this.WindowCenter;
			provider.Draw();

			command.EndState = applicator.CreateMemento();

			if (!command.EndState.Equals(command.BeginState))
			{
				applicator.SetMemento(command.EndState);
				image.ImageViewer.CommandHistory.AddCommand(command);
			}

			return true;
		}
	}
}
