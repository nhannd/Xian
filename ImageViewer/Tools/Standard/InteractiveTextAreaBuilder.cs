#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	internal class InteractiveTextAreaBuilder : InteractiveTextGraphicBuilder
	{
		public InteractiveTextAreaBuilder(ITextGraphic textGraphic) : base(textGraphic) {}

		internal new ITextGraphic Graphic
		{
			get { return (ITextGraphic) base.Graphic; }
		}

		protected override ITextGraphic FindTextGraphic()
		{
			IGraphic graphic = this.Graphic;
			while (graphic != null && !(graphic is TextEditControlGraphic))
				graphic = graphic.ParentGraphic;
			return graphic as TextEditControlGraphic;
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
			this.Graphic.Location = mouseInformation.Location;
			this.Graphic.ResetCoordinateSystem();
			this.NotifyGraphicComplete();
			return true;
		}
	}
}