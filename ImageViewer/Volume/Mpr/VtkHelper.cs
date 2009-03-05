using System;
using System.Diagnostics;
using ClearCanvas.ImageViewer.Mathematics;
using vtk;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	internal class VtkHelper
	{
		#region Error Handling helpers

		public static void RegisterVtkErrorEvents(vtkObject obj)
		{
			obj.AddObserver((uint)EventIds.ErrorEvent, VtkEventCallback);
			obj.AddObserver((uint)EventIds.WarningEvent, VtkEventCallback);
		}

		public static void VtkEventCallback(vtkObject vtkObj, uint eventId, object obj, IntPtr ptr)
		{
			switch ((EventIds)eventId)
			{
				case EventIds.ErrorEvent:
				case EventIds.WarningEvent:
					{
						string explanation = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ptr);
						Debug.WriteLine(explanation);
						//TODO: Hook into CC logging/exception framework
					}
					break;
				default:
					Debug.Fail("Unexpected VTK event received");
					break;
			}
		}

		#endregion

		#region Convert to VTK helpers

		// Note: vtkMatrix4x4 and Math.Matrix are transposed! This is due to the
		//	fact that vtkMatrix4x4 uses an x,y interface where Matrix uses a row,col interface.
		public static vtkMatrix4x4 ConvertToVtkMatrix(Matrix matrix)
		{
			vtkMatrix4x4 vtkMatrix = new vtkMatrix4x4();

			for (int row = 0; row < 4; row++)
				for (int column = 0; column < 4; column++)
					vtkMatrix.SetElement(column, row, matrix[row, column]);

			return vtkMatrix;
		}

		public static vtkShortArray ConvertToVtkShortArray(short[] shortArray)
		{
			vtkShortArray vtkShortArray = new vtkShortArray();
			vtkShortArray.SetArray(shortArray, shortArray.Length, 1);
			return vtkShortArray;
		}

		public static vtkUnsignedShortArray ConvertToVtkUnsignedShortArray(ushort[] ushortArray)
		{
			vtkUnsignedShortArray vtkUnsignedShortArray = new vtkUnsignedShortArray();
			vtkUnsignedShortArray.SetArray(ushortArray, ushortArray.Length, 1);
			return vtkUnsignedShortArray;
		}

		#endregion
	}
}
