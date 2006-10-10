using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Rendering;
using vtk;
using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
	public class VtkRenderingSurface : IRenderingSurface
	{
		private IntPtr _windowID;
		private IntPtr _contextID;
		private Control _hostControl;
		private vtkFormsWindowControl _vtkControl;

		public VtkRenderingSurface(IntPtr windowID)
		{
			_windowID = windowID;
			_hostControl = Control.FromHandle(_windowID);

			_vtkControl = new vtkFormsWindowControl();
			_hostControl.Controls.Add(_vtkControl);
			_vtkControl.Dock = DockStyle.Fill;
		}

		#region IRenderingSurface Members

		public IntPtr WindowID
		{
			get { return _windowID; }
		}

		public IntPtr ContextID
		{
			get { return _contextID; }
			set { _contextID = value; }
		}

		#endregion

		public vtkRenderWindow vtkRenderWindow
		{
			get { return _vtkControl.GetRenderWindow(); }
		}

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				DisposeVtkControl();
			}
		}

		private void DisposeVtkControl()
		{
			if (_vtkControl != null)
			{
				_hostControl.Controls.Remove(_vtkControl);
				_vtkControl.Dispose();
			}
		}

		internal void SetSize(int width, int height)
		{
		}

		internal void Draw(DrawArgs args)
		{
		}
	}
}
