using System;
using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	internal class PhysicalWorkspaceMemento : IMemento
	{
		private LogicalWorkspace _logicalWorkspace;
		private ClientArea _clientArea;
		private ImageBoxCollection _imageBoxes;
		private MementoList _imageBoxMementos;

		public PhysicalWorkspaceMemento(
			LogicalWorkspace logicalWorkspace,
			ClientArea clientArea,
			ImageBoxCollection imageBoxes,
			MementoList imageBoxMementos)
		{
			Platform.CheckForNullReference(logicalWorkspace, "LogicalWorkspace");
			Platform.CheckForNullReference(clientArea, "clientArea");
			Platform.CheckForNullReference(imageBoxes, "imageBoxes");
			Platform.CheckForNullReference(imageBoxMementos, "imageBoxMementos");

			_logicalWorkspace = logicalWorkspace;
			_clientArea = clientArea;
			_imageBoxes = imageBoxes;
			_imageBoxMementos = imageBoxMementos;
		}

		public LogicalWorkspace LogicalWorkspace
		{
			get { return _logicalWorkspace; }
		}

		public ClientArea ClientArea
		{
			get { return _clientArea; }
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
