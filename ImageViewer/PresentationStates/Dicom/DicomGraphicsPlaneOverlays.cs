using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	public interface IDicomGraphicsPlaneOverlays : IEnumerable<OverlayPlaneGraphic>
	{
		OverlayPlaneGraphic this[int index] { get; }
		int Count { get; }
		void ActivateAsLayer(OverlayPlaneGraphic overlay, string layerId);
		void ActivateAsLayer(int index, string layerId);
		void ActivateAsShutter(OverlayPlaneGraphic overlay);
		void ActivateAsShutter(int index);
		void Deactivate(OverlayPlaneGraphic overlay);
		void Deactivate(int index);
		void Add(OverlayPlaneGraphic overlay);
		void Remove(OverlayPlaneGraphic overlay);
		bool Contains(int index);
		bool Contains(OverlayPlaneGraphic overlay);
		void Clear();
		bool IsShutter(OverlayPlaneGraphic overlay);
		bool IsLayer(OverlayPlaneGraphic overlay);
		string GetLayer(OverlayPlaneGraphic overlay);
	}

	public partial class DicomGraphicsPlane
	{
		private class OverlaysCollection : IDicomGraphicsPlaneOverlays
		{
			private readonly DicomGraphicsPlane _owner;
			private readonly OverlayPlaneGraphic[] _overlays;

			public OverlaysCollection(DicomGraphicsPlane owner)
			{
				_owner = owner;
				_overlays = new OverlayPlaneGraphic[16];
			}

			public int Count
			{
				get
				{
					int count = 0;
					foreach (OverlayPlaneGraphic overlay in this)
						count++;
					return count;
				}
			}

			public void ActivateAsLayer(OverlayPlaneGraphic overlay, string layerId)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(_overlays[overlay.Index] == overlay, "Overlay must be part of collection.");
				ActivateAsLayer(overlay.Index, layerId);
			}

			public void ActivateAsLayer(int index, string layerId)
			{
				Platform.CheckArgumentRange(index, 0, 15, "index");
				OverlayPlaneGraphic overlay = _overlays[index];
				if (overlay != null)
				{
					if (overlay.ParentGraphic is ShutterCollection)
					{
						ShutterCollection shutterCollection = (ShutterCollection) overlay.ParentGraphic;
						shutterCollection.Deactivate(overlay);
						shutterCollection.Graphics.Remove(overlay);
					}

					ILayer layer = _owner.Layers[layerId];
					if (overlay.ParentGraphic is CompositeGraphic && overlay.ParentGraphic != layer)
						((CompositeGraphic)overlay.ParentGraphic).Graphics.Remove(overlay);
					if (overlay.ParentGraphic == null)
						layer.Graphics.Add(overlay);
					overlay.Visible = true;
				}
			}

			public void ActivateAsShutter(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(_overlays[overlay.Index] == overlay, "Overlay must be part of collection.");
				ActivateAsShutter(overlay.Index);
			}

			public void ActivateAsShutter(int index)
			{
				Platform.CheckArgumentRange(index, 0, 15, "index");
				OverlayPlaneGraphic overlay = _overlays[index];
				if (overlay != null)
				{
					if (overlay.ParentGraphic is CompositeGraphic && overlay.ParentGraphic != _owner.Shutters)
						((CompositeGraphic)overlay.ParentGraphic).Graphics.Remove(overlay);
					if (overlay.ParentGraphic == null)
						_owner.Shutters.Add(overlay);
					_owner.Shutters.Activate(overlay);
				}
				else
				{
					_owner.Shutters.DeactivateAll();
				}
			}

			public void Deactivate(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(_overlays[overlay.Index] == overlay, "Overlay must be part of collection.");
				Deactivate(overlay.Index);
			}

			public void Deactivate(int index)
			{
				Platform.CheckArgumentRange(index, 0, 15, "index");
				OverlayPlaneGraphic overlay = _overlays[index];
				if (overlay != null)
				{
					if (overlay.ParentGraphic == _owner.Shutters)
						_owner.Shutters.Deactivate(overlay);
					if (overlay.ParentGraphic is LayerGraphic && _owner._layers.Graphics.Contains(overlay.ParentGraphic))
						this.ActivateAsLayer(index, string.Empty);
				}
			}

			public OverlayPlaneGraphic this[int index]
			{
				get
				{
					Platform.CheckArgumentRange(index, 0, 15, "index");
					return _overlays[index];
				}
			}

			public void Add(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(CanAdd(overlay), "Overlay is already part of a different collection.");
				if(overlay.ParentGraphic == null)
					_owner.Layers.InactiveLayer.Graphics.Add(overlay);
				_overlays[overlay.Index] = overlay;
			}

			public void Remove(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(_overlays[overlay.Index] == overlay, "Overlay must be part of collection.");
				_overlays[overlay.Index] = null;
			}

			public bool Contains(int index)
			{
				Platform.CheckArgumentRange(index, 0, 15, "index");
				return _overlays[index] != null;
			}

			public bool Contains(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				return _overlays[overlay.Index] == overlay;
			}

			public void Clear()
			{
				_overlays[0] = null;
				_overlays[1] = null;
				_overlays[2] = null;
				_overlays[3] = null;
				_overlays[4] = null;
				_overlays[5] = null;
				_overlays[6] = null;
				_overlays[7] = null;
				_overlays[8] = null;
				_overlays[9] = null;
				_overlays[10] = null;
				_overlays[11] = null;
				_overlays[12] = null;
				_overlays[13] = null;
				_overlays[14] = null;
				_overlays[15] = null;
			}

			public bool IsShutter(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(_overlays[overlay.Index] == overlay, "Overlay must be part of collection.");
				return (overlay.ParentGraphic == _owner.Shutters);
			}

			public bool IsLayer(OverlayPlaneGraphic overlay)
			{
				return !string.IsNullOrEmpty(GetLayer(overlay));
			}

			public string GetLayer(OverlayPlaneGraphic overlay)
			{
				Platform.CheckForNullReference(overlay, "overlay");
				Platform.CheckTrue(_overlays[overlay.Index] == overlay, "Overlay must be part of collection.");
				if (overlay.ParentGraphic is ILayer && overlay.ParentGraphic.ParentGraphic == _owner.Layers)
					return ((ILayer) overlay.ParentGraphic).Id;
				return null;
			}

			public IEnumerator<OverlayPlaneGraphic> GetEnumerator()
			{
				foreach (OverlayPlaneGraphic overlay in _overlays)
					if (overlay != null)
						yield return overlay;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			private bool CanAdd(OverlayPlaneGraphic overlay)
			{
				if (overlay.ParentGraphic == null)
					// The overlay has no current parent
					return true;

				if (overlay.ParentGraphic is LayerGraphic)
					// The owning DicomGraphicsPlane should be the parent of the LayerCollection that is the parent of the LayerGraphic
					return (overlay.ParentGraphic.ParentGraphic is LayerCollection && overlay.ParentGraphic.ParentGraphic.ParentGraphic == _owner);

				if (overlay.ParentGraphic is ShutterCollection)
					// The owning DicomGraphicsPlane should be the parent of the ShutterCollection that is the parent of the overlay
					return (overlay.ParentGraphic.ParentGraphic == _owner);

				return false;
			}
		}
	}
}