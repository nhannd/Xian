#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	// TODO (later): see if there's a way to cleanly get rid of this and/or 
	// put the delegates into one of the superclasses so it can be used on something other than just images.
	// For now, though, just leave it, since it's code that's already been released.

	/// <summary>
	/// A simple way to implement an ImageOperation, using delegates.
	/// </summary>
	public class BasicImageOperation : ImageOperation
	{
		/// <summary>
		/// Defines a delegate used to get the originator for a given <see cref="IPresentationImage"/>.
		/// </summary>
		public delegate IMemorable GetOriginatorDelegate(IPresentationImage image);

		/// <summary>
		/// Defines a delegate used to apply an undoable operation to an <see cref="IPresentationImage"/>.
		/// </summary>
		public delegate void ApplyDelegate(IPresentationImage image);

		private readonly GetOriginatorDelegate _getOriginatorDelegate;
		private readonly ApplyDelegate _applyDelegate;

		/// <summary>
		/// Mandatory constructor.
		/// </summary>
		public BasicImageOperation(GetOriginatorDelegate getOriginatorDelegate, ApplyDelegate applyDelegate)
		{
			Platform.CheckForNullReference(getOriginatorDelegate, "getOriginatorDelegate");
			Platform.CheckForNullReference(applyDelegate, "applyDelegate");

			_getOriginatorDelegate = getOriginatorDelegate;
			_applyDelegate = applyDelegate;
		}

		/// <summary>
		/// Gets the originator for the input <see cref="IPresentationImage"/>, which must be <see cref="IMemorable"/>.
		/// </summary>
		public override IMemorable GetOriginator(IPresentationImage image)
		{
			return _getOriginatorDelegate(image);
		}

		/// <summary>
		/// Applies the operation to the input <see cref="IPresentationImage"/>.
		/// </summary>
		public sealed override void Apply(IPresentationImage image)
		{
			_applyDelegate(image);
		}
	}
}
