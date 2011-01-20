#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Provides data for the <see cref="StatefulCompositeGraphic.StateChanged"/>
	/// event.
	/// </summary>
	public class GraphicStateChangedEventArgs : EventArgs
	{
		private IStatefulGraphic _statefulGraphic;
		private GraphicState _oldState;
		private GraphicState _newState;
		private IMouseInformation _mouseInformation;

		/// <summary>
		/// Constructor.
		/// </summary>
		public GraphicStateChangedEventArgs()
		{

		}

		/// <summary>
		/// Gets the <see cref="IStatefulGraphic"/>.
		/// </summary>
		public IStatefulGraphic StatefulGraphic
		{
			get { return _statefulGraphic; }
			set { _statefulGraphic = value; }
		}

		/// <summary>
		/// Gets the old <see cref="GraphicState"/>.
		/// </summary>
		public GraphicState OldState
		{
			get { return _oldState; }
			set { _oldState = value; }
		}

		/// <summary>
		/// Gets the new <see cref="GraphicState"/>.
		/// </summary>
		public GraphicState NewState
		{
			get { return _newState; }
			set 
			{
				Platform.CheckForNullReference(value, "NewState");
				_newState = value; 
			}
		}

		/// <summary>
		/// Gets the <see cref="IMouseInformation"/>.
		/// </summary>
		public IMouseInformation MouseInformation
		{
			get { return _mouseInformation; }
			set { _mouseInformation = value; }
		}
	}
}
