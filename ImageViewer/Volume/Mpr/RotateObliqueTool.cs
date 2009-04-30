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
	[MenuAction("activate", "imageviewer-contextmenu/MenuVolume/Rotate Oblique", "Apply", Flags = ClickActionFlags.CheckAction)]
	//[ButtonAction("activate", "global-toolbars/ToolbarsMpr/Rotate Oblique", "Apply", Flags = ClickActionFlags.CheckAction)]
	[IconSet("activate", IconScheme.Colour, "Icons.RotateObliqueToolLarge.png", "Icons.RotateObliqueToolMedium.png", "Icons.RotateObliqueToolSmall.png")]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[VisibleStateObserver("activate", "Visible", "VisibleChanged")]

	[GroupHint("activate", "Tools.Mpr.Manipulation.Rotate")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class RotateObliqueTool : MouseImageViewerTool
	{
		private MprImageViewerToolHelper _toolHelper;

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

			_toolHelper = new MprImageViewerToolHelper(Context);

			Visible = _toolHelper.GetMprLayoutManager() != null;
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

				MprDisplaySet obliqueDisplaySet = _toolHelper.GetObliqueDisplaySet();
				int rotationX = obliqueDisplaySet.RotateAboutX;
				int rotationY = obliqueDisplaySet.RotateAboutY;
				int rotationZ = obliqueDisplaySet.RotateAboutZ;

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

				obliqueDisplaySet.Rotate(rotationX, rotationY, rotationZ);
				return true;
			}

			return false;
		}

		private void OnActivationChanged(object sender, System.EventArgs e)
		{
			base.ActivationChanged -= new System.EventHandler(OnActivationChanged);
			RemovePinwheelGraphic();
			UpdateRotationAxis();
		}

		protected override void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			RemovePinwheelGraphic();
			UpdateRotationAxis();
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			RemovePinwheelGraphic();
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

			if (!_toolHelper.IsMprImage(selectedImage))
				return;

			MprDisplaySet displaySet = (MprDisplaySet)selectedImage.ParentDisplaySet;
			if (displaySet.Identifier == MprDisplaySetIdentifier.Oblique)
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

		private void RemovePinwheelGraphic()
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

		private void UpdateRotationAxis()
		{
			_rotationAxis = -1;

			IPresentationImage selectedImage = base.Context.Viewer.SelectedPresentationImage;
			if (selectedImage == null)
				return;

			MprDisplaySet displaySet = selectedImage.ParentDisplaySet as MprDisplaySet;
			if (displaySet == null)
				return;

			if (displaySet.Identifier == MprDisplaySetIdentifier.Identity)
				_rotationAxis = 0; //x
			else if (displaySet.Identifier == MprDisplaySetIdentifier.OrthoX)
				_rotationAxis = 1; //y
			else if (displaySet.Identifier == MprDisplaySetIdentifier.OrthoY)
				_rotationAxis = 2; //z
		}

		private int GetRotationAngle()
		{
			MprDisplaySet obliqueDisplaySet = _toolHelper.GetObliqueDisplaySet();
			int rotationX = obliqueDisplaySet.RotateAboutX;
			int rotationY = obliqueDisplaySet.RotateAboutY;
			int rotationZ = obliqueDisplaySet.RotateAboutZ;

			if (_rotationAxis == 0)
				return rotationX;
			else if (_rotationAxis == 1)
				return rotationY;
			else if (_rotationAxis == 2)
				return rotationZ;

			return 0;
		}
	}
}
