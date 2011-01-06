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

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A collection of <see cref="ITile"/> objects.
	/// </summary>
	public class TileCollection : ObservableList<ITile>
	{
		private bool _locked;

		/// <summary>
		/// Initializes a new instance of <see cref="TileCollection"/>.
		/// </summary>
		internal TileCollection()
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
		public override void Add(ITile item)
		{
			if (_locked)
				throw new InvalidOperationException("The tile collection is locked.");

			base.Add(item);
		}

		/// <summary>
		/// Inserts <paramref name="item"/> at the specified <paramref name="index"/>.
		/// </summary>
		public override void Insert(int index, ITile item)
		{
			if (_locked)
				throw new InvalidOperationException("The tile collection is locked.");
				
			base.Insert(index, item);
		}

		/// <summary>
		/// Removes the specified <paramref name="item"/> from the list.
		/// </summary>
		/// <returns>True if the item was in the list and was removed.</returns>
		public override bool Remove(ITile item)
		{
			if (_locked)
				throw new InvalidOperationException("The tile collection is locked.");
				
			return base.Remove(item);
		}

		/// <summary>
		/// Removes the item at the specified <paramref name="index"/>.
		/// </summary>
		public override void RemoveAt(int index)
		{
			if (_locked)
				throw new InvalidOperationException("The tile collection is locked.");
				
			base.RemoveAt(index);
		}

		/// <summary>
		/// Clears the list.
		/// </summary>
		public override void Clear()
		{
			if (_locked)
				throw new InvalidOperationException("The tile collection is locked.");
				
			base.Clear();
		}

		/// <summary>
		/// Gets or sets the item at the specified index.
		/// </summary>
		public override ITile this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				if (_locked)
					throw new InvalidOperationException("The tile collection is locked.");
					
				base[index] = value;
			}
		}
	}
}
