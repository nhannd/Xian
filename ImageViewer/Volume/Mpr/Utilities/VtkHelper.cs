#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using vtk;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Utilities
{
	internal static class VtkHelper
	{
		#region Error Handling helpers

		public static void RegisterVtkErrorEvents(vtkObject obj)
		{
			obj.AddObserver((uint)EventIds.ErrorEvent, VtkEventCallback);
			obj.AddObserver((uint)EventIds.WarningEvent, VtkEventCallback);
		}

		public static void VtkEventCallback(vtkObject vtkObj, uint eventId, object obj, IntPtr ptr)
		{
			const string unexpectedMessage = "Unexpected VTK event received.";
			const string message = "VTK Event 0x{0:x4}: {1}";

			switch ((EventIds) eventId)
			{
				case EventIds.WarningEvent:
					string warnDetails = Marshal.PtrToStringAnsi(ptr);
					Platform.Log(LogLevel.Warn, message, eventId, warnDetails);
					break;
				case EventIds.ErrorEvent:
					string errorDetails = Marshal.PtrToStringAnsi(ptr);
					Platform.Log(LogLevel.Error, message, eventId, errorDetails);
					break;
				default:
					Platform.Log(LogLevel.Fatal, unexpectedMessage);
					Debug.Fail(unexpectedMessage);
					break;
			}
		}

		#endregion

		#region Convert to VTK helpers

		/// <summary>
		/// Converts a <see cref="Matrix"/> to a <see cref="vtkMatrix4x4"/>.
		/// </summary>
		/// <remarks>
		/// The <see cref="vtkMatrix4x4"/> matrix is equivalent to <see cref="Matrix"/> transposed!
		/// This is due to the fact that vtkMatrix4x4 uses (x,y) addresses whereas Matrix
		/// uses (row,column).
		/// </remarks>
		/// <param name="matrix">The source <see cref="Matrix"/>.</param>
		/// <returns>The equivalent <see cref="vtkMatrix4x4"/>.</returns>
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
			vtkShortArray.SetArray(shortArray, (VtkIdType)shortArray.Length, 1);
			return vtkShortArray;
		}

		public static vtkUnsignedShortArray ConvertToVtkUnsignedShortArray(ushort[] ushortArray)
		{
			vtkUnsignedShortArray vtkUnsignedShortArray = new vtkUnsignedShortArray();
			vtkUnsignedShortArray.SetArray(ushortArray, (VtkIdType)ushortArray.Length, 1);
			return vtkUnsignedShortArray;
		}

		#endregion
	}
}