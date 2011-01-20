#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A strategy for determining what the <see cref="CursorToken"/> should be
	/// for a given <see cref="TargetGraphic"/>.
	/// </summary>
	[Cloneable(true)]
	public abstract class StretchCursorTokenStrategy : ICursorTokenProvider
	{
		[CloneIgnore]
		private IGraphic _targetGraphic;

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected StretchCursorTokenStrategy()
		{
		}

		/// <summary>
		/// The target <see cref="Graphic"/> for which the <see cref="CursorToken"/>
		/// is to be determined.
		/// </summary>
		public IGraphic TargetGraphic
		{
			get { return _targetGraphic; }
			set { _targetGraphic = value; }
		}

		#region ICursorTokenProvider Members

		/// <summary>
		/// Gets the cursor token to be shown at the current mouse position.
		/// </summary>
		public abstract CursorToken GetCursorToken(Point point);

		#endregion
	}
}
