using System;
using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Common.Application;

namespace ClearCanvas.Workstation.Model
{
	/// <summary>
	/// Summary description for PhysicalWorkspaceMemento.
	/// </summary>
	internal class PhysicalWorkspaceMemento : IMemento
	{
		private LogicalWorkspace m_LogicalWorkspace;
		private ClientArea m_ClientArea;
		private ImageBoxCollection m_ImageBoxes;
		private MementoList m_ImageBoxMementos;

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

			m_LogicalWorkspace = logicalWorkspace;
			m_ClientArea = clientArea;
			m_ImageBoxes = imageBoxes;
			m_ImageBoxMementos = imageBoxMementos;
		}

		public LogicalWorkspace LogicalWorkspace
		{
			get { return m_LogicalWorkspace; }
		}

		public ClientArea ClientArea
		{
			get { return m_ClientArea; }
		}

		public ImageBoxCollection ImageBoxes
		{
			get { return m_ImageBoxes; }
		}

		public MementoList ImageBoxMementos
		{
			get { return m_ImageBoxMementos; }
		}
	}
}
