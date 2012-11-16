#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines a container for <see cref="IDisplaySet"/> objects.
	/// </summary>
	public interface IImageSet : IDrawable, IDisposable
	{
		/// <summary>
		/// Gets the associated <see cref="IImageViewer"/>.
		/// </summary>
		/// <value>The associated <see cref="IImageViewer"/> or <b>null</b> if the 
		/// <see cref="IImageSet"/> is not part of the 
		/// logical workspace yet.</value>
		IImageViewer ImageViewer { get; }

		/// <summary>
		/// Gets the parent <see cref="ILogicalWorkspace"/>
		/// </summary>
		/// <value>The parent <see cref="ILogicalWorkspace"/> or <b>null</b> if the 
		/// <see cref="IImageSet"/> has not been added to an 
		/// <see cref="ILogicalWorkspace"/> yet.</value>
		ILogicalWorkspace ParentLogicalWorkspace { get; }

		/// <summary>
		/// Gets the collection of <see cref="IDisplaySet"/> objects that belong
		/// to this <see cref="IImageSet"/>.
		/// </summary>
		DisplaySetCollection DisplaySets { get; }

		/// <summary>
		/// Gets a collection of linked <see cref="IDisplaySet"/> objects.
		/// </summary>
		IEnumerable<IDisplaySet> LinkedDisplaySets { get; }

		/// <summary>
		/// Gets the <see cref="IImageSetDescriptor"/> describing this <see cref="IImageSet"/>.
		/// </summary>
		IImageSetDescriptor Descriptor { get; }

		/// <summary>
		/// Gets the name of the image set.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the patient information associated with the image set.
		/// </summary>
		string PatientInfo { get; }

		/// <summary>
		/// Gets the unique identifier for this <see cref="IImageSet"/>.
		/// </summary>
		string Uid { get; }

        /// <summary>
        /// A place for extensions to store custom data about the image set.
        /// </summary>
        ExtensionData ExtensionData { get; }
    }
}
