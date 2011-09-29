#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Text;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Interactive builder class that interprets mouse clicks as ordered
	/// vertices to setup an open <see cref="IPointsGraphic"/>.
	/// </summary>
	/// <remarks>
	/// This builder takes input until the maximum number of vertices is reached.
	/// </remarks>
	public class InteractivePolylineGraphicBuilder : InteractiveGraphicBuilder
	{
		private readonly int _maximumVertices;
		private int _numberOfPointsAnchored = 0;

		/// <summary>
		/// Constructs an interactive builder for the specified graphic.
		/// </summary>
		/// <param name="pointsGraphic">The graphic to be interactively built.</param>
		public InteractivePolylineGraphicBuilder(IPointsGraphic pointsGraphic)
			: this(int.MaxValue, pointsGraphic) {}

		/// <summary>
		/// Constructs an interactive builder for the specified graphic.
		/// </summary>
		/// <param name="maximumVertices">The maximum number of vertices to accept.</param>
		/// <param name="pointsGraphic">The graphic to be interactively built.</param>
		public InteractivePolylineGraphicBuilder(int maximumVertices, IPointsGraphic pointsGraphic)
			: base(pointsGraphic)
		{
			_maximumVertices = maximumVertices;
		}

        public bool StopOnDoubleClick { get; set; }

		/// <summary>
		/// Gets the graphic that the builder is operating on.
		/// </summary>
		public new IPointsGraphic Graphic
		{
			get { return (IPointsGraphic) base.Graphic; }
		}

		/// <summary>
		/// Resets any internal state of the builder, allowing the same graphic to be rebuilt.
		/// </summary>
		public override void Reset()
		{
			_numberOfPointsAnchored = 0;
			base.Reset();
		}

		/// <summary>
		/// Rolls back the internal state of the builder by one mouse click, allowing the same graphic to be rebuilt by resuming from an earlier state.
		/// </summary>
		protected override void Rollback()
		{
			_numberOfPointsAnchored = Math.Max(_numberOfPointsAnchored - 1, 0);
		}

		/// <summary>
		/// Passes user input to the builder when <see cref="IMouseButtonHandler.Start"/> is called on the owning tool.
		/// </summary>
		/// <param name="mouseInformation">The user input data.</param>
		/// <returns>True if the builder did something as a result of the call, and hence would like to receive capture; False otherwise.</returns>
		public override bool Start(IMouseInformation mouseInformation)
		{
			_numberOfPointsAnchored++;

			// We just started creating
			if (_numberOfPointsAnchored == 1)
			{
				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				this.Graphic.Points.Add(mouseInformation.Location);
				this.Graphic.Points.Add(mouseInformation.Location);
				this.Graphic.ResetCoordinateSystem();
			}
			// We're done creating
			else if (_numberOfPointsAnchored == _maximumVertices || mouseInformation.ClickCount == 2 && StopOnDoubleClick)
			{
                // When user moves very quickly and events are filtered for performance purpose (eg web viewer case), 
                // the final point may not be the same as the last tracked point. Must update the final point based on the latest mouse position.
                this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
			    this.Graphic.Points[this.Graphic.Points.Count-1] = mouseInformation.Location;
                this.Graphic.ResetCoordinateSystem();

				this.NotifyGraphicComplete();
			}
			// We're in the middle of creating
			else if (_numberOfPointsAnchored >= 2 && _maximumVertices > 2)
			{
				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;

                // Update the final position of current point based on the latest mouse position.
                this.Graphic.Points[Graphic.Points.Count-1] = mouseInformation.Location;

                // Add a new point for tracking
                this.Graphic.Points.Add(mouseInformation.Location);
				this.Graphic.ResetCoordinateSystem();
			}

			return true;
		}

		/// <summary>
		/// Passes user input to the builder when <see cref="IMouseButtonHandler.Track"/> is called on the owning tool.
		/// </summary>
		/// <param name="mouseInformation">The user input data.</param>
		/// <returns>True if the builder handled the message; False otherwise.</returns>
		public override bool Track(IMouseInformation mouseInformation)
		{
			this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
			this.Graphic.Points[_numberOfPointsAnchored] = mouseInformation.Location;
			this.Graphic.ResetCoordinateSystem();
			this.Graphic.Draw();

			return true;
		}

		/// <summary>
		/// Passes user input to the builder when <see cref="IMouseButtonHandler.Stop"/> is called on the owning tool.
		/// </summary>
		/// <param name="mouseInformation">The user input data.</param>
		/// <returns>True if the tool should not release capture; False otherwise.</returns>
		public override bool Stop(IMouseInformation mouseInformation)
		{
			return true;
		}
	}
}