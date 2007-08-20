using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal class VoiLutManagerMemento : IMemento
	{
		private string _name;

		public VoiLutManagerMemento(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}
	}

	internal class VoiLutManager : IVoiLutManager
	{
		private GrayscaleImageGraphic _grayscaleImageGraphic;

		public VoiLutManager(GrayscaleImageGraphic grayscaleImageGraphic)
		{
			Platform.CheckForNullReference(grayscaleImageGraphic, "grayscaleImageGraphic");
			_grayscaleImageGraphic = grayscaleImageGraphic;
		}

		#region IVoiLutManager Members

		public void SetVoiLut(string name)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public IVoiLut VoiLut
		{
			get { return _grayscaleImageGraphic.VoiLut; }
		}

		#endregion

		#region IMemorable Members

		public IMemento CreateMemento()
		{
			return new VoiLutManagerMemento(this.VoiLut.Name);
		}

		public void SetMemento(IMemento memento)
		{
			VoiLutManagerMemento voiLutManagerMemento = memento as VoiLutManagerMemento;

			SetVoiLut(voiLutManagerMemento.Name);
		}

		#endregion

	}
}
