using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	[MouseToolButton(XMouseButtons.Left, false)]
	[MenuAction("activate", "imageviewer-contextmenu/Rotate Oblique", "Apply", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarsMpr/Rotate Oblique", "Apply", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[VisibleStateObserver("activate", "Visible", "VisibleChanged")]

	[GroupHint("activate", "Tools.Mpr.Manipulation.Rotate")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class RotateObliqueTool : MouseImageViewerTool
	{
		private PinwheelGraphic _currentPinwheelGraphic;
		private bool _rotatingGraphic = false;
		private int _rotationAxis = -1;

		private bool _visible;
		public event EventHandler VisibleChanged;	

		public RotateObliqueTool()
		{
		}

		public bool Visible
		{
			get { return _visible; }
			set
			{
				if (_visible != value)
				{
					_visible = value;
					EventsHelper.Fire(VisibleChanged, this, EventArgs.Empty);
				}
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			Visible = GetMprLayoutManager() != null;
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			_rotatingGraphic = false;

			if (_currentPinwheelGraphic != null)
			{
				_currentPinwheelGraphic.CoordinateSystem = CoordinateSystem.Destination;
				_rotatingGraphic = _currentPinwheelGraphic.HitTest(mouseInformation.Location);
				_currentPinwheelGraphic.ResetCoordinateSystem();
			}

			return _rotatingGraphic;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			return _rotatingGraphic = false;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_rotationAxis < 0)
				return base.Track(mouseInformation);

			if (_rotatingGraphic)
			{
				_currentPinwheelGraphic.CoordinateSystem = CoordinateSystem.Destination;
				PointF rotationAnchor = _currentPinwheelGraphic.RotationAnchor;
				PointF vertex = _currentPinwheelGraphic.Anchor;
				PointF mouse = mouseInformation.Location;
				double angle = Vector.SubtendedAngle(mouse, vertex, rotationAnchor);

				int rotationX, rotationY, rotationZ;
				GetRotationAngles(out rotationX, out rotationY, out rotationZ);

				if (_rotationAxis == 0)
				{
					rotationX += (int)angle;
					_currentPinwheelGraphic.Rotation = rotationX;
				}
				else if (_rotationAxis == 1)
				{
					rotationY += (int)angle;
					_currentPinwheelGraphic.Rotation = rotationY;
				}
				else
				{
					rotationZ += (int)angle;
					_currentPinwheelGraphic.Rotation = rotationZ;
				}

				_currentPinwheelGraphic.ResetCoordinateSystem();
				_currentPinwheelGraphic.Draw();

				GetMprLayoutManager().RotateObliqueImage(GetObliqueImage(), rotationX, rotationY, rotationZ);
				return true;
			}

			return false;
		}

		private void OnActivationChanged(object sender, System.EventArgs e)
		{
			base.ActivationChanged -= new System.EventHandler(OnActivationChanged);
			RemoveGraphic();
			UpdateRotationAxis();
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			RemoveGraphic();
			UpdateRotationAxis();

			if (Visible && Active)
			{
				AddPinwheelGraphic();
			}
		}

		public void Apply()
		{
			if (Active)
			{
				Active = false;
			}
			else if (Visible)
			{
				base.Select();

				base.ActivationChanged += new System.EventHandler(OnActivationChanged);

				UpdateRotationAxis();
				AddPinwheelGraphic();
			}
		}

		private void AddPinwheelGraphic()
		{
			IPresentationImage selectedImage = base.SelectedPresentationImage;
			if (selectedImage == null)
				return;

			IOverlayGraphicsProvider overlayProvider = selectedImage as IOverlayGraphicsProvider;
			IImageGraphicProvider imageGraphicProvider = selectedImage as IImageGraphicProvider;
			
			if (overlayProvider != null && imageGraphicProvider != null)
			{
				_currentPinwheelGraphic = new PinwheelGraphic();

				int width = imageGraphicProvider.ImageGraphic.Columns;
				int height = imageGraphicProvider.ImageGraphic.Rows;

				overlayProvider.OverlayGraphics.Add(_currentPinwheelGraphic);
				_currentPinwheelGraphic.CoordinateSystem = CoordinateSystem.Source;
				_currentPinwheelGraphic.Rotation = GetRotationAngle();
				_currentPinwheelGraphic.Draw();
			}
		}

		private void UpdateRotationAxis()
		{
			IPresentationImage selectedImage = base.Context.Viewer.SelectedPresentationImage;
			MprLayoutManager layoutManager = GetMprLayoutManager();
			_rotationAxis = -1;

			if (selectedImage != null && layoutManager != null)
			{
				if (IsSaggittalImage(selectedImage))
					_rotationAxis = 0; //x
				else if (IsCoronalImage(selectedImage))
					_rotationAxis = 1; //y
				else if (IsAxialImage(selectedImage))
					_rotationAxis = 2; //z
			}
		}

		private void RemoveGraphic()
		{
			if (_currentPinwheelGraphic != null)
			{
				IPresentationImage image = _currentPinwheelGraphic.ParentPresentationImage;
				((CompositeGraphic)_currentPinwheelGraphic.ParentGraphic).Graphics.Remove(_currentPinwheelGraphic);
				image.Draw();

				_currentPinwheelGraphic.Dispose();
			}

			_currentPinwheelGraphic = null;
		}

		private IPresentationImage GetObliqueImage()
		{
			return ImageViewer.PhysicalWorkspace.ImageBoxes[3].TopLeftPresentationImage;
		}

		private int GetRotationAngle()
		{
			int rotationX, rotationY, rotationZ;
			GetRotationAngles(out rotationX, out rotationY, out rotationZ);

			if (_rotationAxis == 0)
				return rotationX;
			else if (_rotationAxis == 1)
				return rotationY;
			else if (_rotationAxis == 2)
				return rotationZ;

			return 0;
		}

		private void GetRotationAngles(out int rotationX, out int rotationY, out int rotationZ)
		{
			rotationX = 0;
			rotationY = 0;
			rotationZ = 0;

			IPresentationImage obliqueImage = GetObliqueImage();
			MprLayoutManager layoutManager = GetMprLayoutManager();

			if (obliqueImage != null && layoutManager != null)
			{
				rotationX = layoutManager.GetObliqueImageRotationX(obliqueImage);
				rotationY = layoutManager.GetObliqueImageRotationY(obliqueImage);
				rotationZ = layoutManager.GetObliqueImageRotationZ(obliqueImage);
			}
		}

		private MprLayoutManager GetMprLayoutManager()
		{
			if (base.Context.Viewer is MprImageViewerComponent)
				return ((MprImageViewerComponent)base.Context.Viewer).LayoutManager as MprLayoutManager;

			return null;
		}

		private bool IsAxialImage(IPresentationImage image)
		{
			return image.ParentDisplaySet.ImageBox == GetAxialImageBox();
		}

		private bool IsCoronalImage(IPresentationImage image)
		{
			return image.ParentDisplaySet.ImageBox == GetCoronalImageBox();
		}
		
		private bool IsSaggittalImage(IPresentationImage image)
		{
			return image.ParentDisplaySet.ImageBox == GetSagittalImageBox();
		}

		private IImageBox GetSagittalImageBox()
		{
			IPhysicalWorkspace workspace = base.Context.Viewer.PhysicalWorkspace;
			return workspace.ImageBoxes[0];
		}

		private IImageBox GetCoronalImageBox()
		{
			IPhysicalWorkspace workspace = base.Context.Viewer.PhysicalWorkspace;
			return workspace.ImageBoxes[1];
		}

		private IImageBox GetAxialImageBox()
		{
			IPhysicalWorkspace workspace = base.Context.Viewer.PhysicalWorkspace;
			return workspace.ImageBoxes[2];
		}
	}
}
