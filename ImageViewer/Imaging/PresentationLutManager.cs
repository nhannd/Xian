using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal class PresentationLutManagerMemento : IMemento
	{
		private string _name;

		public PresentationLutManagerMemento(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}
	}

	internal class PresentationLutManager : IPresentationLutManager
	{
		private GrayscaleImageGraphic _grayscaleImageGraphic;

		public PresentationLutManager(GrayscaleImageGraphic grayscaleImageGraphic)
		{
			Platform.CheckForNullReference(grayscaleImageGraphic, "grayscaleImageGraphic");
			_grayscaleImageGraphic = grayscaleImageGraphic;
		}

		#region IPresentationLutManager Members

		public void SetPresentationLut(string name)
		{
			_grayscaleImageGraphic.SetVoiLut(name);
		}

		public IPresentationLut PresentationLut
		{
			get { return _grayscaleImageGraphic.PresentationLut; }
		}

		#endregion

		#region IMemorable Members

		public IMemento CreateMemento()
		{
			return new PresentationLutManagerMemento(this.PresentationLut.Name);
		}

		public void SetMemento(IMemento memento)
		{
			PresentationLutManagerMemento presentationLutManagerMemento = memento as PresentationLutManagerMemento;

			SetPresentationLut(presentationLutManagerMemento.Name);
		}

		#endregion
	}
}
