#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Utilities;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A collection of <see cref="IImageBox"/> objects.
	/// </summary>
	public class ImageBoxCollection : ObservableList<IImageBox>
	{
		private bool _locked;

		/// <summary>
		/// Initializes a new instance of <see cref="ImageBoxCollection"/>.
		/// </summary>
		internal ImageBoxCollection()
		{
		}

		/// <summary>
		/// Creates a copy of the object.
		/// </summary>
		/// <param name="collection"></param>
		/// <remarks>
		/// Creates a <i>shallow</i> copy.  That is, only references to objects
		/// in the collection are copied.
		/// </remarks>
		internal ImageBoxCollection(IEnumerable<IImageBox> collection)
			: base(collection)
		{
		}

		//NOTE: opted not to override base.IsReadOnly b/c the semantic is slightly different.
		//This is not a 'read only' list, rather the parent object has temporarily locked it.
		internal bool Locked
		{
			get { return _locked; }
			set { _locked = value; }
		}

		/// <summary>
		/// Adds the specified item to the list.
		/// </summary>
		public override void Add(IImageBox item)
		{
			if (_locked)
				throw new InvalidOperationException("The image box collection is locked.");
				
			base.Add(item);
		}

		/// <summary>
		/// Inserts <paramref name="item"/> at the specified <paramref name="index"/>.
		/// </summary>
		public override void Insert(int index, IImageBox item)
		{
			if (_locked)
				throw new InvalidOperationException("The image box collection is locked.");

			base.Insert(index, item);
		}

		/// <summary>
		/// Removes the specified <paramref name="item"/> from the list.
		/// </summary>
		/// <returns>True if the item was in the list and was removed.</returns>
		public override bool Remove(IImageBox item)
		{
			if (_locked)
				throw new InvalidOperationException("The image box collection is locked.");
				
			return base.Remove(item);
		}

		/// <summary>
		/// Removes the item at the specified <paramref name="index"/>.
		/// </summary>
		public override void RemoveAt(int index)
		{
			if (_locked)
				throw new InvalidOperationException("The image box collection is locked.");

			base.RemoveAt(index);
		}

		/// <summary>
		/// Clears the list.
		/// </summary>
		public override void Clear()
		{
			if (_locked)
				throw new InvalidOperationException("The image box collection is locked.");

			base.Clear();
		}

		/// <summary>
		/// Gets or sets the item at the specified index.
		/// </summary>
		public override IImageBox this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				if (_locked)
					throw new InvalidOperationException("The image box collection is locked.");

				base[index] = value;
			}
		}
	}
}
