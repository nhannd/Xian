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
	public interface IDicomGraphicsPlaneLayers : IList<string>, IEnumerable<LayerGraphic>
	{
		LayerGraphic InactiveLayer { get; }
		LayerGraphic this[string layerId] { get; }
		LayerGraphic this[int index] { get; }
		new LayerGraphic Add(string layerId);
		new LayerGraphic Insert(int index, string layerId);
	}

	public partial class DicomGraphicsPlane
	{
		[Cloneable(true)]
		private class LayerCollection : CompositeGraphic, IDicomGraphicsPlaneLayers
		{
			[CloneIgnore]
			private readonly Dictionary<string, LayerGraphic> _layers = new Dictionary<string, LayerGraphic>();

			[OnCloneComplete]
			private void OnCloneComplete()
			{
				foreach (LayerGraphic graphic in base.Graphics)
					_layers.Add(graphic.Id, graphic);
			}

			public LayerGraphic InactiveLayer
			{
				get { return this[string.Empty]; }
			}

			public LayerGraphic this[string layerId]
			{
				get
				{
					layerId = layerId.ToUpperInvariant();
					if (!_layers.ContainsKey(layerId))
						return this.Add(layerId);
					return _layers[layerId];
				}
			}

			public LayerGraphic this[int index]
			{
				get
				{
					Platform.CheckArgumentRange(index, 0, _layers.Count - 1, "index");
					return (LayerGraphic) base.Graphics[index];
				}
			}

			public int Count
			{
				get { return _layers.Count; }
			}

			public bool Contains(string layerId)
			{
				layerId = layerId.ToUpperInvariant();
				return _layers.ContainsKey(layerId);
			}

			public LayerGraphic Add(string layerId)
			{
				layerId = layerId.ToUpperInvariant();
				if (_layers.ContainsKey(layerId))
					return _layers[layerId];



				LayerGraphic layer = new LayerGraphic(layerId);
				_layers.Add(layerId, layer);
				base.Graphics.Add(layer);
				return layer;
			}

			public bool Remove(string layerId)
			{
				layerId = layerId.ToUpperInvariant();
				if (!_layers.ContainsKey(layerId))
					return false;

				base.Graphics.Remove(_layers[layerId]);
				_layers.Remove(layerId);
				return true;
			}

			public LayerGraphic Insert(int index, string layerId)
			{
				Platform.CheckArgumentRange(index, 0, _layers.Count, "index");

				layerId = layerId.ToUpperInvariant();
				if (_layers.ContainsKey(layerId))
					return _layers[layerId];

				LayerGraphic layer = new LayerGraphic(layerId);
				_layers.Add(layerId, layer);
				base.Graphics.Insert(index, layer);
				return layer;
			}

			public void RemoveAt(int index)
			{
				Platform.CheckArgumentRange(index, 0, _layers.Count - 1, "index");
				LayerGraphic layer = (LayerGraphic) base.Graphics[index];
				_layers.Remove(layer.Id);
				base.Graphics.Remove(layer);
			}

			public int IndexOf(string layerId)
			{
				layerId = layerId.ToUpperInvariant();
				if (!_layers.ContainsKey(layerId))
					return -1;

				return base.Graphics.IndexOf(_layers[layerId]);
			}

			public void Clear()
			{
				_layers.Clear();
				base.Graphics.Clear();
			}

			public IEnumerator<LayerGraphic> GetEnumerator()
			{
				for (int n = 0; n < base.Graphics.Count; n++)
					yield return (LayerGraphic) base.Graphics[n];
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			#region IList<string> Members

			string IList<string>.this[int index]
			{
				get { return this[index].Id; }
				set { throw new NotSupportedException("Renaming layers is not supported at this time."); }
			}

			void ICollection<string>.Add(string layerId)
			{
				this.Add(layerId);
			}

			void IList<string>.Insert(int index, string layerId)
			{
				this.Insert(index, layerId);
			}

			void ICollection<string>.CopyTo(string[] array, int arrayIndex)
			{
				foreach (string layerId in (IEnumerable<string>) this)
					array[arrayIndex++] = layerId;
			}

			bool ICollection<string>.IsReadOnly
			{
				get { return false; }
			}

			IEnumerator<string> IEnumerable<string>.GetEnumerator()
			{
				for (int n = 0; n < base.Graphics.Count; n++)
					yield return ((LayerGraphic) base.Graphics[n]).Id;
			}

			#endregion
		}
	}

	[Cloneable]
	public sealed class LayerGraphic : CompositeGraphic
	{
		[CloneIgnore]
		private readonly string _id;

		private int[] _displayCIELabColor;
		private int? _displayGrayscaleColor;
		private string _description;

		internal LayerGraphic(string id)
		{
			_id = id.ToUpperInvariant();
			this.Visible = true;
		}

		internal LayerGraphic(LayerGraphic source, ICloningContext context)
		{
			context.CloneFields(source, this);
			_id = source._id;
		}

		public string Id
		{
			get { return _id; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public int? DisplayGrayscaleColor
		{
			get { return _displayGrayscaleColor; }
			set { _displayGrayscaleColor = value; }
		}

		public int[] DisplayCIELabColor
		{
			get { return _displayCIELabColor; }
			set { _displayCIELabColor = value; }
		}

		public new DicomGraphicsPlane ParentGraphic
		{
			get
			{
				IGraphic layerCollection = base.ParentGraphic;
				if (layerCollection == null)
					return null;
				return (DicomGraphicsPlane) layerCollection.ParentGraphic;
			}
		}

		public override bool Visible
		{
			get { return base.Visible; }
			set
			{
				if (string.IsNullOrEmpty(_id))
				{
					base.Visible = false;
					return;
				}
				base.Visible = value;
			}
		}
	}
}