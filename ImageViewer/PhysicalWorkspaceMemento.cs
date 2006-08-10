using System;
using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Summary description for PhysicalWorkspaceMemento.
	/// </summary>
	internal class PhysicalWorkspaceMemento : IMemento
	{
		private LogicalWorkspace _LogicalWorkspace;
		private ClientArea _ClientArea;
		private ImageBoxCollection _ImageBoxes;
		private MementoList _ImageBoxMementos;

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

			_LogicalWorkspace = logicalWorkspace;
			_ClientArea = clientArea;
			_ImageBoxes = imageBoxes;
			_ImageBoxMementos = imageBoxMementos;
		}

		public LogicalWorkspace LogicalWorkspace
		{
			get { return _LogicalWorkspace; }
		}

		public ClientArea ClientArea
		{
			get { return _ClientArea; }
		}

		public ImageBoxCollection ImageBoxes
		{
			get { return _ImageBoxes; }
		}

		public MementoList ImageBoxMementos
		{
			get { return _ImageBoxMementos; }
		}
	}
}
