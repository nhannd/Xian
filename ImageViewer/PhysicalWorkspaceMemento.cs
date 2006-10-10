using System;
using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	internal class PhysicalWorkspaceMemento : IMemento
	{
		private LogicalWorkspace _logicalWorkspace;
		private ImageBoxCollection _imageBoxes;
		private MementoList _imageBoxMementos;

		public PhysicalWorkspaceMemento(
			LogicalWorkspace logicalWorkspace,
			ImageBoxCollection imageBoxes,
			MementoList imageBoxMementos)
		{
			Platform.CheckForNullReference(logicalWorkspace, "LogicalWorkspace");
			Platform.CheckForNullReference(imageBoxes, "imageBoxes");
			Platform.CheckForNullReference(imageBoxMementos, "imageBoxMementos");

			_logicalWorkspace = logicalWorkspace;
			_imageBoxes = imageBoxes;
			_imageBoxMementos = imageBoxMementos;
		}

		public LogicalWorkspace LogicalWorkspace
		{
			get { return _logicalWorkspace; }
		}

		public ImageBoxCollection ImageBoxes
		{
			get { return _imageBoxes; }
		}

		public MementoList ImageBoxMementos
		{
			get { return _imageBoxMementos; }
		}
	}
}
