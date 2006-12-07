using System;
using vtk;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
	interface IVtkProp
	{
		void ApplySetting(string setting);
		vtkProp VtkProp { get; }
	}
}
