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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Represents a collection of available display shutters.
	/// </summary>
	public interface IDicomGraphicsPlaneShutters : IList<IShutterGraphic>
	{
		void Activate(IShutterGraphic shutter);
		void Activate(int index);
		void ActivateFirst();

		void Deactivate(IShutterGraphic shutter);
		void Deactivate(int index);
		void DeactivateAll();

		IShutterGraphic ActiveShutter { get; }

		new IShutterGraphic this[int index] { get; }

		/// <summary>
		/// Enables or disables all display shutters.
		/// </summary>
		bool Enabled { get; set; }
	}

	public partial class DicomGraphicsPlane
	{
		[Cloneable(true)]
		private class ShutterCollection : CompositeGraphic, IDicomGraphicsPlaneShutters, IEnumerable<IGraphic>
		{
			public bool Enabled
			{
				get { return base.Visible; }
				set { base.Visible = value; }
			}

			public IShutterGraphic ActiveShutter
			{
				get { return CollectionUtils.SelectFirst(base.Graphics, delegate(IGraphic g) { return g.Visible; }) as IShutterGraphic; }
			}

			public IShutterGraphic this[int index]
			{
				get { return (IShutterGraphic) base.Graphics[index]; }
			}

			public int Count
			{
				get { return base.Graphics.Count; }
			}

			public void Activate(IShutterGraphic shutter)
			{
				Platform.CheckTrue(base.Graphics.Contains(shutter), "Shutter must be part of the collection.");
				foreach (IShutterGraphic graphic in this)
					graphic.Visible = (graphic == shutter);
			}

			public void Activate(int index)
			{
				Platform.CheckArgumentRange(index, 0, base.Graphics.Count - 1, "index");
				for (int n = 0; n < base.Graphics.Count; n++)
					base.Graphics[n].Visible = (n == index);
			}

			public void ActivateFirst()
			{
				if (base.Graphics.Count > 0)
					this.Activate(0);
			}

			public void Deactivate(IShutterGraphic shutter)
			{
				Platform.CheckTrue(base.Graphics.Contains(shutter), "Shutter must be part of the collection.");
				foreach (IShutterGraphic graphic in this)
					if (graphic == shutter)
						graphic.Visible = false;
			}

			public void Deactivate(int index)
			{
				Platform.CheckArgumentRange(index, 0, base.Graphics.Count - 1, "index");
				for (int n = 0; n < base.Graphics.Count; n++)
					if (n == index)
						base.Graphics[n].Visible = false;
			}

			public void DeactivateAll()
			{
				foreach (IShutterGraphic graphic in this)
					graphic.Visible = false;
			}

			public void Add(IShutterGraphic shutter)
			{
				base.Graphics.Add(shutter);
				shutter.Visible = false;
			}

			public bool Remove(IShutterGraphic shutter)
			{
				return base.Graphics.Remove(shutter);
			}

			public void Insert(int index, IShutterGraphic shutter)
			{
				base.Graphics.Insert(0, shutter);
				shutter.Visible = false;
			}

			public void RemoveAt(int index)
			{
				base.Graphics.RemoveAt(index);
			}

			public int IndexOf(IShutterGraphic shutter)
			{
				return base.Graphics.IndexOf(shutter);
			}

			public void Clear()
			{
				base.Graphics.Clear();
			}

			public bool Contains(IShutterGraphic shutter)
			{
				return base.Graphics.Contains(shutter);
			}

			public IEnumerator<IShutterGraphic> GetEnumerator()
			{
				foreach (IGraphic graphic in base.Graphics)
					yield return (IShutterGraphic) graphic;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			#region IList<IShutterGraphic> Members

			IShutterGraphic IList<IShutterGraphic>.this[int index]
			{
				get { return this[index]; }
				set { throw new NotSupportedException(); }
			}

			void ICollection<IShutterGraphic>.CopyTo(IShutterGraphic[] array, int arrayIndex)
			{
				foreach (IShutterGraphic graphic in this)
					array[arrayIndex++] = graphic;
			}

			bool ICollection<IShutterGraphic>.IsReadOnly
			{
				get { return false; }
			}

			#endregion

			#region IEnumerable<IGraphic> Members

			IEnumerator<IGraphic> IEnumerable<IGraphic>.GetEnumerator()
			{
				return base.Graphics.GetEnumerator();
			}

			#endregion
		}
	}
}