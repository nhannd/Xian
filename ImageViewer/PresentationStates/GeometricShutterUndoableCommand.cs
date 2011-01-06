#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	/// <summary>
	/// <see cref="UndoableCommand"/> for adding a <see cref="GeometricShutter"/> to a
	/// <see cref="GeometricShuttersGraphic"/>.
	/// </summary>
	public class AddGeometricShutterUndoableCommand : UndoableCommand
	{
		private readonly GeometricShutter _shutter;
		private readonly GeometricShuttersGraphic _parent;

		/// <summary>
		/// Constructs a new <see cref="AddGeometricShutterUndoableCommand"/>.
		/// </summary>
		/// <param name="parent">The parent <see cref="GeometricShuttersGraphic"/>.</param>
		/// <param name="shutter">The <see cref="GeometricShutter"/> to add to <paramref name="parent"/>.</param>
		public AddGeometricShutterUndoableCommand(GeometricShuttersGraphic parent, GeometricShutter shutter)
		{
			_parent = parent;
			_shutter = shutter;
		}

		/// <summary>
		/// Removes the <see cref="GeometricShutter"/> from the parent <see cref="GeometricShuttersGraphic"/>.
		/// </summary>
		public override void Unexecute()
		{
			if (_parent.CustomShutters.Contains(_shutter))
				_parent.CustomShutters.Remove(_shutter);
		}

		/// <summary>
		/// Adds the <see cref="GeometricShutter"/> to the parent <see cref="GeometricShuttersGraphic"/>.
		/// </summary>
		public override void Execute()
		{
			if (!_parent.CustomShutters.Contains(_shutter))
				_parent.CustomShutters.Add(_shutter);
		}
	}

	/// <summary>
	/// <see cref="UndoableCommand"/> for removing a <see cref="GeometricShutter"/> from a
	/// <see cref="GeometricShuttersGraphic"/>.
	/// </summary>
	public class RemoveGeometricShutterUndoableCommand : UndoableCommand
	{
		private readonly GeometricShutter _shutter;
		private readonly GeometricShuttersGraphic _parent;

		/// <summary>
		/// Constructs a new <see cref="RemoveGeometricShutterUndoableCommand"/>.
		/// </summary>
		/// <param name="parent">The parent <see cref="GeometricShuttersGraphic"/>.</param>
		/// <param name="shutter">The <see cref="GeometricShutter"/> to add to <paramref name="parent"/>.</param>
		public RemoveGeometricShutterUndoableCommand(GeometricShuttersGraphic parent, GeometricShutter shutter)
		{
			_parent = parent;
			_shutter = shutter;
		}

		/// <summary>
		/// Adds the <see cref="GeometricShutter"/> to the parent <see cref="GeometricShuttersGraphic"/>.
		/// </summary>
		public override void Unexecute()
		{
			if (!_parent.CustomShutters.Contains(_shutter))
				_parent.CustomShutters.Add(_shutter);
		}

		/// <summary>
		/// Removes the <see cref="GeometricShutter"/> from the parent <see cref="GeometricShuttersGraphic"/>.
		/// </summary>
		public override void Execute()
		{
			if (_parent.CustomShutters.Contains(_shutter))
				_parent.CustomShutters.Remove(_shutter);
		}
	}
}