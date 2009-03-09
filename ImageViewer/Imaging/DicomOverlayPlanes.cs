using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IDicomOverlayPlanes : IEnumerable<DicomOverlayPlane>
	{
		DicomOverlayPlane this[int index] { get; }

		int Count { get; }
	}

	internal class DicomOverlayPlanes : IDicomOverlayPlanes, IList<DicomOverlayPlane>
	{
		private readonly IDicomPresentationImage _dicomPresentationImage;
		private readonly OverlayPlanesGraphic _overlayPlanesGraphic;
		private readonly List<DicomOverlayPlane> _overlayPlanes;

		public DicomOverlayPlanes(IDicomPresentationImage dicomPresentationImage)
		{
			Platform.CheckForNullReference(dicomPresentationImage, "overlayGraphicsProvider");
			_overlayPlanes = new List<DicomOverlayPlane>();
			_overlayPlanesGraphic = new OverlayPlanesGraphic();
			_dicomPresentationImage = dicomPresentationImage;
		}

		internal void Populate()
		{
			if (!_dicomPresentationImage.DicomGraphics.Contains(_overlayPlanesGraphic))
			{
				OverlayPlaneModuleIod overlays = new OverlayPlaneModuleIod(_dicomPresentationImage.ImageSop.DataSource);
				foreach (OverlayPlane overlay in overlays)
				{
					try
					{
						DicomOverlayPlane dop = new DicomOverlayPlane(overlay);
						this.Add(dop);
					}
					catch (Exception ex)
					{
						Platform.Log(LogLevel.Warn, ex, "Failed to render image overlay plane at offset 0x{0:x8}.", overlay.TagOffset);
					}
				}
				_dicomPresentationImage.DicomGraphics.Add(_overlayPlanesGraphic);
			}
		}

		#region CompositeGraphic

		private class OverlayPlanesGraphic : CompositeGraphic {}

		#endregion

		#region IDicomOverlayPlanes Members

		public IEnumerator<DicomOverlayPlane> GetEnumerator()
		{
			return _overlayPlanes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		#region IList<OverlayPlane> Members

		public int IndexOf(DicomOverlayPlane overlayPlane)
		{
			return _overlayPlanes.IndexOf(overlayPlane);
		}

		public void Insert(int index, DicomOverlayPlane overlayPlane)
		{
			if (_overlayPlanes.Count != _overlayPlanesGraphic.Graphics.Count)
			{
				this.Add(overlayPlane);
				return;
			}

			_overlayPlanes.Insert(index, overlayPlane);
			_overlayPlanesGraphic.Graphics.Insert(index, overlayPlane.Graphic);
		}

		public void RemoveAt(int index)
		{
			if (_overlayPlanes.Count != _overlayPlanesGraphic.Graphics.Count)
			{
				this.Remove(_overlayPlanes[index]);
				return;
			}

			_overlayPlanes.RemoveAt(index);
			_overlayPlanesGraphic.Graphics.RemoveAt(index);
		}

		public DicomOverlayPlane this[int index]
		{
			get { return _overlayPlanes[index]; }
			set
			{
				this.RemoveAt(index);
				this.Insert(index, value);
			}
		}

		public void Add(DicomOverlayPlane overlayPlane)
		{
			_overlayPlanes.Add(overlayPlane);
			_overlayPlanesGraphic.Graphics.Add(overlayPlane.Graphic);
		}

		public void Clear()
		{
			_overlayPlanes.Clear();
			_overlayPlanesGraphic.Graphics.Clear();
		}

		public bool Contains(DicomOverlayPlane overlayPlane)
		{
			return _overlayPlanes.Contains(overlayPlane);
		}

		public void CopyTo(DicomOverlayPlane[] array, int arrayIndex)
		{
			_overlayPlanes.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _overlayPlanes.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(DicomOverlayPlane overlayPlane)
		{
			_overlayPlanesGraphic.Graphics.Remove(overlayPlane.Graphic);
			return _overlayPlanes.Remove(overlayPlane);
		}

		#endregion
	}
}