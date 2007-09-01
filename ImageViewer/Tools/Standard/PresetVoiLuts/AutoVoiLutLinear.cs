using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	public interface IAutoVoiLutLinear : IVoiLutLinear
	{
		void ApplyNext();
	}

	public sealed class AutoVoiLutLinear : CalculatedVoiLutLinear, IAutoVoiLutLinear
	{
		private class AutoVoiLutLinearMemento : IMemento
		{
			public readonly uint Index;

			public AutoVoiLutLinearMemento(uint index)
			{
				this.Index = index;
			}
		}

		private readonly ImageSop _imageSop;
		private uint _index;

		public AutoVoiLutLinear(ImageSop imageSop, uint index)
		{
		}

		public AutoVoiLutLinear(ImageSop imageSop)
		{
			_imageSop = imageSop;
			_index = 0;
		}
		
		private Window[] WindowCenterAndWidth
		{
			get { return _imageSop.WindowCenterAndWidth; }
		}

		#region IAutoVoiLut Members

		public void ApplyNext()
		{
			SetIndex(_index + 1);
		}
		
		#endregion

		#region IVoiLutLinear Members

		public override double WindowWidth
		{
			get { return this.WindowCenterAndWidth[_index].Width; }
		}

		public override double WindowCenter
		{
			get { return this.WindowCenterAndWidth[_index].Center; }
		}

		#endregion

		public override string GetDescription()
		{
			return String.Format("W:{0} L:{1} (Auto)", WindowWidth, WindowCenter);
		}

		public override IMemento CreateMemento()
		{
			return new AutoVoiLutLinearMemento(_index);
		}

		public override void SetMemento(IMemento memento)
		{
			AutoVoiLutLinearMemento autoMemento = memento as AutoVoiLutLinearMemento;
			Platform.CheckForInvalidCast(autoMemento, "memento", typeof(AutoVoiLutLinearMemento).Name);
			this.SetIndex(autoMemento.Index);
		}

		private void SetIndex(uint newIndex)
		{
			uint lastIndex = _index;
			_index = newIndex;
			if (_index >= this.WindowCenterAndWidth.Length)
				_index = 0;

			if (lastIndex != _index)
				base.OnLutChanged();
		}
	}
}
