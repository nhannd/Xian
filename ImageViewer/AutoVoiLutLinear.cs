using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Dicom;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	public interface IAutoVoiLutLinear : IVoiLutLinear
	{
		uint Index { get; set; }
		uint MaximumIndex { get; }
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

		private ImageSop _imageSop;
		private uint _index;

		public AutoVoiLutLinear(ImageSop imageSop, uint index)
		{
			Platform.CheckForNullReference(imageSop, "imageSop");
			Platform.CheckIndexRange((int)index, 0, imageSop.WindowCenterAndWidth.Length - 1, imageSop.WindowCenterAndWidth);
			_imageSop = imageSop;
			_index = index;
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

		public uint Index
		{
			get { return _index; }
			set
			{
				uint index = Math.Min((uint)this.WindowCenterAndWidth.Length, value);
				if (index == _index)
					return;

				_index = index;
				base.OnLutChanged();
			}
		}
		
		public uint MaximumIndex
		{
			get { return (uint)this.WindowCenterAndWidth.Length - 1; }
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

		public override IMemento CreateMemento()
		{
			return new AutoVoiLutLinearMemento(Index);
		}

		public override void SetMemento(IMemento memento)
		{
			AutoVoiLutLinearMemento autoMemento = memento as AutoVoiLutLinearMemento;
			Platform.CheckForInvalidCast(autoMemento, "memento", typeof(AutoVoiLutLinearMemento).Name);
			this.Index = autoMemento.Index;
		}
	}
}
